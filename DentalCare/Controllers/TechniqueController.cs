using DentalCare.Models;
using DentalCare.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        public IActionResult Index()
        {
            var techWorks = _techWorkService.GetAll();
            ViewBag.Techs = _techniqueService.GetAll();
            return View(techWorks);
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
            tech.Id = _techWorkService.GenerateID();
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

        public IActionResult Edit(Techposition tech)
        {
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
