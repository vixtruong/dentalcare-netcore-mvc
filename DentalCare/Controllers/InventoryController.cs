using Microsoft.AspNetCore.Mvc;

namespace DentalCare.Controllers
{
    public class InventoryController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
