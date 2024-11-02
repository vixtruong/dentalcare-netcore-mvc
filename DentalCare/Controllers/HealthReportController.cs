using DentalCare.Models;
using DentalCare.Services;
using Microsoft.AspNetCore.Mvc;
using X.PagedList.Extensions;

namespace DentalCare.Controllers
{
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

        public IActionResult Index(int? page)
        {
            ViewBag.MedicalExams = _medicalExamService.GetAll();
            ViewBag.Doctors = _doctorService.GetAll();
            ViewBag.Customers = _customerService.GetAll();

            int pageNumber = (page ?? 1);
            int pageSize = 10;

            var hrList = _healthReportService.GetAll();
            var pagedList = hrList.ToPagedList(pageNumber, pageSize);
            return View(pagedList);
        }

        [HttpGet]
        public IActionResult Add()
        {
            ViewBag.MedicalExams = _medicalExamService.GetAll();
            return View();
        }

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

        [HttpGet]
        public IActionResult Edit(string id)
        {
            ViewBag.MedicalExams = _medicalExamService.GetAll();
            var healthReport = _healthReportService.Get(id);
            return View(healthReport);
        }

        [HttpPost]
        public IActionResult Edit(Healthreport model)
        {
            var healthReport = _healthReportService.Get(model.Id);
            healthReport.Status = model.Status;

            _healthReportService.Update(healthReport);
           return RedirectToAction("Index");
        }

        public IActionResult Delete(string id)
        {
            return RedirectToAction("Index");
        }
    }
}
