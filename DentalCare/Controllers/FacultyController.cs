using DentalCare.Models;
using DentalCare.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentalCare.Controllers
{
    [Authorize]
    public class FacultyController : Controller
    {
        private readonly FacultyService _facultyService;

        public FacultyController(FacultyService facultyService)
        {
            _facultyService = facultyService;
        }

        [Route("doctor-faculty/add")]
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [Route("doctor-faculty/add")]
        [HttpPost]
        public IActionResult Add(string name)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }

            var faculty = new Faculty
            {
                Id = _facultyService.GenerateID(),
                Name = name
            };
            
            _facultyService.Add(faculty);
            return RedirectToAction("Manage", "Employee", new { role = "Doctor"});
        }
    }
}
