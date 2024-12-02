using DentalCare.Models;
using DentalCare.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using X.PagedList.Extensions;

namespace DentalCare.Controllers
{
    [Authorize]
    public class TechSheetController : Controller
    {
        private readonly DoctorService _doctorService;
        private readonly CustomerService _customerService;
        private readonly TechniqueService _techniqueService;
        private readonly TechWorkService _techWorkService;
        private readonly MedicalExamService _medicalExamService;
        private readonly TechDetailService _techDetailService;
        private readonly TechSheetService _techSheetService;
        private readonly InvoiceService _invoiceService;

        public TechSheetController(DoctorService doctorService, CustomerService customerService, TechniqueService techniqueService, TechWorkService techWorkService, MedicalExamService medicalExamService, TechDetailService techDetailService, TechSheetService techSheetService, InvoiceService invoiceService)
        {
            _doctorService = doctorService;
            _customerService = customerService;
            _techniqueService = techniqueService;
            _techWorkService = techWorkService;
            _medicalExamService = medicalExamService;
            _techDetailService = techDetailService;
            _techSheetService = techSheetService;
            _invoiceService = invoiceService;
        }

        [Route("tech-sheet/manage")]
        public IActionResult Index(int? page, string sortColumn, string sortDirection, string searchQuery)
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole.Contains("R"))
            {
                return NotFound();
            }

            var pageNumber = (page ?? 1);
            var pageSize = 10;

            var userId = HttpContext.Session.GetString("UserId");

            ViewBag.Doctors = _doctorService.GetAll();
            ViewBag.Customers = _customerService.GetAll();
            ViewBag.MedicalExams = _medicalExamService.GetAll();

            var techSheets = _techSheetService.GetAll().Where(x => x.Medicalexamination.Doctorid == userId).ToList();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                techSheets = techSheets.Where(a => a.Id.Contains(searchQuery) ||
                                                   a.MedicalexaminationId.ToString().Contains(searchQuery) ||
                                                   a.Date.ToString("dd-MM-yyyy").Contains(searchQuery) ||
                                                   a.Medicalexamination.Doctorid.Contains(searchQuery) ||
                                                   a.Medicalexamination.Customerid.Contains(searchQuery)).ToList();

                ViewBag.SearchQuery = searchQuery;
            }

            techSheets = (sortColumn switch
            {
                "Date" => sortDirection == "desc" ? techSheets.OrderByDescending(a => a.Date) : techSheets.OrderBy(a => a.Date),
                "MES" => sortDirection == "desc" ? techSheets.OrderByDescending(a => a.Medicalexamination?.Id ?? "") : techSheets.OrderBy(a => a.Medicalexamination?.Id ?? ""),
                "Customer ID" => sortDirection == "desc" ? techSheets.OrderByDescending(a => a.Medicalexamination.Customer?.Id ?? "") : techSheets.OrderBy(a => a.Medicalexamination.Customer?.Id ?? ""),
                "Customer" => sortDirection == "desc" ? techSheets.OrderByDescending(a => a.Medicalexamination.Customer?.Name ?? "") : techSheets.OrderBy(a => a.Medicalexamination.Customer?.Name ?? ""),
                "Doctor" => sortDirection == "desc" ? techSheets.OrderByDescending(a => a.Medicalexamination.Doctor?.Name ?? "") : techSheets.OrderBy(a => a.Medicalexamination.Doctor?.Name ?? ""),
                "Doctor ID" => sortDirection == "desc" ? techSheets.OrderByDescending(a => a.Medicalexamination.Doctor?.Id ?? "") : techSheets.OrderBy(a => a.Medicalexamination.Doctor?.Id ?? ""),
                "ID" => sortDirection == "desc" ? techSheets.OrderByDescending(a => a.Id) : techSheets.OrderBy(a => a.Id),
                _ => techSheets.OrderBy(a => a.Id)
            }).ToList();

            ViewBag.SortColumn = sortColumn;
            ViewBag.SortDirection = sortDirection;
            ViewBag.NextSortDirection = sortDirection == "asc" ? "desc" : "asc";

            var pagedList = techSheets.ToPagedList(pageNumber, pageSize);
            return View(pagedList);
        }

        [HttpGet]
        public JsonResult GetTechworkByType(string id)
        {
            var medicines = _techWorkService.GetByType(id);
            var medicineList = medicines.Select(d => new { id = d.Id, name = d.Name }).ToList();
            return Json(medicineList);
        }

        [Route("tech-sheet/add")]
        [HttpGet]
        public IActionResult Add()
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole.Contains("R"))
            {
                return NotFound();
            }

            var userId = HttpContext.Session.GetString("UserId");

            ViewBag.MedicalExams = _medicalExamService.GetAll().Where(x => x.Doctorid == userId && x.Date == DateTime.Today).ToList();
            ViewBag.Types = _techniqueService.GetAll();
            ViewBag.Medicines = _techWorkService.GetAll();
            return View();
        }

        [Route("tech-sheet/add")]
        [HttpPost]
        public IActionResult Add(TechSheetViewModel model)
        {
            if (_invoiceService.GetAll().Any(x => x.Medicalexaminationid == model.MedicalExamId))
            {
                TempData["ErrorDetailNullMessage"] = "MES has been created invoice before.";
                return RedirectToAction("Add");
            }

            if (model.Details.Count == 0)
            {
                TempData["ErrorDetailNullMessage"] = "List techworks is empty. Please choose techworks to add!";
                return RedirectToAction("Add");
            }

            if (_techSheetService.IsExistMes(model.MedicalExamId))
            {
                TempData["ErrorMessage"] = "A techsheet has already been created for this mes. Please edit if you want to change the techsheet information.";
                return RedirectToAction("Index");
            }

            var techsheet = new Techsheet
            {
                Id = _techSheetService.GenerateID(),
                Date = DateTime.Today,
                MedicalexaminationId = model.MedicalExamId
            };
            _techSheetService.Add(techsheet);

            var techDetailList = new List<Techdetail>();
            foreach (var detail in model.Details)
            {
                var newDetail = new Techdetail
                {
                    Techpositionid = detail.TechworkId,
                    Quantity = short.Parse(detail.Quantity),
                    TechsheetId = techsheet.Id
                };

                techDetailList.Add(newDetail);
            }

            _techDetailService.AddRange(techDetailList);
            return RedirectToAction("Index");
        }

        [Route("tech-sheet/edit/{id}")]
        [HttpGet]
        public IActionResult Edit(string id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole.Contains("R"))
            {
                return NotFound();
            }

            var prescription = _techSheetService.Get(id);
            var invoices = _invoiceService.GetAll();

            if (invoices.Any(x => x.Medicalexaminationid == prescription.MedicalexaminationId))
            {
                TempData["ErrorMessage"] = "This technical sheet has already been created with an invoice. Can not edit.";
                return RedirectToAction("Index");
            }

            ViewBag.Types = _techniqueService.GetAll();
            ViewBag.Medicines = _techWorkService.GetAll();

            
            var details = new List<TechDetailViewModel>();
            foreach (var detail in _techDetailService.GetAll())
            {
                if (detail.TechsheetId == id)
                {
                    details.Add(new TechDetailViewModel { TechworkId = detail.Techpositionid, Quantity = detail.Quantity.ToString() });
                }
            }

            var prescriptionViewModel = new TechSheetViewModel
            {
                Id = prescription.Id,
                MedicalExamId = prescription.MedicalexaminationId,
                Date = prescription.Date,
                Details = details
            };

            return View(prescriptionViewModel);
        }

        [Route("tech-sheet/edit/{id}")]
        [HttpPost]
        public IActionResult Edit(TechSheetViewModel model)
        {
            var techSheet = _techSheetService.Get(model.Id);

            _techDetailService.DeleteRangeByTechsheetId(model.Id);

            var updateList = new List<Techdetail>();
            foreach (var detail in model.Details)
            {
                var updateDetail = new Techdetail
                {
                    TechsheetId = techSheet.Id,
                    Techpositionid = detail.TechworkId,
                    Quantity = short.Parse(detail.Quantity)
                };
                updateList.Add(updateDetail);
            }

            _techDetailService.AddRange(updateList);

            return RedirectToAction("Index");
        }

        public IActionResult Delete(string id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole.Contains("R"))
            {
                return NotFound();
            }

            foreach (var detail in _techDetailService.GetAll())
            {
                if (id == detail.TechsheetId)
                {
                    TempData["ErrorMessage"] = "This techsheet relate to your business database! Can not delete!";
                    return RedirectToAction("Index");
                }
            }

            _techSheetService.Delete(id);

            return RedirectToAction("Index");
        }
    }
}
