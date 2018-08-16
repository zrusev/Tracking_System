﻿namespace Metrics_Track.Web.Areas.Admin.Models.Statuses
{
    using Metrics_Track.Services.Models.Status;
    using System.Collections.Generic;

    public class StatusListViewModel
    {
        public IEnumerable<StatusModel> StatusList { get; set; } = new List<StatusModel>();
    }
}
