using Firebase.Auth;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TimelyTastes.Models;

namespace TimelyTastes.Controllers
{
    public class LogInController : Controller
    {
        private const string API_KEY = "AIzaSyAgBBP_mB5WAJQC9sZMRAjoq7dNUxplKtU";
        FirebaseAuthProvider _firebaseAuth = new FirebaseAuthProvider(new FirebaseConfig(API_KEY));

        public IActionResult Index()
        {

            return View();
        }

        public IActionResult Create()
        {

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterUser(LogInModel vm)
        {

            try
            {

                await _firebaseAuth.CreateUserWithEmailAndPasswordAsync(vm.Email, vm.Password);
                var firebaseLink = await _firebaseAuth.SignInWithEmailAndPasswordAsync(vm.Email, vm.Password);
                string accessToken = firebaseLink.FirebaseToken;

                if (accessToken != null)
                {
                    HttpContext.Session.SetString("AccessToken", accessToken);
                    return RedirectToAction("Index", "Listings");

                }
                else
                {
                    return View("Index");
                }

            }
            catch (FirebaseAuthException ex)
            {
                var firebaseex = JsonConvert.DeserializeObject<ErrorModel>(ex.RequestData);
                ModelState.AddModelError(string.Empty, firebaseex.message);
                return View("Index");

            }
        }
    }
}