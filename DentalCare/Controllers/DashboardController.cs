using DentalCare.Models;
using DentalCare.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentalCare.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private AccountService _accountService;
        private DoctorService _doctorService;
        private ReceptionistService _receptionistService;

        public DashboardController(AccountService accountService, DoctorService doctorService,
            ReceptionistService receptionistService)
        {
            _accountService = accountService;
            _doctorService = doctorService;
            _receptionistService = receptionistService;
        }

        [Route("dashboard")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
