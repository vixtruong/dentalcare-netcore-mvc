using Microsoft.AspNetCore.Mvc;

namespace DentalCare.Controllers
{
    public class HealthReportController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Add()
        {
            return View();
        }
    }
}
