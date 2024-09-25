using Microsoft.AspNetCore.Mvc;

namespace DentalCare.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Profile()
        {
            return View();
        }

        public IActionResult Invoice()
        {
            return View();
        }

        public IActionResult InvoiceDetail()
        {
            return View();
        }
    }
}
