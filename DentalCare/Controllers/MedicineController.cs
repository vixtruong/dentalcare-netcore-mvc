using DentalCare.Models;
using DentalCare.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DentalCare.Controllers
{
    [Authorize]
    public class MedicineController : Controller
    {
        private readonly MedicineService _medicineService;
        private readonly MedicineTypeService _medicineTypeService;

        public MedicineController(MedicineService medicineService, MedicineTypeService medicineTypeService)
        {
            _medicineService = medicineService;
            _medicineTypeService = medicineTypeService;
        }

        public IActionResult Index()
        {
            var medicines = _medicineService.GetAll();
            ViewBag.MedicineTypes = _medicineTypeService.GetAll();
            return View(medicines);
        }

        [HttpGet]
        public IActionResult AddType()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AddType(Medicinetype type)
        {
            Medicinetype newType = new Medicinetype
            {
                Id = _medicineTypeService.GenerateID(),
                Name = type.Name,
            };

            _medicineTypeService.Add(newType);
            return View("Index");
        }

        [HttpGet]
        public IActionResult Add()
        {
            ViewBag.Types = _medicineTypeService.GetAll();
            return View();
        }

        public IActionResult Add(Medicine medicine)
        {
            medicine.Id = _medicineService.GenerateID();
            _medicineService.Add(medicine);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            var medicine = _medicineService.Get(id);
            ViewBag.Types = _medicineTypeService.GetAll();
            return View(medicine);
        }

        [HttpPost]
        public IActionResult Edit(Medicine medicine)
        {
            _medicineService.Update(medicine);
            return RedirectToAction("Index", "Medicine");
        }

        public IActionResult Delete(string id)
        {
            _medicineService.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
