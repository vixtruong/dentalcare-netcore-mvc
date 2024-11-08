﻿using DentalCare.Models;
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
        public IActionResult Index(int? page)
        {
            var pageNumber = (page ?? 1);
            var pageSize = 10;
            var equipments = _equipmentService.GetAll();
            var pagedList = equipments.ToPagedList(pageNumber, pageSize);
            ViewBag.Types = _equipmentTypeService.GetAll();
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
            _equipmentService.Delete(id);
            return RedirectToAction("Index");
        }
    }
}
