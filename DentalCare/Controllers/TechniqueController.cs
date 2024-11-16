using DentalCare.Models;
using DentalCare.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using X.PagedList.Extensions;

namespace DentalCare.Controllers
{
    [Authorize]
    public class TechniqueController : Controller
    {
        private readonly TechWorkService _techWorkService;
        private readonly TechniqueService _techniqueService;

        public TechniqueController(TechWorkService techWorkService, TechniqueService techniqueService)
        {
            _techWorkService = techWorkService;
            _techniqueService = techniqueService;
        }

        [Route("technique")]
        public IActionResult Index(string sortColumn, string sortDirection, string searchQuery, int? page = 1)
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole.Contains("D"))
            {
                return NotFound();
            }

            ViewBag.Techs = _techniqueService.GetAll();
            var pageNumber = (page ?? 1);
            var pageSize = 10;
            var techWorks = _techWorkService.GetAll();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                techWorks = techWorks.Where(a => a.Id.Contains(searchQuery) ||
                                                 a.Name.ToString().Contains(searchQuery) ||
                                                 a.Technique.Name.Contains(searchQuery, StringComparison.OrdinalIgnoreCase)).ToList();

                ViewBag.SearchQuery = searchQuery;
            }

            techWorks = (sortColumn switch
            {
                "ID" => sortDirection == "desc" ? techWorks.OrderByDescending(a => a.Id) : techWorks.OrderBy(a => a.Id),
                "Type" => sortDirection == "desc" ? techWorks.OrderByDescending(a => a.Technique?.Name ?? "") : techWorks.OrderBy(a => a.Technique?.Name ?? ""),
                "Name" => sortDirection == "desc" ? techWorks.OrderByDescending(a => a.Name) : techWorks.OrderBy(a => a.Name),
                "Unit" => sortDirection == "desc" ? techWorks.OrderByDescending(a => a.Unit) : techWorks.OrderBy(a => a.Unit),
                "Price" => sortDirection == "desc" ? techWorks.OrderByDescending(a => a.Price) : techWorks.OrderBy(a => a.Price),
                _ => techWorks.OrderBy(a => a.Id)
            }).ToList();

            ViewBag.SortColumn = sortColumn;
            ViewBag.SortDirection = sortDirection;
            ViewBag.NextSortDirection = sortDirection == "asc" ? "desc" : "asc";

            var pagedList = techWorks.ToPagedList(pageNumber, pageSize);
            return View(pagedList);
        }

        [HttpGet]
        public IActionResult AddTech()
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole.Contains("D"))
            {
                return NotFound();
            }

            return View();
        }

        [HttpPost]
        public IActionResult AddTech(Technique tech)
        {
            tech.Id = _techniqueService.GenerateID();
            _techniqueService.Add(tech);
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

            ViewBag.Techs = _techniqueService.GetAll();
            return View();
        }

        [HttpPost]
        public IActionResult Add(Techposition tech)
        {
            var techniqueName = _techniqueService.Get(tech.Techniqueid).Name;
            tech.Id = _techWorkService.GenerateID();
            tech.Techniquename = techniqueName;
            _techWorkService.Add(tech);
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

            var tech = _techWorkService.Get(id);
            ViewBag.Techs = _techniqueService.GetAll();
            return View(tech);
        }

        [HttpPost]
        public IActionResult Edit(Techposition tech)
        {
            var techniqueName = _techniqueService.Get(tech.Techniqueid).Name;
            tech.Techniquename = techniqueName;
            _techWorkService.Update(tech);
            return RedirectToAction("Index");
        }

        public IActionResult Delete(string id)
        {
            var userRole = HttpContext.Session.GetString("UserRole");

            if (userRole.Contains("D"))
            {
                return NotFound();
            }

            _techWorkService.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
