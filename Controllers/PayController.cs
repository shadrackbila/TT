using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TimelyTastes.Data;
using TimelyTastes.Models;
using TimelyTastes.Services;

namespace TimelyTastes.Controllers
{
    public class PayController : Controller
    {
        private readonly SQLiteDbContext _context;

        private readonly IPayment _payment;

        readonly string PayGateID = "10011072130";
        readonly string PayGateKey = "secret";

        public PayController(SQLiteDbContext context)
        {
            _context = context;
            _payment = new Payment(_context);
        }

        public ActionResult Index()
        {
            return View();
        }




        [HttpGet("pay/getrequest")]
        public async Task<IActionResult> GetRequest(int? listingId, string email)
        {
            try
            {

                if (listingId == null || string.IsNullOrEmpty(email))
                {
                    return NotFound();
                }

                var listing = await _context.Listings
                    .FirstOrDefaultAsync(m => m.Id == listingId);
                if (listing == null)
                {
                    return NotFound();
                }


                HttpClient http = new HttpClient();
                Dictionary<string, string> request = new Dictionary<string, string>();

                int amountInCents = (int)(listing.DiscountPrice * 100);
                string paymentAmount = amountInCents.ToString();  // amount in cents

                var random = new Random();
                string paymentReference = random.Next(10000, 99999).ToString();



                var order = new Orders
                {
                    ID = Guid.NewGuid(),
                    OrderDate = DateTime.UtcNow,
                    OrderNumber = paymentReference,
                    OTP = new Random().Next(1000, 9999).ToString(),
                    OrderStatus = "Failed",
                    Listing = listing,
                    Email = email,
                    Vendor = await _context.Vendors.FirstOrDefaultAsync(v => v.VendorID == listing.VendorID)
                };

                _context.Orders.Add(order);


                request.Add("PAYGATE_ID", PayGateID);
                request.Add("REFERENCE", paymentReference); // Payment ref e.g ORDER NUMBER
                request.Add("AMOUNT", paymentAmount);
                request.Add("CURRENCY", "ZAR"); // South Africa
                request.Add("RETURN_URL", "https://soaringly-suborbicular-erika.ngrok-free.dev/pay/completepayment");
                request.Add("TRANSACTION_DATE", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                request.Add("LOCALE", "en-za");
                request.Add("COUNTRY", "ZAF");
                request.Add("EMAIL", "timelytastes.PTY@gmail.com");

                // Generate checksum
                request.Add("CHECKSUM", _payment.GetMd5Hash(request, PayGateKey));

                string requestString = _payment.ToUrlEncodedString(request);
                StringContent content = new StringContent(requestString, Encoding.UTF8, "application/x-www-form-urlencoded");

                HttpResponseMessage response = await http.PostAsync("https://secure.paygate.co.za/payweb3/initiate.trans", content);
                response.EnsureSuccessStatusCode();

                string responseContent = await response.Content.ReadAsStringAsync();
                Dictionary<string, string> results = _payment.ToStringResponse(responseContent);

                if (results.ContainsKey("ERROR"))
                {
                    return Content("<h2>An error occurred while initiating your request.</h2>", "text/html");
                }

                if (!_payment.VerifyMd5Hash(results, PayGateKey, results["CHECKSUM"]))
                {
                    return Content("<h2>MD5 verification failed.</h2>", "text/html");
                }

                // Ensure AddTransaction always succeeds
                bool isRecorded = _payment.AddTransaction(request, results["PAY_REQUEST_ID"]);


                // Build auto-submit form to PayGate
                string payRequestId = results["PAY_REQUEST_ID"];
                string checksum = results["CHECKSUM"];

                if (isRecorded)
                {
                    string form = $@"
<html>
    <body onload='document.forms[0].submit()'>
        <form action='https://secure.paygate.co.za/payweb3/process.trans' method='POST'>
            <input type='hidden' name='PAY_REQUEST_ID' value='{payRequestId}' />
            <input type='hidden' name='CHECKSUM' value='{checksum}' />
            <noscript>
                <input type='submit' value='Click here to continue to PayGate' /> 
            </noscript>
        </form>
        <p style='text-align:center;'>Redirecting to PayGate...</p>
    </body>
</html>";

                    return Content(form, "text/html");
                }
                else
                {
                    return Json("Something went wrong");
                }
            }
            catch (Exception ex)
            {
                // Log exception somewhere for debugging
                return Content($"<h2>An unexpected error occurred: {ex.Message}</h2>", "text/html");
            }
        }






        [HttpPost("pay/completepayment")]
        public async Task<ActionResult> CompletePayment()
        {
            // string responseContent = Request.Params.ToString();
            // Dictionary<string, string> results = _payment.ToStringResponse(responseContent);

            var form = await Request.ReadFormAsync();
            Dictionary<string, string> results = form.ToDictionary(x => x.Key, x => x.Value.ToString());


            Transaction transaction = _payment.GetTransaction(results["PAY_REQUEST_ID"]);

            if (transaction == null)
            {
                // Unable to reconsile transaction
                return RedirectToAction("Failed");
            }

            // Reorder attributes for MD5 check
            Dictionary<string, string> validationSet = new Dictionary<string, string>();
            validationSet.Add("PAYGATE_ID", PayGateID);
            validationSet.Add("PAY_REQUEST_ID", results["PAY_REQUEST_ID"]);
            validationSet.Add("TRANSACTION_STATUS", results["TRANSACTION_STATUS"]);
            validationSet.Add("REFERENCE", transaction.REFERENCE);

            if (!_payment.VerifyMd5Hash(validationSet, PayGateKey, results["CHECKSUM"]))
            {
                // checksum error
                return RedirectToAction("Failed");
            }
            /** Payment Status 
             * -2 = Unable to reconsile transaction
             * -1 = Checksum Error
             * 0 = Pending
             * 1 = Approved
             * 2 = Declined
             * 3 = Cancelled
             * 4 = User Cancelled
             */
            int paymentStatus = int.Parse(results["TRANSACTION_STATUS"]);

            if (paymentStatus == 1)
            {
                var existingTransaction = await _context.Transactions
                  .FirstOrDefaultAsync(t => t.REFERENCE == transaction.REFERENCE);

                if (existingTransaction == null)
                {
                    // Transaction not found — prevent proceeding
                    paymentStatus = 404;
                    return RedirectToAction("Complete", new { id = paymentStatus });
                }

                // Optionally double-check the PAY_REQUEST_ID too
                if (existingTransaction.PAY_REQUEST_ID != results["PAY_REQUEST_ID"])
                {
                    // Mismatch — could be tampering
                    paymentStatus = -2;
                    return RedirectToAction("Complete", new { id = paymentStatus });
                }

                TempData["PaymentStatus"] = paymentStatus;
                TempData["sendEmail"] = true;


                if (transaction.REFERENCE == null)
                {
                    return NotFound();
                }

                var Order = await _context.Orders
                   .Include(o => o.Vendor)
                      .Include(o => o.Listing)
                    .FirstOrDefaultAsync(m => m.OrderNumber == transaction.REFERENCE);

                if (Order == null)
                {
                    return NotFound();
                }

                var listing = await _context.Listings
                    .FirstOrDefaultAsync(m => m.Id == Order.Listing.Id);
                if (listing == null)
                {
                    return NotFound();
                }
                Order.OrderStatus = "Active";
                listing.QuantityAvailable = listing.QuantityAvailable - 1;
                await _context.SaveChangesAsync();

                // Redirect without showing the ID
                return RedirectToAction("Approved", new { orderNumber = Order.OrderNumber });

            }
            // Query paygate transaction details
            // And update user transaction on your database
            // await VerifyTransaction(responseContent, transaction.REFERENCE);

            string responseContent = string.Join("&", results.Select(r => $"{r.Key}={r.Value}"));
            await VerifyTransaction(responseContent, transaction.REFERENCE);

            return RedirectToAction("Complete", new { id = results["TRANSACTION_STATUS"] });
        }


        private async Task VerifyTransaction(string responseContent, string Referrence)
        {
            HttpClient client = new HttpClient();
            Dictionary<string, string> response = _payment.ToStringResponse(responseContent);
            Dictionary<string, string> request = new Dictionary<string, string>();

            request.Add("PAYGATE_ID", PayGateID);
            request.Add("PAY_REQUEST_ID", response["PAY_REQUEST_ID"]);
            request.Add("REFERENCE", Referrence);
            request.Add("CHECKSUM", _payment.GetMd5Hash(request, PayGateKey));

            string requestString = _payment.ToUrlEncodedString(request);

            StringContent content = new StringContent(requestString, Encoding.UTF8, "application/x-www-form-urlencoded");
            HttpResponseMessage res = await client.PostAsync("https://secure.paygate.co.za/payweb3/query.trans", content);
            res.EnsureSuccessStatusCode();

            string _responseContent = await res.Content.ReadAsStringAsync();

            Dictionary<string, string> results = _payment.ToStringResponse(_responseContent);

            if (!results.Keys.Contains("ERROR"))
            {
                _payment.UpdateTransaction(results, results["PAY_REQUEST_ID"]);
            }
        }


        public async Task<ViewResult> Approved(string orderNumber)
        {
            if (TempData["PaymentStatus"] == null)
                return View("NotFound");

            TempData.Keep("PaymentStatus");


            var order = await _context.Orders
                .Include(o => o.Vendor)   // Load Vendor
                .Include(o => o.Listing)  // Load Listing
                .FirstOrDefaultAsync(o => o.OrderNumber == orderNumber);

            if (order == null)
                return View("NotFound");

            bool sendEmail = (bool)(TempData["sendEmail"] ?? false);
            if (sendEmail)
            {
                ISendEmail em = new Email();
                em.SendEmail(order);
                TempData["sendEmail"] = false;
            }


            return View(order);
        }



        public ViewResult Complete(int? id)
        {
            string status = "Unknown";
            switch (id.ToString())
            {
                case "-2":
                    status = "Reconsile";
                    break;
                case "-1":
                    status = "Error";
                    break;
                case "0":
                    status = "NotDone";
                    break;

                case "2":
                    status = "Declined";
                    break;
                case "3":
                    status = "Cancelled";
                    break;
                case "4":
                    status = "UserCancelled";
                    break;
                default:
                    // status = $"Unknown Status({id})";
                    status = $"NotFound";

                    break;
            }
            TempData["Status"] = status;

            return View(status);
        }

    }
}