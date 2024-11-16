using DentalCare.Models;
using DentalCare.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using X.PagedList.Extensions;

namespace DentalCare.Controllers
{
    [Authorize]
    public class EquipmentController : Controller
    {
        private readonly EquipmentService _equipmentService;
        private readonly EquipmentTypeService _equipmentTypeService;

        public EquipmentController(EquipmentService equipmentService, EquipmentTypeService equipmentTypeService)
        {
            _equipmentService = equipmentService;
            _equipmentTypeService = equipmentTypeService;
        }

        [Route("equipment")]
        public IActionResult Index(string sortColumn, string sortDirection, string searchQuery, int? page = 1)
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole.Contains("D"))
            {
                return NotFound();
            }

            ViewBag.Types = _equipmentTypeService.GetAll();
            var pageNumber = (page ?? 1);
            var pageSize = 10;
            var equipments = _equipmentService.GetAll();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                equipments = equipments.Where(a => a.Id.Contains(searchQuery) ||
                                                   a.Name.ToString().Contains(searchQuery) ||
                                                   a.Equipmenttype.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)).ToList();

                ViewBag.SearchQuery = searchQuery;
            }

            equipments = (sortColumn switch
            {
                "ID" => sortDirection == "desc" ? equipments.OrderByDescending(a => a.Id) : equipments.OrderBy(a => a.Id),
                "Type" => sortDirection == "desc" ? equipments.OrderByDescending(a => a.Equipmenttype?.Name ?? "") : equipments.OrderBy(a => a.Equipmenttype?.Name ?? ""),
                "Name" => sortDirection == "desc" ? equipments.OrderByDescending(a => a.Name) : equipments.OrderBy(a => a.Name),
                "Unit" => sortDirection == "desc" ? equipments.OrderByDescending(a => a.Unit) : equipments.OrderBy(a => a.Unit),
                "Quantity" => sortDirection == "desc" ? equipments.OrderByDescending(a => a.Quantity) : equipments.OrderBy(a => a.Quantity),
                "Price" => sortDirection == "desc" ? equipments.OrderByDescending(a => a.Price) : equipments.OrderBy(a => a.Price),
                _ => equipments.OrderBy(a => a.Id)
            }).ToList();

            ViewBag.SortColumn = sortColumn;
            ViewBag.SortDirection = sortDirection;
            ViewBag.NextSortDirection = sortDirection == "asc" ? "desc" : "asc";

            var pagedList = equipments.ToPagedList(pageNumber, pageSize);
            return View(pagedList);
        }

        [HttpGet]
        public IActionResult GetQuantityForEquipment(string medicineId)
        {
            var quantity = _equipmentService.GetQuantityForEquipment(medicineId);
            return Json(new { quantity });
        }

        [HttpGet]
        public IActionResult AddType()
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole.Contains("D"))
            {
                return NotFound();
            }

            return View();
        }

        [HttpPost]
        public IActionResult AddType(Equipmenttype type)
        {
            type.Id = _equipmentTypeService.GenerateID();
            _equipmentTypeService.Add(type);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Add()
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole.Contains("D"))
            {
                return NotFound();
            }

            ViewBag.Types = _equipmentTypeService.GetAll();
            return View();
        }

        public IActionResult Add(Equipment equipment)
        {
            equipment.Id = _equipmentService.GenerateID();
            _equipmentService.Add(equipment);
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole.Contains("D"))
            {
                return NotFound();
            }

            var equipment = _equipmentService.Get(id);
            ViewBag.Types = _equipmentTypeService.GetAll();
            return View(equipment);
        }

        [HttpPost]
        public IActionResult Edit(Equipment equipment)
        {
            _equipmentService.Update(equipment);
            return RedirectToAction("Index");
        }

        public IActionResult Delete(string id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole.Contains("D"))
            {
                return NotFound();
            }

            _equipmentService.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
