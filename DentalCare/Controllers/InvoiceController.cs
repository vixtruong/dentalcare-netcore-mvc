using DentalCare.Models;
using DentalCare.Payments.Momo.Config;
using DentalCare.Payments.Momo.Request;
using DentalCare.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using X.PagedList.Extensions;

namespace DentalCare.Controllers
{
    [Authorize]
    public class InvoiceController : Controller
    {
        private readonly MomoConfig _momoConfig;

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
            TechWorkService techWorkService, TechDetailService techDetailService, MedicineService medicineService, TechSheetService techSheetService, MomoConfig momoConfig)
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
            _momoConfig = momoConfig;
        }

        [Route("invoice/manage")]
        public IActionResult Index(int? page, string sortColumn, string sortDirection, string searchQuery)
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole.Contains("D"))
            {
                return NotFound();
            }

            var invoices = _invoiceService.GetAll();
            ViewBag.Receptionists = _receptionistService.GetAll();
            ViewBag.Customers = _customerService.GetAll();
            ViewBag.MedicalExams = _medicalExamService.GetAll();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                invoices = invoices.Where(a => a.Id.Contains(searchQuery) ||
                                               a.Medicalexaminationid.ToString().Contains(searchQuery) ||
                                               a.Date.ToString("dd-MM-yyyy").Contains(searchQuery) ||
                                               a.Medicalexamination.Doctorid.Contains(searchQuery) ||
                                               a.Medicalexamination.Customerid.Contains(searchQuery) ||
                                               a.Receptionistid.Contains(searchQuery) ||
                                               a.Payment.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)).ToList();

                ViewBag.SearchQuery = searchQuery;
            }

            invoices = (sortColumn switch
            {
                "Date" => sortDirection == "desc" ? invoices.OrderByDescending(a => a.Date) : invoices.OrderBy(a => a.Date),
                "MES" => sortDirection == "desc" ? invoices.OrderByDescending(a => a.Medicalexamination?.Id ?? "") : invoices.OrderBy(a => a.Medicalexamination?.Id ?? ""),
                "Customer ID" => sortDirection == "desc" ? invoices.OrderByDescending(a => a.Medicalexamination.Customer?.Id ?? "") : invoices.OrderBy(a => a.Medicalexamination.Customer?.Id ?? ""),
                "Customer" => sortDirection == "desc" ? invoices.OrderByDescending(a => a.Medicalexamination.Customer?.Name ?? "") : invoices.OrderBy(a => a.Medicalexamination.Customer?.Name ?? ""),
                "ID" => sortDirection == "desc" ? invoices.OrderByDescending(a => a.Id) : invoices.OrderBy(a => a.Id),
                "Total Due" => sortDirection == "desc" ? invoices.OrderByDescending(a => a.Finaltotal) : invoices.OrderBy(a => a.Finaltotal),
                "Payment" => sortDirection == "desc" ? invoices.OrderByDescending(a => a.Payment) : invoices.OrderBy(a => a.Payment),
                _ => invoices.OrderBy(a => a.Id)
            }).ToList();

            ViewBag.SortColumn = sortColumn;
            ViewBag.SortDirection = sortDirection;
            ViewBag.NextSortDirection = sortDirection == "asc" ? "desc" : "asc";

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

            return Json(new { success = true, prescriptionDetails, medicines });
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

        [Route("invoice/add")]
        [HttpGet]
        public IActionResult Add()
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole.Contains("D"))
            {
                return NotFound();
            }

            var invoices = _invoiceService.GetAll();
            var prescriptions = _prescriptionService.GetAll();
            var techSheets = _techSheetService.GetAll();

            ViewBag.MedicalExams = _medicalExamService.GetAll().Where(m => (prescriptions.Any(p => p.Medicalexaminationid == m.Id) 
                                                                           || techSheets.Any(t => t.MedicalexaminationId == m.Id))
                                                                           && !invoices.Any(i => i.Medicalexaminationid == m.Id)).ToList();
            ViewBag.Prescriptions = prescriptions;
            ViewBag.PrescriptionDetails = _prescriptionDetailService.GetAll();
            ViewBag.Medicines = _medicineService.GetAll();
            ViewBag.TechSheets = techSheets;
            ViewBag.TechWorks = _techWorkService.GetAll();
            ViewBag.TechDetails = _techDetailService.GetAll();
            return View();
        }

        [Route("invoice/add")]
        [HttpPost]
        public IActionResult Add(Bill model)
        {
            if (model.Total == 0)
            {
                TempData["ErrorMessage"] = "List medicines and technique are both empty. Can not create invoice!";
                return RedirectToAction("Add");
            }

            if (_invoiceService.GetAll().Any(x => x.Medicalexaminationid == model.Medicalexaminationid))
            {
                TempData["ErrorMessage"] = "Medical Examination Slip has already been created with an invoice!";
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
            else if (model.Payment.Equals("Transfer"))
            {
                var mes = _medicalExamService.Get(model.Medicalexaminationid);

                var paymentRequest = new MomoOneTimePaymentRequest(
                    _momoConfig.PartnerCode,
                    Guid.NewGuid().ToString(),
                    (long)model.Finaltotal,
                    model.Id + DateTime.Now.Ticks,
                 model.Medicalexaminationid,
                    _momoConfig.ReturnUrl,
                    _momoConfig.IpnUrl,
                    "captureWallet",
                model.Discount.ToString());

                paymentRequest.MakeSignature(_momoConfig.AccessKey, _momoConfig.SecretKey);

                var (isSuccess, paymentUrl) = paymentRequest.GetLink(_momoConfig.PaymentUrl);

                if (isSuccess)
                {
                    return Redirect(paymentUrl);
                }
                else
                {
                    TempData["ErrorMessage"] = "Can not create payment link. Please try again.";
                    return RedirectToAction("Add");
                }
            }

            return RedirectToAction("Index");
        }

        [Route("invoice/edit/{id}")]
        [HttpGet]
        public IActionResult Edit(string id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole.Contains("D"))
            {
                return NotFound();
            }

            var invoice = _invoiceService.Get(id);

            return View(invoice);
        }

        [Route("invoice/edit/{id}")]
        [HttpPost]
        public IActionResult Edit(Bill model)
        {
            var invoice = _invoiceService.Get(model.Id);

            invoice.Discount = model.Discount;
            invoice.Finaltotal = invoice.Total * (100 - model.Discount) / 100;

            _invoiceService.Update(invoice);
            return RedirectToAction("Index");
        }

        [Route("invoice/delete/{id}")]
        public IActionResult Delete(string id)
        {
            return View("Index");
        }

        [Route("invoice/detail/{invoiceId}")]
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Detail(string invoiceId)
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole != null && userRole.Contains("D"))
            {
                return NotFound();
            }

            var invoice = _invoiceService.Get(invoiceId);
            if (invoice == null) return NotFound();

            var mes = _medicalExamService.Get(invoice.Medicalexaminationid);
            if (mes == null) return NotFound();

            // Prescription and medicine details
            var prescription = _prescriptionService.GetByMesId(mes.Id);
            var prescriptionDetail = _prescriptionDetailService.GetAll()
                .Where(x => x.Prescriptionid == prescription?.Id)
                .ToList();
            var medicines = _medicineService.GetAll()
                .Where(m => prescriptionDetail.Any(p => p.Medicineid == m.Id))
                .ToList();

            // Technique details
            var techSheet = _techSheetService.GetByMesId(mes.Id);
            var techDetails = _techDetailService.GetAll()
                .Where(x => x.TechsheetId == techSheet?.Id)
                .ToList();
            var techWorks = _techWorkService.GetAll()
                .Where(t => techDetails.Any(td => td.Techpositionid == t.Id))
                .ToList();

            // Customer and receptionist
            var customer = _customerService.Get(mes.Customerid);
            var receptionist = _receptionistService.Get(invoice?.Receptionistid);

            // Create ViewModel
            var model = new InvoiceDetailViewModel
            {
                Invoice = invoice,
                MedicalExam = mes,
                Customer = customer,
                Receptionist = receptionist,
                TechDetails = techDetails,
                TechWorks = techWorks,
                PrescriptionDetails = prescriptionDetail,
                Medicines = medicines
            };
            return View(model);
        }
    }
}
