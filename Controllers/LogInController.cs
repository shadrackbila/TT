using Firebase.Auth;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TimelyTastes.Data;
using TimelyTastes.Models;

namespace TimelyTastes.Controllers
{
    public class LogInController : Controller
    {
        private string _apiKey;
        FirebaseAuthProvider _firebaseAuth;

        private readonly SQLiteDbContext _context;


        public LogInController(SQLiteDbContext context, IConfiguration config)
        {
            _apiKey = config["ApiSettings:ApiKey"] ?? "";
            _context = context;
            _firebaseAuth = new FirebaseAuthProvider(new FirebaseConfig(_apiKey));
        }



        [HttpGet]
        public IActionResult LogIn()
        {
            var vendorId = HttpContext.Session.GetString("VendorID");

            if (!string.IsNullOrEmpty(vendorId))
            {


                if (_context.Vendors.Any(v => v.VendorID == vendorId))
                {
                    return RedirectToAction("Index", "Listings");
                }
            }

            return View();

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogIn(LogInModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            try
            {
                var firebaseLink = await _firebaseAuth.SignInWithEmailAndPasswordAsync(vm.Email, vm.Password);

                var accessToken = firebaseLink.FirebaseToken;
                var vendorId = firebaseLink.User.LocalId;

                if (!string.IsNullOrEmpty(accessToken))
                {
                    HttpContext.Session.SetString("AccessToken", accessToken);
                    HttpContext.Session.SetString("VendorID", vendorId);

                    var vendor = await _context.Vendors.FirstOrDefaultAsync(v => v.VendorID == vendorId);


                    if (vendor == null)
                        return RedirectToAction("Create", "Vendors");

                    if (vendor.IsDeleted)
                    {
                        HttpContext.Session.Remove("VendorID");
                        HttpContext.Session.Remove("AccessToken");
                        return RedirectToAction("Register", "SignUp");
                    }

                    return RedirectToAction("Index", "Listings");

                }

                return View(vm);
            }
            catch (FirebaseAuthException ex)
            {
                var firebaseex = JsonConvert.DeserializeObject<ErrorModel>(ex.RequestData);
                ViewBag.ErrorMessage = firebaseex?.message ?? "Unknown error";
                return View(vm);

            }
        }



        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("LogIn");
        }

    }
}