using Microsoft.AspNetCore.Mvc;

namespace DentalCare.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Recover()
        {
            return View();
        }
    }
}
