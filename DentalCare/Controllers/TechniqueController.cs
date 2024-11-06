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
        public IActionResult Index(int? page)
        {
            var pageNumber = (page ?? 1);
            var pageSize = 10;
            var techWorks = _techWorkService.GetAll();
            var pagedList = techWorks.ToPagedList(pageNumber, pageSize);
            ViewBag.Techs = _techniqueService.GetAll();
            return View(pagedList);
        }

        [HttpGet]
        public IActionResult AddTech()
        {
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
            _techWorkService.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
