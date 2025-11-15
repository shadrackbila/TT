using Firebase.Auth;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TimelyTastes.Data;
using TimelyTastes.Models;

namespace TimelyTastes.Controllers
{
    public class SignUpController : Controller
    {
        private const string API_KEY = "AIzaSyAgBBP_mB5WAJQC9sZMRAjoq7dNUxplKtU";
        FirebaseAuthProvider _firebaseAuth = new FirebaseAuthProvider(new FirebaseConfig(API_KEY));

        private readonly SQLiteDbContext _context;


        public SignUpController(SQLiteDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()

        {

            return View();
        }



        [HttpPost]
        public async Task<IActionResult> Register(SignUpModel vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            try
            {
                await _firebaseAuth.CreateUserWithEmailAndPasswordAsync(vm.Email, vm.Password);
                var firebaseLink = await _firebaseAuth.SignInWithEmailAndPasswordAsync(vm.Email, vm.Password);

                var accessToken = firebaseLink.FirebaseToken;
                var vendorId = firebaseLink.User.LocalId;

                if (!string.IsNullOrEmpty(accessToken))
                {
                    HttpContext.Session.SetString("AccessToken", accessToken);
                    HttpContext.Session.SetString("VendorID", vendorId);


                    return RedirectToAction("Create", "Vendors");

                }

                return View(vm);
            }
            catch (FirebaseAuthException ex)
            {
                var firebaseex = JsonConvert.DeserializeObject<ErrorModel>(ex.RequestData);
                ModelState.AddModelError(string.Empty, firebaseex.message);
                return View(vm);

            }
        }





    }
}