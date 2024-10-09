using Microsoft.AspNetCore.Mvc;

namespace DentalCare.Controllers
{
    public class InvoiceController : Controller
    {
        public IActionResult Add()
        {
            return View();
        }

        public IActionResult Manage()
        {
            return View();
        }

        public IActionResult Detail()
        {
            return View();
        }
    }
}
