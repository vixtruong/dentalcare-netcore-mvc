using DentalCare.Models;
using DentalCare.Services;
using Microsoft.AspNetCore.Mvc;
using X.PagedList.Extensions;

namespace DentalCare.Controllers
{
    public class EquipmentSheetController : Controller
    {
        private readonly DoctorService _doctorService;
        private readonly EquipmentSheetService _equipmentSheetService;
        private readonly EquipmentDetailService _equipmentDetailService;
        private readonly CustomerService _customerService;
        private readonly EquipmentService _equipmentService;
        private readonly EquipmentTypeService _equipmentTypeService;
        private readonly MedicalExamService _medicalExamService;

        public EquipmentSheetController(DoctorService doctorService, EquipmentSheetService equipmentSheetService, EquipmentDetailService equipmentDetailService, 
            CustomerService customerService, EquipmentService equipmentService, EquipmentTypeService equipmentTypeService, MedicalExamService medicalExamService)
        {
           _doctorService = doctorService;
           _equipmentService = equipmentService;
           _customerService = customerService;
           _equipmentTypeService= equipmentTypeService;
           _equipmentSheetService = equipmentSheetService;
           _equipmentDetailService = equipmentDetailService;
           _medicalExamService = medicalExamService;
        }

        public IActionResult Index(int? page, string sortColumn, string sortDirection, string searchQuery)
        {
            var pageSize = 10;
            var pageNumber = (page ?? 1);
            var prescriptions = _equipmentSheetService.GetAll();
            
            ViewBag.MedicalExams = _medicalExamService.GetAll();
            ViewBag.Doctors = _doctorService.GetAll();
            ViewBag.Customers = _customerService.GetAll();

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
                _ => prescriptions.OrderBy(a => a.Id)
            }).ToList();

            ViewBag.SortColumn = sortColumn;
            ViewBag.SortDirection = sortDirection;
            ViewBag.NextSortDirection = sortDirection == "asc" ? "desc" : "asc";

            var pagedList = prescriptions.ToPagedList(pageNumber, pageSize);
            return View(pagedList);
        }

        [HttpGet]
        public JsonResult GetEquipmentByType(string id)
        {
            var medicines = _equipmentService.GetByType(id).Where(x => x.Quantity > 0).ToList();
            var medicineList = medicines.Select(d => new { id = d.Id, name = d.Name }).ToList();
            return Json(medicineList);
        }

        [HttpGet]
        public IActionResult Add()
        {
            ViewBag.MedicalExams = _medicalExamService.GetAll();
            ViewBag.Types = _equipmentTypeService.GetAll();
            ViewBag.Medicines = _equipmentService.GetAll();
            return View();
        }

        [HttpPost]
        public IActionResult Add(EquipmentSheetViewModel model)
        {
            if (model.Details.Count == 0)
            {
                TempData["ErrorDetailNullMessage"] = "List equipments is empty. Please choose equipments to add!";
                return RedirectToAction("Add");
            }

            var mes = _medicalExamService.Get(model.MedicalExamId);
            if (_equipmentSheetService.IsExistMes(mes.Id))
            {
                TempData["ErrorMessage"] = "A equipment sheet has already been created for this mes. Please edit if you want to change the prescription information.";
                return RedirectToAction("Index");
            }

            var equipmentSheet = new EquipmentSheet
            {
                Id = _equipmentSheetService.GenerateID(),
                Date = model.Date,
                MedicalexaminationId = model.MedicalExamId
            };
            _equipmentSheetService.Add(equipmentSheet);

            var detailList = new List<Equipmentdetail>();
            foreach (var detail in model.Details)
            {
                var newDetail = new Equipmentdetail
                {
                    Equipmentid = detail.EquipmentId,
                    Quantity = short.Parse(detail.Quantity),
                    EquipmentsheetId = equipmentSheet.Id
                };

                var equipment = _equipmentService.Get(detail.EquipmentId);
                equipment.Quantity -= newDetail.Quantity;
                _equipmentService.Update(equipment);

                detailList.Add(newDetail);
            }

            _equipmentDetailService.AddRange(detailList);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            ViewBag.MedicalExams = _medicalExamService.GetAll();
            ViewBag.Types = _equipmentTypeService.GetAll();
            ViewBag.Medicines = _equipmentService.GetAll();

            var prescription = _equipmentSheetService.Get(id);
            var details = new List<EquipmentDetailViewModel>();
            foreach (var detail in _equipmentDetailService.GetAll())
            {
                if (detail.EquipmentsheetId == id)
                {
                    details.Add(new EquipmentDetailViewModel { EquipmentId = detail.Equipmentid, Quantity = detail.Quantity.ToString() });
                }
            }

            var prescriptionViewModel = new EquipmentSheetViewModel
            {
                Id = prescription.Id,
                MedicalExamId = prescription.MedicalexaminationId,
                Date = prescription.Date,
                Details = details
            };

            return View(prescriptionViewModel);
        }

        [HttpPost]
        public IActionResult Edit(EquipmentSheetViewModel model)
        {
            var prescription = _equipmentSheetService.Get(model.Id);
            prescription.Date = model.Date;
            prescription.MedicalexaminationId = model.MedicalExamId;
            _equipmentSheetService.Update(prescription);

            var detailList = new List<Equipmentdetail>();
            var updateMedicines = new List<Equipment>();

            var existingDetails = _equipmentDetailService.GetAll()
                .Where(d => d.EquipmentsheetId == prescription.Id)
                .ToList();

            foreach (var detail in model.Details)
            {
                var medicine = _equipmentService.Get(detail.EquipmentId);
                var existingDetail = existingDetails
                    .FirstOrDefault(d => d.Equipmentid == detail.EquipmentId);

                if (existingDetail != null)
                {
                    medicine.Quantity += existingDetail.Quantity;
                    existingDetail.Quantity = short.Parse(detail.Quantity);
                    var newPresDetail = new Equipmentdetail
                    {
                        Quantity = existingDetail.Quantity,
                        EquipmentsheetId = existingDetail.EquipmentsheetId,
                        Equipmentid = existingDetail.Equipmentid
                    };
                    detailList.Add(newPresDetail);
                }
                else
                {
                    var newDetail = new Equipmentdetail
                    {
                        Equipmentid = detail.EquipmentId,
                        Quantity = short.Parse(detail.Quantity),
                        EquipmentsheetId = prescription.Id
                    };
                    detailList.Add(newDetail);
                }

                medicine.Quantity -= short.Parse(detail.Quantity);
                updateMedicines.Add(medicine);
            }

            _equipmentService.UpdateRange(updateMedicines);
            _equipmentDetailService.DeleteByEquipmentSheetId(prescription.Id);
            _equipmentDetailService.AddRange(detailList);

            return RedirectToAction("Index");
        }

        public IActionResult Delete(string id)
        {
            if (_equipmentDetailService.GetAll().Any(x => x.EquipmentsheetId == id))
            {
                TempData["ErrorMessage"] = "This equipment sheet relate to your business database! Can not delete!";
                return RedirectToAction("Index");
            }

            _equipmentSheetService.Delete(id);

            return RedirectToAction("Index");
        }
    }
}