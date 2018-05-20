﻿namespace Metrics_Track.Data.Models
{
    using Metrics_Track.Data.Constants;
    using Microsoft.AspNetCore.Identity;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    public class User : IdentityUser
    {
        [Required]
        [MinLength(DataConstants.UserNameMinLength)]
        [MaxLength(DataConstants.UserNameMaxLength)]
        public string FirstName { get; set; }

        [Required]
        [MinLength(DataConstants.UserNameMinLength)]
        [MaxLength(DataConstants.UserNameMaxLength)]
        public string LastName { get; set; }

        public List<trel_AgentCountry> Countries { get; set; } = new List<trel_AgentCountry>();

        public List<trel_AgentMining> Minings { get; set; } = new List<trel_AgentMining>();
    }
}