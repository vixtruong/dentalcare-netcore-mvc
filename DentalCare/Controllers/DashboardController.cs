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

        public IActionResult AddInvoice()
        {
            return View();
        }

        public IActionResult InvoiceManage()
        {
            return View();
        }

        public IActionResult InvoiceDetail()
        {
            return View();
        }

        public IActionResult AddEmployee()
        {
            return View();
        }

        public IActionResult EmployeesManage()
        {
            return View();
        }

        public IActionResult AddShift()
        {
            return View();
        }

        public IActionResult ShiftManage()
        {
            return View();
        }

        public IActionResult AddAppointment()
        {
            return View();
        }

        public IActionResult AppointmentManage()
        {
            return View();
        }
    }
}
