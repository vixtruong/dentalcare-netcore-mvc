﻿using DentalCare.Models;
using DentalCare.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using X.PagedList.Extensions;

namespace DentalCare.Controllers
{
    [Authorize]
    public class InvoiceController : Controller
    {
        private readonly ReceptionistService _receptionistService;
        private readonly InvoiceService _invoiceService;
        private readonly CustomerService _customerService;
        private readonly MedicalExamService _medicalExamService;
        private readonly PrescriptionService _prescriptionService;
        private readonly PrescriptionDetailService _prescriptionDetailService;
        private readonly MedicineService _medicineService;
        private readonly TechSheetService _techSheetService;
        private readonly TechWorkService _techWorkService;
        private readonly TechDetailService _techDetailService;

        public InvoiceController(ReceptionistService receptionistService, InvoiceService invoiceService, CustomerService customerService, 
            MedicalExamService medicalExamService, PrescriptionService prescriptionService, PrescriptionDetailService prescriptionDetailService, 
            TechWorkService techWorkService, TechDetailService techDetailService, MedicineService medicineService, TechSheetService techSheetService)
        {
            _receptionistService = receptionistService;
            _invoiceService = invoiceService;
            _customerService = customerService;
            _medicalExamService = medicalExamService;
            _prescriptionService = prescriptionService;
            _techWorkService = techWorkService;
            _techDetailService = techDetailService;
            _prescriptionDetailService = prescriptionDetailService;
            _medicineService = medicineService;
            _techSheetService = techSheetService;
        }

        [Route("invoice")]
        public IActionResult Index(int? page)
        {
            var invoices = _invoiceService.GetAll();
            ViewBag.Receptionists = _receptionistService.GetAll();
            ViewBag.Customers = _customerService.GetAll();
            ViewBag.MedicalExams = _medicalExamService.GetAll();

            int pageNumber = (page ?? 1);
            int pageSize = 10;
            var pagedList = invoices.ToPagedList(pageNumber, pageSize);
            return View(pagedList);
        }

        [HttpGet]
        public JsonResult GetPrescriptionInformationByMesId(string mesId)
        {
            if (string.IsNullOrEmpty(mesId))
            {
                return Json(new { success = false, message = "MES ID không hợp lệ." });
            }

            var prescription = _prescriptionService.GetByMesId(mesId);
            if (prescription == null)
            {
                return Json(new { success = false, message = "Không tìm thấy đơn thuốc với MES ID này." });
            }

            var prescriptionDetails = _prescriptionDetailService.GetAll()
                .Where(x => x.Prescriptionid == prescription.Id)
                .Select(x => new
                {
                    x.Id,
                    x.Medicineid,
                    x.Quantity
                }).ToList();

            var medicines = _medicineService.GetAll()
                .Where(x => prescriptionDetails.Any(d => d.Medicineid == x.Id))
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.Price,
                    x.Unit
                }).ToList();

            return Json(new { success = true, prescriptionDetails, medicines});
        }

        [HttpGet]
        public JsonResult GetTechSheetInformationByMesId(string mesId)
        {
            if (string.IsNullOrEmpty(mesId))
            {
                return Json(new { success = false, message = "MES ID không hợp lệ." });
            }

            var techSheet = _techSheetService.GetByMesId(mesId);
            if (techSheet == null)
            {
                return Json(new { success = false, message = "Không tìm thấy phiếu kỹ thuật với MES ID này." });
            }
            var techDetails = _techDetailService.GetAll()
                .Where(x => x.TechsheetId == techSheet.Id)
                .Select(x => new
                {
                    x.Id,
                    x.Techpositionid,
                    x.Quantity
                }).ToList();

            var techWorks = _techWorkService.GetAll()
                .Where(x => techDetails.Any(d => d.Techpositionid == x.Id))
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.Price,
                    x.Unit,
                    x.Techniquename
                }).ToList();

            return Json(new { success = true, techDetails, techWorks });
        }

        [HttpGet]
        public IActionResult Add()
        {
            ViewBag.MedicalExams = _medicalExamService.GetAll();
            ViewBag.Prescriptions = _prescriptionService.GetAll();
            ViewBag.PrescriptionDetails = _prescriptionDetailService.GetAll();
            ViewBag.Medicines = _medicineService.GetAll();
            ViewBag.TechSheets = _techSheetService.GetAll();
            ViewBag.TechWorks = _techWorkService.GetAll();
            ViewBag.TechDetails = _techDetailService.GetAll();
            return View();
        }

        [HttpPost]
        public IActionResult Add(Bill model)
        {
            if (_invoiceService.GetAll().Any(x => x.Medicalexaminationid == model.Medicalexaminationid))
            {
                TempData["ErrorMessage"] = "Medical Examination Slip has already been created with a invoice!";
                return RedirectToAction("Add");
            }

            var receptionistId = HttpContext.Session.GetString("UserId");
            model.Id = _invoiceService.GenerateID();
            model.Date = DateTime.Today;
            model.Receptionistid = receptionistId;

            if (model.Payment.Equals("Cash"))
            {
                _invoiceService.Add(model);
                return RedirectToAction("Index");
            }

            var mes = _medicalExamService.Get(model.Medicalexaminationid);
            var customer = _customerService.Get(mes.Customerid);

            return RedirectToAction();
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            return View();
        }

        public IActionResult Delete(string id)
        {
            return View("Index");
        }

        public IActionResult Detail()
        {
            return View();
        }
    }
}
