using DentalCare.Models;
using DentalCare.Services;
using Microsoft.AspNetCore.Mvc;
using X.PagedList.Extensions;

namespace DentalCare.Controllers
{
    public class PrescriptionController : Controller
    {
        private readonly DoctorService _doctorService;
        private readonly PrescriptionService _prescriptionService;
        private readonly PrescriptionDetailService _prescriptionDetailService;
        private readonly CustomerService _customerService;
        private readonly MedicalExamService _medicalExamService;
        private readonly MedicineTypeService _medicineTypeService;
        private readonly MedicineService _medicineService;

        public PrescriptionController(DoctorService doctorService, PrescriptionService prescriptionService,
            PrescriptionDetailService prescriptionDetailService, CustomerService customerService,
            MedicalExamService medicalExamService, MedicineTypeService medicineTypeService, MedicineService medicineService)
        {
            _doctorService = doctorService;
            _prescriptionService = prescriptionService;
            _customerService = customerService;
            _prescriptionDetailService = prescriptionDetailService;
            _medicalExamService = medicalExamService;
            _medicineTypeService = medicineTypeService;
            _medicineService = medicineService;
        }

        public IActionResult Index(int? page)
        {
            var pageSize = 10;
            var pageNumber = (page ?? 1);
            var prescriptions = _prescriptionService.GetAll();
            var pagedList = prescriptions.ToPagedList(pageNumber, pageSize);

            ViewBag.MedicalExams = _medicalExamService.GetAll();
            ViewBag.Doctors = _doctorService.GetAll();
            ViewBag.Customers = _customerService.GetAll();

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
            ViewBag.MedicalExams = _medicalExamService.GetAll();
            ViewBag.Types = _medicineTypeService.GetAll();
            ViewBag.Medicines = _medicineService.GetAll();
            return View();
        }

        [HttpPost]
        public IActionResult Add(PrescriptionViewModel model)
        {
            var mes = _medicalExamService.Get(model.MedicalExamId);

            var prescription = new Prescription
            {
                Id = _prescriptionService.GenerateID(),
                Date = model.Date,
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
            ViewBag.MedicalExams = _medicalExamService.GetAll();
            ViewBag.Types = _medicineTypeService.GetAll();
            ViewBag.Medicines = _medicineService.GetAll();

            var prescription = _prescriptionService.Get(id);
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

        //public IActionResult Edit(PrescriptionViewModel model)
        //{
        //    var prescription = _prescriptionService.Get(model.Id);
        //    prescription.Date = model.Date;
        //    prescription.Medicalexaminationid = model.MedicalExamId;
        //    _prescriptionService.Update(prescription);

        //    var detailList = new List<Prescriptiondetail>();

        //    foreach (var detail in model.Details)
        //    {
        //        var medicine = _medicineService.Get(detail.MedicineId);

        //        foreach (var presDetail in _prescriptionDetailService.GetAll())
        //        {
        //            if (presDetail.Prescriptionid == prescription.Id && presDetail.Medicineid == medicine.Id)
        //            {
        //                medicine.Quantity += short.Parse(detail.Quantity);
        //            }
        //        }

        //        var newDetail = new Prescriptiondetail
        //        {
        //            Medicineid = detail.MedicineId,
        //            Quantity = short.Parse(detail.Quantity),
        //            Prescriptionid = prescription.Id,
        //        };

        //        medicine.Quantity -= newDetail.Quantity;

        //        _medicineService.Update(medicine);
        //        detailList.Add(newDetail);
        //    }

        //    _prescriptionDetailService.DeleteByPrescriptionId(prescription.Id);
        //    _prescriptionDetailService.AddRage(detailList);

        //    return RedirectToAction("Index");
        //}

        [HttpPost]
        public IActionResult Edit(PrescriptionViewModel model)
        {
            var prescription = _prescriptionService.Get(model.Id);
            prescription.Date = model.Date;
            prescription.Medicalexaminationid = model.MedicalExamId;
            _prescriptionService.Update(prescription);

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
            foreach (var detail in _prescriptionDetailService.GetAll())
            {
                if (id == detail.Prescriptionid)
                {
                    TempData["ErrorMessage"] = "This prescription relate to your business database! Can not delete";
                    return RedirectToAction("Index");
                }
            }

            _prescriptionService.Delete(id);

            return RedirectToAction("Index");
        }
    }
}
