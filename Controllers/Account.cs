using Microsoft.AspNetCore.Mvc;

namespace TimelyTastes.Controllers
{
    public class Account : Controller
    {

        public IActionResult Index()
        {

            return View();
        }
    }
}