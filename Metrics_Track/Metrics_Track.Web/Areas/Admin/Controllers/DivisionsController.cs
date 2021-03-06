﻿namespace Metrics_Track.Web.Areas.Admin.Controllers
{
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models.Divisions;
    using Services.Contracts;
    using Services.Models.Division;
    using X.PagedList;

    [Area(WebConstants.AdminArea)]
    [Authorize(Roles = WebConstants.AdministratorRole)]
    public class DivisionsController : Controller
    {
        private readonly IDivision division;

        public DivisionsController(IDivision division)
        {
            this.division = division;
        }

        [HttpGet]
        public IActionResult Index(int? page)
        {
            var pageNumber = page ?? 1;

            var divisions = this.division.All();

            var onePageList = divisions.ToPagedList(pageNumber, WebConstants.MaxItemsPerPage);

            return View(new DivisionListViewModel { DivisionList = onePageList });
        }

        [HttpGet]
        public IActionResult ById(string divisionId)
        {
            var division = this.division.ById(int.Parse(divisionId));

            return View(new DivisionViewModel { Division = division });
        }

        [HttpPost]
        public IActionResult ById(DivisionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, WebConstants.InvalidDivisionDetails);
                return View(model);
            }

            var division = this.division.ById(model.Division.IdDivision);
            var divisionExists = division != null;

            if (!divisionExists)
            {
                ModelState.AddModelError(string.Empty, WebConstants.InvalidDivisionDetails);
                return View(model);
            }

            var successId = this.division.UpdateDivision(new DivisionModel
            {
                IdDivision = model.Division.IdDivision,
                Division = model.Division.Division
            });

            TempData.AddSuccessMessage($"Division: {model.Division.Division} with ID: {successId} has been updated successfully.");

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult AddDivision() => View(new AddDivisionViewModel());

        [HttpPost]
        public IActionResult AddDivision(AddDivisionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var newId = this.division.AddDivision(new DivisionModel
            {
                Division = model.Division
            });

            TempData.AddSuccessMessage($"Division: {model.Division} with ID: {newId} has been added successfully.");
            return RedirectToAction(nameof(Index));
        }

        public IActionResult RemoveDivision(int id)
        {
            if (id == 0)
            {
                TempData.AddErrorMessage(WebConstants.InvalidDivisionId);
                return RedirectToAction(nameof(Index));
            }

            this.division.RemoveDivision(id);

            TempData.AddSuccessMessage($"Division with ID: {id} has been removed successfully.");
            return RedirectToAction(nameof(Index));
        }
    }
}