﻿namespace Metrics_Track.Controllers
{
    using Metrics_Track.Services.Contracts;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Authorization;
    using Metrics_Track.Data.Models;

    public class DashboardController : Controller
    {   
        private readonly ICountry countries;
        private readonly IMining mining;
        private readonly ITransaction transaction;
        private readonly UserManager<User> userManager;

        private const int TestLoginID = 145;
        private const string AppVersion = "3.0.0.0";

        public DashboardController(ICountry countries, IMining mining, ITransaction transaction, UserManager<User> userManager)
        {
            this.countries = countries;
            this.mining = mining;
            this.transaction = transaction;
            this.userManager = userManager;
        }

        [HttpGet]
        [Authorize]
        [Route("dashboard")]
        public async Task<IActionResult> Users()
        {
            var user = await userManager.GetUserAsync(User);

            var userId = userManager.GetUserId(User);

            var modelCountries = this.countries.ById(userId);

            var modelMining = this.mining.ById(1);

            var cvm = new CountryViewModel();

            foreach (var model in modelCountries)
            {
                cvm.CountryList.Add(model);
            }

            foreach (var model in modelMining)
            {
                cvm.MiningList.Add(model);
            }
            
            return View(cvm);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> UpdateStatus(string type, string comment)
        {
            int id = TestLoginID;
            string activityType = type;
            DateTime stamp = DateTime.Now;
            string activityCommment = comment;
            short sandbox = await this.mining.GetUserSandboxAsync(id);
            string version = AppVersion;

            this.mining.AddUserActivity(id, activityType, stamp, activityCommment, sandbox, version);

            return Json(new { Status = activityType });
        }

        [HttpPost]
        [Authorize]
        public IActionResult SubmitTransaction(int countryId, int processId, int activityId, int lobId,
                                               DateTime receivedDate, DateTime startDate, DateTime completeDate, int statusId, string comment,
                                               string numberId, string partnerId, string contactId, double premium, string currCode,
                                               string insuredName, string tranRequestor, int originalId, short statusCode, short priority, string attachments,
                                               DateTime inceptionDate, DateTime dateReceived)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, errors = ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage).ToList() });
            }

            var identityId = this.transaction.AddTransaction(TestLoginID, countryId, processId, activityId, lobId, processId, processId, processId, 
                                                            receivedDate, startDate, DateTime.Now, statusId, comment, numberId, partnerId,
                                                            contactId, premium, currCode, insuredName, tranRequestor, originalId, 1, 0, attachments, 
                                                            inceptionDate, dateReceived);

            return Json(new { success = true, newId = identityId });
        }

        public JsonResult GetMining(int id)
        {
            var modelMining = this.mining.ById(id);
            return Json(modelMining);
        }
    }
}