﻿namespace Metrics_Track.Web.Areas.Admin.Controllers
{
    using Infrastructure.Extensions;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Models.Statuses;
    using Services.Contracts;
    using Services.Models.Status;
    using X.PagedList;

    [Area(WebConstants.AdminArea)]
    [Authorize(Roles = WebConstants.AdministratorRole)]
    public class StatusesController : Controller
    {
        private readonly IStatus status;

        public StatusesController(IStatus status)
        {
            this.status = status;
        }

        [HttpGet]
        public IActionResult Index(int? page)
        {
            var pageNumber = page ?? 1;

            var statuses = this.status.All();

            var onePageList = statuses.ToPagedList(pageNumber, WebConstants.MaxItemsPerPage);

            return View(new StatusListViewModel { StatusList = onePageList });
        }

        [HttpGet]
        public IActionResult ById(string statusId)
        {
            var status = this.status.ById(int.Parse(statusId));

            return View(new StatusViewModel { Status = status });
        }

        [HttpPost]
        public IActionResult ById(StatusViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, WebConstants.InvalidStatusDetails);
                return View(model);
            }

            var status = this.status.ById(model.Status.IdStatus);
            var statusExists = status != null;

            if (!statusExists)
            {
                ModelState.AddModelError(string.Empty, WebConstants.InvalidStatusDetails);
                return View(model);
            }

            var successId = this.status.UpdateStatus(new StatusModel
            {
                IdStatus = model.Status.IdStatus,
                Status = model.Status.Status
            });

            TempData.AddSuccessMessage($"Status: {model.Status.Status} with ID: {successId} has been updated successfully.");

            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public IActionResult AddStatus() => View(new AddStatusViewModel());

        [HttpPost]
        public IActionResult AddStatus(AddStatusViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var newId = this.status.AddStatus(new StatusModel
            {
                Status = model.Status
            });

            TempData.AddSuccessMessage($"Status: {model.Status} with ID: {newId} has been added successfully.");
            return RedirectToAction(nameof(Index));
        }

        public IActionResult RemoveStatus(int id)
        {
            if (id == 0)
            {
                TempData.AddErrorMessage(WebConstants.InvalidStatusId);
                return RedirectToAction(nameof(Index));
            }

            this.status.RemoveStatus(id);

            TempData.AddSuccessMessage($"Status with ID: {id} has been removed successfully.");
            return RedirectToAction(nameof(Index));
        }
    }
}