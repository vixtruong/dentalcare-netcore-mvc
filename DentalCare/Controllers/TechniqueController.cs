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
        public async Task<IActionResult> Add(Techposition tech, IFormFile techImg)
        {
            try
            {
                if (techImg != null && techImg.Length > 0)
                {
                    var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/technique-imgs");
                    var fileName = Path.GetFileName(techImg.FileName);
                    var filePath = Path.Combine(uploads, fileName);

                    Directory.CreateDirectory(uploads);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await techImg.CopyToAsync(fileStream);
                    }

                    tech.Image = $"/uploads/technique-imgs/{fileName}";
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", "File upload failed");
            }

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
        public async Task<IActionResult> Edit(Techposition tech, IFormFile techImg)
        {
            try
            {
                if (techImg != null && techImg.Length > 0)
                {
                    var uploads = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/technique-imgs");
                    var fileName = Path.GetFileName(techImg.FileName);
                    var filePath = Path.Combine(uploads, fileName);

                    Directory.CreateDirectory(uploads);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await techImg.CopyToAsync(fileStream);
                    }

                    tech.Image = $"/uploads/technique-imgs/{fileName}";
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", "File upload failed");
            }

            var oldTech = _techWorkService.Get(tech.Id);

            var techniqueName = _techniqueService.Get(tech.Techniqueid).Name;

            oldTech.Name = tech.Name;
            oldTech.Unit = tech.Unit;
            oldTech.Price = tech.Price;
            oldTech.Detail = tech.Detail;
            oldTech.Techniqueid = tech.Techniqueid;
            oldTech.Techniquename = techniqueName;

            if (tech.Image != null)
            {
                oldTech.Image = tech.Image;
            }

            _techWorkService.Update(oldTech);
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
