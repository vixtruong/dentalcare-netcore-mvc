using DentalCare.Models;
using DentalCare.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using X.PagedList.Extensions;

namespace DentalCare.Controllers
{
    [Authorize]
    public class HealthReportController : Controller
    {
        private readonly HealthReportService _healthReportService;
        private readonly DoctorService _doctorService;
        private readonly CustomerService _customerService;
        private readonly MedicalExamService _medicalExamService;

        public HealthReportController(HealthReportService healthReportService, DoctorService doctorService,
            CustomerService customerService, MedicalExamService medicalExamService)
        {
            _healthReportService = healthReportService;
            _doctorService = doctorService;
            _customerService = customerService;
            _medicalExamService = medicalExamService;
        }

        public JsonResult GetInformationById(string id)
        {
            var mes = _medicalExamService.Get(id);
            if (mes == null)
            {
                return Json(new { success = false, message = "Medical Exam not found." });
            }

            var doctor = _doctorService.GetAll().FirstOrDefault(x => x.Id == mes.Doctorid);
            var customer = _customerService.GetAll().FirstOrDefault(x => x.Id == mes.Customerid);

            if (doctor == null || customer == null)
            {
                return Json(new { success = false, message = "Doctor or Customer not found." });
            }

            var data = new
            {
                DoctorId = doctor.Id,
                DoctorName = doctor.Name,
                CustomerId = customer.Id,
                CustomerName = customer.Name
            };

            return Json(new { success = true, data });
        }

        [Route("health-record/index")]
        public IActionResult Index(int? page, string sortColumn, string sortDirection, string searchQuery)
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole.Contains("R"))
            {
                return NotFound();
            }

            var userId = HttpContext.Session.GetString("UserId");

            ViewBag.MedicalExams = _medicalExamService.GetAll();
            ViewBag.Doctors = _doctorService.GetAll();
            ViewBag.Customers = _customerService.GetAll();

            var prescriptions = _healthReportService.GetAll().Where(x => x.Medicalexamination.Doctorid == userId);
            int pageNumber = (page ?? 1);
            int pageSize = 10;

            if (!string.IsNullOrEmpty(searchQuery))
            {
                prescriptions = prescriptions.Where(a => a.Id.Contains(searchQuery) ||
                                                         a.MedicalexaminationId.ToString().Contains(searchQuery) ||
                                                         a.Date.ToString("dd-MM-yyyy").Contains(searchQuery) ||
                                                         a.Medicalexamination.Doctorid.Contains(searchQuery) ||
                                                         a.Medicalexamination.Customerid.Contains(searchQuery)).ToList();

                ViewBag.SearchQuery = searchQuery;
            }

            prescriptions = (sortColumn switch
            {
                "Date" => sortDirection == "desc" ? prescriptions.OrderByDescending(a => a.Date) : prescriptions.OrderBy(a => a.Date),
                "MES" => sortDirection == "desc" ? prescriptions.OrderByDescending(a => a.Medicalexamination?.Id ?? "") : prescriptions.OrderBy(a => a.Medicalexamination?.Id ?? ""),
                "Customer ID" => sortDirection == "desc" ? prescriptions.OrderByDescending(a => a.Medicalexamination.Customer?.Id ?? "") : prescriptions.OrderBy(a => a.Medicalexamination.Customer?.Id ?? ""),
                "Customer" => sortDirection == "desc" ? prescriptions.OrderByDescending(a => a.Medicalexamination.Customer?.Name ?? "") : prescriptions.OrderBy(a => a.Medicalexamination.Customer?.Name ?? ""),
                "Doctor" => sortDirection == "desc" ? prescriptions.OrderByDescending(a => a.Medicalexamination.Doctor?.Name ?? "") : prescriptions.OrderBy(a => a.Medicalexamination.Doctor?.Name ?? ""),
                "Doctor ID" => sortDirection == "desc" ? prescriptions.OrderByDescending(a => a.Medicalexamination.Doctor?.Id ?? "") : prescriptions.OrderBy(a => a.Medicalexamination.Doctor?.Id ?? ""),
                "ID" => sortDirection == "desc" ? prescriptions.OrderByDescending(a => a.Id) : prescriptions.OrderBy(a => a.Id),
                "Fre" => sortDirection == "desc" ? prescriptions.OrderByDescending(a => a.Frequency) : prescriptions.OrderBy(a => a.Frequency),
                _ => prescriptions.OrderBy(a => a.Id)
            }).ToList();

            ViewBag.SortColumn = sortColumn;
            ViewBag.SortDirection = sortDirection;
            ViewBag.NextSortDirection = sortDirection == "asc" ? "desc" : "asc";


            var pagedList = prescriptions.ToPagedList(pageNumber, pageSize);
            return View(pagedList);
        }

        [Route("health-record/add")]
        [HttpGet]
        public IActionResult Add()
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole.Contains("R"))
            {
                return NotFound();
            }

            var userId = HttpContext.Session.GetString("UserId");

            ViewBag.MedicalExams = _medicalExamService.GetAll().Where(x => x.Doctorid == userId && x.Date == DateTime.Today);
            return View();
        }

        [Route("health-record/add")]
        [HttpPost]
        public IActionResult Add(Healthreport model)
        {
            var mes = _medicalExamService.Get(model.MedicalexaminationId);
            int frequency = _healthReportService.GetFrequencyByCustomerId(mes.Customerid);
            var newHealthReport = new Healthreport
            {
                Id = _healthReportService.GenerateID(),
                Date = DateTime.Today,
                Status = model.Status,
                MedicalexaminationId = model.MedicalexaminationId,
                CustomerId = mes.Customerid,
                Frequency = frequency + 1
            };
            _healthReportService.Add(newHealthReport);
            return RedirectToAction("Index");
        }

        [Route("health-record/edit/{id}")]
        [HttpGet]
        public IActionResult Edit(string id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole.Contains("R"))
            {
                return NotFound();
            }

            ViewBag.MedicalExams = _medicalExamService.GetAll();
            var healthReport = _healthReportService.Get(id);
            return View(healthReport);
        }

        [Route("health-record/edit/{model}")]
        [HttpPost]
        public IActionResult Edit(Healthreport model)
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole.Contains("R"))
            {
                return NotFound();
            }

            var healthReport = _healthReportService.Get(model.Id);
            healthReport.Status = model.Status;

            _healthReportService.Update(healthReport);
           return RedirectToAction("Index");
        }

        public IActionResult Delete(string id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole.Contains("R"))
            {
                return NotFound();
            }

            return RedirectToAction("Index");
        }
    }
}
