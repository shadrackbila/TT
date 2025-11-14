using Firebase.Auth;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TimelyTastes.Data;
using TimelyTastes.Models;

namespace TimelyTastes.Controllers
{
    public class LogInController : Controller
    {
        private const string API_KEY = "AIzaSyAgBBP_mB5WAJQC9sZMRAjoq7dNUxplKtU";
        FirebaseAuthProvider _firebaseAuth = new FirebaseAuthProvider(new FirebaseConfig(API_KEY));

        private readonly SQLiteDbContext _context;


        public LogInController(SQLiteDbContext context)
        {
            _context = context;
        }



        [HttpGet]
        public async Task<IActionResult> LogIn()
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


                    if (_context.Vendors.Any(v => v.VendorID == vendorId))
                    {
                        return RedirectToAction("Index", "Listings");
                    }
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