﻿namespace Metrics_Track.Data.Models
{
    using System;

    public class SSC_View_CurrentStatus
    {
        public int ID_Login { get; set; }

        public string DisplayName { get; set; }

        public string TeamLead { get; set; }

        public string Type { get; set; }

        public DateTime LastUpdate { get; set; }

        public string Comment { get; set; }

        public short Sandbox { get; set; }
    }
}
