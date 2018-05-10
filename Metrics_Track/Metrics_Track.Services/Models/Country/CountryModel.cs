﻿namespace Metrics_Track.Services.Models.Country
{
    using Process;
    using System.Collections.Generic;
    public class CountryModel
    {
        public int ID_Country { get; set; }

        public string Country { get; set; }

        public List<ProcessModel> ProcessList { get; set; }

        public CountryModel()
        {
            this.ProcessList = new List<ProcessModel>();
        }
    }
}