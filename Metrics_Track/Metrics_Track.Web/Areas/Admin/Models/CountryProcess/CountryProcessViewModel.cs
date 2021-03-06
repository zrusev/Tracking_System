﻿namespace Metrics_Track.Web.Areas.Admin.Models.CountryProcess
{
    using Microsoft.AspNetCore.Mvc.Rendering;
    using System.Collections.Generic;

    public class CountryProcessViewModel
    {
        public int IdCountry { get; set; }

        public IEnumerable<SelectListItem> CountryList { get; set; } = new List<SelectListItem>();

        public int[] IdProcesses { get; set; }

        public IEnumerable<SelectListItem> Processes { get; set; }
    }
}
