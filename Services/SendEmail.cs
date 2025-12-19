using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Mvc;
using TimelyTastes.Models;
using TimelyTastes.Services;

namespace TimelyTastes.Controllers
{
    public class Email : ISendEmail
    {

        private readonly string _returnUrl;
        public Email()
        {
            _returnUrl = "";

        }
        public Email(IConfiguration config)
        {
            if (string.IsNullOrWhiteSpace(config["returnUrl:url"]))
                throw new ArgumentException("Return URL is not configured.");

            _returnUrl = config["returnUrl:url"]!.TrimEnd('/');
        }

        public bool SendEmail(Orders order)
        {
            bool status = false;

            // Recipient info
            string recipientEmail = !string.IsNullOrWhiteSpace(order.Email) ? order.Email : "timelytastes.PTY@gmail.com";
            string senderAddress = "timelytastes.PTY@gmail.com";

            // Email content
            string subject = $"Order Confirmation - #{order.OrderNumber}";


            string body = $@"
<div style=""font-family: Arial, sans-serif; background-color: #f9fafb; padding-top:28px;"">
    <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"" style=""max-width:600px; margin:auto; background-color:#ffffff; border-radius:12px; box-shadow:0 4px 6px rgba(0,0,0,0.1);"">
        <!-- Header -->
        <tr>
            <td style=""background-color:#4CAF50; color:#ffffff; text-align:center; padding:24px; border-top-left-radius:12px; border-top-right-radius:12px;"">
                <h1 style=""margin:0; font-size:24px; font-weight:bold;"">Order Confirmed!</h1>
                <p style=""margin:4px 0 0; opacity:0.9;"">Your payment was successful</p>
            </td>
        </tr>

        <!-- Order Details -->
        <tr>
            <td style=""padding:24px;"">
                <h2 style=""font-size:18px; font-weight:bold; color:#111827;"">Order Summary</h2>

                <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"" style=""margin-top:16px;"">
                    <tr>
                        <td style=""width:50%; padding:4px 0;"">
                            <p style=""margin:0; font-size:14px; color:#6b7280;"">Order Number</p>
                            <p style=""margin:2px 0 0; font-weight:bold; color:#111827;"">#{order.OrderNumber}</p>
                        </td>
                        <td style=""width:50%; padding:4px 0;"">
                            <p style=""margin:0; font-size:14px; color:#6b7280;"">Shop Name</p>
                            <p style=""margin:2px 0 0; font-weight:bold; color:#111827;"">{order.Vendor.Name}</p>
                        </td>
                    </tr>
                </table>

                <p style=""margin-top:16px; font-size:14px; color:#6b7280;"">Pickup Location</p>
                <p style=""margin:2px 0 0; font-weight:bold; color:#111827;"">{order.Vendor.Address}</p>
                <p style=""margin:2px 0 0; font-size:12px; color:#9ca3af;"">Near the cashier counter</p>

                <table width=""100%"" cellpadding=""0"" cellspacing=""0"" border=""0"" style=""margin-top:16px;"">
                    <tr>
                        <td style=""width:50%; padding:4px 0;"">
                            <p style=""margin:0; font-size:14px; color:#6b7280;"">Pickup Time</p>
                            <p style=""margin:2px 0 0; font-weight:bold; color:#111827;"">{order.Listing.AvailableFrom: d MMMM, h:mm tt}</p>
                        </td>
                        <td style=""width:50%; padding:4px 0;"">
                            <p style=""margin:0; font-size:14px; color:#6b7280;"">Expires At</p>
                            <p style=""margin:2px 0 0; font-weight:bold; color:#111827;"">{order.Listing.AvailableUntil: d MMMM, h:mm tt}</p>
                        </td>
                    </tr>
                </table>

                <!-- OTP Section -->
                <div style=""margin-top:24px; padding:16px; border:1px solid #FFC107; border-radius:8px; background-color:rgba(255, 193, 7, 0.18);;"">
                    <p style=""margin:0; font-size:14px; color:#6b7280;"">Collection OTP</p>
                    <p style=""margin:4px 0 0; font-size:24px; font-weight:bold; color:#111827;"">{order.OTP}</p>
                    <p style=""margin:4px 0 0; font-size:12px; color:#6b7280;"">Show this code to collect your order</p>
                </div>

                <!-- Actions -->
                <div style=""margin-top:24px;"">
                    <a href=""#"" style=""display:block; text-align:center; text-decoration:none; background-color:#4CAF50; color:#ffffff; padding:12px 0; border-radius:8px; font-weight:bold; margin-bottom:8px;"">
                        View in maps
                    </a>
                    <a href=""#"" style=""display:block; text-align:center; text-decoration:none; border:1px solid #4CAF50; color:#4CAF50; padding:12px 0; border-radius:8px; font-weight:bold;"">
                        Share Receipt
                    </a>
                </div>
            </td>
        </tr>

        <!-- Footer -->
        <tr>
            <td style=""background-color:#f3f4f6; text-align:center; font-size:12px; color:#6b7280; padding:16px; border-bottom-left-radius:12px; border-bottom-right-radius:12px;"">
                Need help? <a href=""#"" style=""color:#4CAF50; text-decoration:none; font-weight:bold;"">Contact Us</a>
            </td>
        </tr>
    </table>
</div>";







            // SMTP setup - kefz uahx cjxf fsqu
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(senderAddress, "kefz uahx cjxf fsqu"),
                EnableSsl = true
            };

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(senderAddress);
            mailMessage.To.Add(recipientEmail);
            mailMessage.Subject = subject;
            mailMessage.Body = body;
            mailMessage.IsBodyHtml = true;




            try
            {
                client.Send(mailMessage);
                Console.WriteLine("Confirmation email sent successfully!");
                status = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending email: " + ex.Message);
                status = false;
            }

            return status;
        }



        public bool RequestRating(Orders order, RatingInvitation invitation)
        {
            bool status = false;

            // Generate the rating link
            var ratingLink = $"{_returnUrl}/Ratings/Rate?token={invitation.Token}";

            // Recipient info
            string recipientEmail = !string.IsNullOrWhiteSpace(order.Email) ? order.Email : "timelytastes.PTY@gmail.com";
            string senderAddress = "timelytastes.PTY@gmail.com";

            // Email content
            string subject = $"Your order has been collected — #{order.OrderNumber} ";


            string body = $@"
<div style=""max-width:600px; margin:0 auto; padding:24px; background-color:#ffffff; border-radius:12px; font-family:Arial, sans-serif;"">

    <!-- Header -->
    <h1 style=""font-size:24px; font-weight:bold; color:#111827; margin-bottom:12px;"">
        Order Collected
    </h1>

    <p style=""font-size:14px; color:#374151; margin-bottom:20px;"">
        Your order has been successfully collected. We’d love to hear your feedback to help us improve your experience.
    </p>

   

    <!-- CTA -->
    <a href=""{ratingLink}""
       style=""display:block; text-align:center; background-color:#4CAF50; color:#ffffff; padding:14px 0;
              border-radius:8px; font-weight:bold; text-decoration:none; font-size:16px;"">
        Rate Your Order
    </a>

    <!-- Footer -->
    <div style=""text-align:center; font-size:12px; color:#6b7280; margin-top:20px;"">
        Need help?
        <a href=""{ratingLink}""
           style=""color:#4CAF50; text-decoration:none; font-weight:bold;"">
            Contact Us
        </a>
    </div>

</div>";




            // SMTP setup - kefz uahx cjxf fsqu
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(senderAddress, "kefz uahx cjxf fsqu"),
                EnableSsl = true
            };

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(senderAddress);
            mailMessage.To.Add(recipientEmail);
            mailMessage.Subject = subject;
            mailMessage.Body = body;
            mailMessage.IsBodyHtml = true;




            try
            {
                client.Send(mailMessage);
                Console.WriteLine("Collection email sent successfully!");
                status = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending email: " + ex.Message);
                status = false;
            }

            return status;
        }
    }
}