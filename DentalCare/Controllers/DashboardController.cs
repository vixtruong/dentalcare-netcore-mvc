using Microsoft.AspNetCore.Mvc;

namespace DentalCare.Controllers
{
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
