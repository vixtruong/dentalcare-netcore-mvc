using DentalCare.Models;
using DentalCare.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using X.PagedList.Extensions;

namespace DentalCare.Controllers
{
    [Authorize]
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

        [Route("shift/add")]
        [HttpGet]
        public IActionResult Add()
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole.Contains("D"))
            {
                return NotFound();
            }

            ViewBag.Nurses = _nurseService.GetAll();
            ViewBag.Faculties = _facultyService.GetAll();

            if (ViewBag.Faculties == null || ViewBag.Nurses == null)
            {
                return Content("Data not found");
            }
            return View();
        }

        [Route("shift/add")]
        [HttpPost]
        public IActionResult Add(Shift shift)
        {
            if (_shiftService.GetAll().Any(x => x.Doctorid == shift.Doctorid && x.Date == shift.Date))
            {
                TempData["ErrorMessage"] = "The selected doctor already has a shift scheduled on this date.";
                return RedirectToAction("Add", shift);
            }

            if (_shiftService.GetAll().Any(x => x.Nurseid == shift.Nurseid && x.Date == shift.Date))
            {
                TempData["ErrorMessage"] = "The selected nurse already has a shift scheduled on this date.";
                return RedirectToAction("Add", shift);
            }

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

        [Route("shift/edit/{id}")]
        [HttpGet]
        public IActionResult Edit(string id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole.Contains("D"))
            {
                return NotFound();
            }

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

        [Route("shift/edit/{id}")]
        [HttpPost]
        public IActionResult Edit(Shift model)
        {
            var shift = _shiftService.Get(model.Id);

            if (_shiftService.GetAll().Any(x => x.Doctorid == shift.Doctorid && x.Date == shift.Date))
            {
                TempData["ErrorMessage"] = "The selected doctor already has a shift scheduled on this date.";
                return RedirectToAction("Edit", model);
            }

            if (_shiftService.GetAll().Any(x => x.Nurseid == shift.Nurseid && x.Date == shift.Date))
            {
                TempData["ErrorMessage"] = "The selected nurse already has a shift scheduled on this date.";
                return RedirectToAction("Edit", model);
            }

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

        [Route("shift/manage")]
        public IActionResult Manage(int? page, string sortColumn, string sortDirection, string searchQuery)
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole.Contains("D"))
            {
                return NotFound();
            }

            ViewBag.Doctors = _doctorService.GetAll();
            ViewBag.Nurses = _nurseService.GetAll();
            var shiftList = _shiftService.GetAll();
            int pageSize = 10;
            int pageNumber = (page ?? 1);

            if (!string.IsNullOrEmpty(searchQuery))
            {
                shiftList = shiftList.Where(a => a.Id.Contains(searchQuery) ||
                                                 a.Date.ToString("dd-MM-yyyy").Contains(searchQuery) ||
                                                 a.Doctorid.Contains(searchQuery) ||
                                                 a.Nurseid.Contains(searchQuery)).ToList();

                ViewBag.SearchQuery = searchQuery;
            }

            shiftList = (sortColumn switch
            {
                "Date" => sortDirection == "desc" ? shiftList.OrderByDescending(a => a.Date) : shiftList.OrderBy(a => a.Date),
                "Doctor" => sortDirection == "desc" ? shiftList.OrderByDescending(a => a.Doctor?.Name ?? "") : shiftList.OrderBy(a => a.Doctor?.Name ?? ""),
                "Doctor ID" => sortDirection == "desc" ? shiftList.OrderByDescending(a => a.Doctor?.Id ?? "") : shiftList.OrderBy(a => a.Doctor?.Id ?? ""),
                "ID" => sortDirection == "desc" ? shiftList.OrderByDescending(a => a.Id) : shiftList.OrderBy(a => a.Id),
                _ => shiftList.OrderBy(a => a.Id)
            }).ToList();

            ViewBag.SortColumn = sortColumn;
            ViewBag.SortDirection = sortDirection;
            ViewBag.NextSortDirection = sortDirection == "asc" ? "desc" : "asc";

            var pagedList = shiftList.ToPagedList(pageNumber, pageSize);

            return View(pagedList);
        }
    }
}
