using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentalCare.Controllers
{
    [Authorize]
    public class InvoiceController : Controller
    {
        public IActionResult Add()
        {
            return View();
        }

        [Route("invoice")]
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
