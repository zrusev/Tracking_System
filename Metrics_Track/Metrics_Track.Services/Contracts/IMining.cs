﻿namespace Metrics_Track.Services.Contracts
{
    using Models.Mining;
    using Models.User;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    public interface IMining
    {
        IEnumerable<MiningModel> ById(int id);
        void AddUserActivity(int id, string type, DateTime stamp, string commment, short sandbox);
        Task<UserDetailsModel> UserDetailsAsync();
    }
}
