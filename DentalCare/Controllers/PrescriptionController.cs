using DentalCare.Models;
using DentalCare.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using X.PagedList.Extensions;

namespace DentalCare.Controllers
{
    [Authorize]
    public class PrescriptionController : Controller
    {
        private readonly DoctorService _doctorService;
        private readonly PrescriptionService _prescriptionService;
        private readonly PrescriptionDetailService _prescriptionDetailService;
        private readonly CustomerService _customerService;
        private readonly MedicalExamService _medicalExamService;
        private readonly MedicineTypeService _medicineTypeService;
        private readonly MedicineService _medicineService;
        private readonly InvoiceService _invoiceService;

        public PrescriptionController(DoctorService doctorService, PrescriptionService prescriptionService,
            PrescriptionDetailService prescriptionDetailService, CustomerService customerService,
            MedicalExamService medicalExamService, MedicineTypeService medicineTypeService, MedicineService medicineService, InvoiceService invoiceService)
        {
            _doctorService = doctorService;
            _prescriptionService = prescriptionService;
            _customerService = customerService;
            _prescriptionDetailService = prescriptionDetailService;
            _medicalExamService = medicalExamService;
            _medicineTypeService = medicineTypeService;
            _medicineService = medicineService;
            _invoiceService = invoiceService;
        }

        public IActionResult Index(int? page, string sortColumn, string sortDirection, string searchQuery)
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole.Contains("R"))
            {
                return NotFound();
            }

            var userId = HttpContext.Session.GetString("UserId");

            var pageSize = 10;
            var pageNumber = (page ?? 1);
            
            ViewBag.MedicalExams = _medicalExamService.GetAll();
            ViewBag.Doctors = _doctorService.GetAll();
            ViewBag.Customers = _customerService.GetAll();

            var prescriptions = _prescriptionService.GetAll().Where(x => x.Medicalexamination.Doctorid == userId).ToList();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                prescriptions = prescriptions.Where(a => a.Id.Contains(searchQuery) ||
                                                         a.Medicalexaminationid.ToString().Contains(searchQuery) ||
                                                         a.Date.ToString("dd-MM-yyyy").Contains(searchQuery) ||
                                                         a.Doctorid.Contains(searchQuery) ||
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
                _ => prescriptions.OrderBy(a => a.Id)
            }).ToList();

            ViewBag.SortColumn = sortColumn;
            ViewBag.SortDirection = sortDirection;
            ViewBag.NextSortDirection = sortDirection == "asc" ? "desc" : "asc";

            var pagedList = prescriptions.ToPagedList(pageNumber, pageSize);

            return View(pagedList);
        }

        [HttpGet]
        public JsonResult GetMedicineByType(string id)
        {
            var medicines = _medicineService.GetByType(id).Where(x => x.Quantity > 0).ToList();
            var medicineList = medicines.Select(d => new { id = d.Id, name = d.Name }).ToList();
            return Json(medicineList);
        }

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
            ViewBag.Types = _medicineTypeService.GetAll();
            ViewBag.Medicines = _medicineService.GetAll();
            return View();
        }

        [HttpPost]
        public IActionResult Add(PrescriptionViewModel model)
        {
            if (_invoiceService.GetAll().Any(x => x.Medicalexaminationid == model.MedicalExamId))
            {
                TempData["ErrorDetailNullMessage"] = "MES has been created invoice before.";
                return RedirectToAction("Add");
            }

            if (model.Details.Count == 0)
            {
                TempData["ErrorDetailNullMessage"] = "List medicines is empty. Please choose medicines to add!";
                return RedirectToAction("Add");
            }

            var mes = _medicalExamService.Get(model.MedicalExamId);
            if (_prescriptionService.IsExistMes(mes.Id))
            {
                TempData["ErrorMessage"] = "A prescription has already been created for this mes. Please edit if you want to change the prescription information.";
                return RedirectToAction("Index");
            }

            var prescription = new Prescription
            {
                Id = _prescriptionService.GenerateID(),
                Date = DateTime.Today,
                Doctorid = mes.Doctorid,
                Medicalexaminationid = mes.Id
            };
            _prescriptionService.Add(prescription);

            var detailList = new List<Prescriptiondetail>();
            foreach (var detail in model.Details)
            {
                var newDetail = new Prescriptiondetail
                {
                    Medicineid = detail.MedicineId,
                    Quantity = short.Parse(detail.Quantity),
                    Prescriptionid = prescription.Id,
                };

                var medicine = _medicineService.Get(detail.MedicineId);
                medicine.Quantity = (short) (medicine.Quantity - newDetail.Quantity);
                _medicineService.Update(medicine);

                detailList.Add(newDetail);
            }

            _prescriptionDetailService.AddRange(detailList);
            
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole.Contains("R"))
            {
                return NotFound();
            }

            var prescription = _prescriptionService.Get(id);
            var invoices = _invoiceService.GetAll();

            if (invoices.Any(x => x.Medicalexaminationid == prescription.Medicalexaminationid))
            {
                TempData["ErrorMessage"] = "This prescription has already been created with an invoice. Can not edit.";
                return RedirectToAction("Index");
            }

            ViewBag.Types = _medicineTypeService.GetAll();
            ViewBag.Medicines = _medicineService.GetAll();

            
            var details = new List<PrescriptionDetailViewModel>();
            foreach (var detail in _prescriptionDetailService.GetAll())
            {
                if (detail.Prescriptionid == id)
                {
                    details.Add(new PrescriptionDetailViewModel {MedicineId = detail.Medicineid, Quantity = detail.Quantity.ToString()});
                }
            }

            var prescriptionViewModel = new PrescriptionViewModel
            {
                Id = prescription.Id,
                MedicalExamId = prescription.Medicalexaminationid,
                Date = prescription.Date,
                Details = details
            };

            return View(prescriptionViewModel);
        }

        [HttpPost]
        public IActionResult Edit(PrescriptionViewModel model)
        {
            var prescription = _prescriptionService.Get(model.Id);

            var detailList = new List<Prescriptiondetail>();
            var updateMedicines = new List<Medicine>();

            var existingDetails = _prescriptionDetailService.GetAll()
                .Where(d => d.Prescriptionid == prescription.Id)
                .ToList();

            foreach (var detail in model.Details)
            {
                var medicine = _medicineService.Get(detail.MedicineId);
                var existingDetail = existingDetails
                    .FirstOrDefault(d => d.Medicineid == detail.MedicineId);

                if (existingDetail != null)
                {
                    medicine.Quantity += existingDetail.Quantity;
                    existingDetail.Quantity = short.Parse(detail.Quantity);
                    var newPresDetail = new Prescriptiondetail
                    {
                        Quantity = existingDetail.Quantity,
                        Prescriptionid = existingDetail.Prescriptionid,
                        Medicineid = existingDetail.Medicineid
                    };
                    detailList.Add(newPresDetail);
                }
                else
                {
                    var newDetail = new Prescriptiondetail
                    {
                        Medicineid = detail.MedicineId,
                        Quantity = short.Parse(detail.Quantity),
                        Prescriptionid = prescription.Id
                    };
                    detailList.Add(newDetail);
                }

                medicine.Quantity -= short.Parse(detail.Quantity);
                updateMedicines.Add(medicine);
            }

            _medicineService.UpdateRange(updateMedicines);
            _prescriptionDetailService.DeleteByPrescriptionId(prescription.Id);
            _prescriptionDetailService.AddRange(detailList);

            return RedirectToAction("Index");
        }

        public IActionResult Delete(string id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole.Contains("R"))
            {
                return NotFound();
            }

            foreach (var detail in _prescriptionDetailService.GetAll())
            {
                if (id == detail.Prescriptionid)
                {
                    TempData["ErrorMessage"] = "This prescription relate to your business database! Can not delete!";
                    return RedirectToAction("Index");
                }
            }

            _prescriptionService.Delete(id);

            return RedirectToAction("Index");
        }
    }
}
