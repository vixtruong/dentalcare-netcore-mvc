using DentalCare.Models;
using DentalCare.Services;
using Microsoft.AspNetCore.Mvc;

namespace DentalCare.Controllers
{
    public class ShiftController : Controller
    {
        private DoctorService _doctorService;
        private NurseService _nurseService;
        private FacultyService _facultyService;
        private ShiftService _shiftService;

        public ShiftController(DoctorService doctorService, NurseService nurseService, FacultyService facultyService, ShiftService shiftService)
        {
            _doctorService = doctorService;
            _nurseService = nurseService;
            _facultyService = facultyService;
            _shiftService = shiftService;
        }

        [HttpGet]
        public JsonResult GetDoctorsByFaculty(string facultyId)
        {
            var doctors = _doctorService.GetByFacultyId(facultyId);
            var doctorList = doctors.Select(d => new { id = d.Id, name = d.Name }).ToList();
            return Json(doctorList);
        }


        [HttpGet]
        public IActionResult Add()
        {
            ViewBag.Nurses = _nurseService.GetAll();
            ViewBag.Faculties = _facultyService.GetAll();

            if (ViewBag.Faculties == null || ViewBag.Nurses == null)
            {
                return Content("Data not found");
            }
            return View();
        }

        [HttpPost]
        public IActionResult Add(Shift shift)
        {
            var newShift = new Shift
            {
                Id = _shiftService.GenerateID(),
                Doctorid = shift.Doctorid,
                Nurseid = shift.Nurseid,
                Date = shift.Date
            };

            _shiftService.Add(newShift);
            return RedirectToAction("Manage", "Shift");
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            var shift = _shiftService.Get(id);
                
            ViewBag.Doctor = _doctorService.Get(shift.Doctorid);
            ViewBag.Nurses = _nurseService.GetAll();
            ViewBag.Faculties = _facultyService.GetAll();

            if (ViewBag.Faculties == null || ViewBag.Nurses == null)
            {
                return Content("Data not found");
            }

            return View(shift);
        }

        [HttpPost]
        public IActionResult Edit(Shift model)
        {
            var shift = _shiftService.Get(model.Id);

            shift.Id = model.Id;
            shift.Date = model.Date;
            shift.Doctorid = model.Doctorid;
            shift.Nurseid = model.Nurseid;

            _shiftService.Update(shift);
            return RedirectToAction("Manage", "Shift");
        }

        [HttpGet]
        public IActionResult Delete(string id)
        {
            _shiftService.Delete(id);
            return RedirectToAction("Manage");
        }

        public IActionResult Manage()
        {
            var shiftList = _shiftService.GetAll();
            ViewBag.Doctors = _doctorService.GetAll();
            ViewBag.Nurses = _nurseService.GetAll();
            return View(shiftList);
        }
    }
}
