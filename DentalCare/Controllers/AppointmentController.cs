using Microsoft.AspNetCore.Mvc;

namespace DentalCare.Controllers
{
    public class AppointmentController : Controller
    {
        public IActionResult Add()
        {
            return View();
        }

        public String Edit()
        {
            return "View()";
        }

        public IActionResult Manage()
        {
            return View();
        }
    }
}
