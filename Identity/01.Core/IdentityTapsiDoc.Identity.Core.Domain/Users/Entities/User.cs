﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityTapsiDoc.Identity.Core.Domain.Users.Entities
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [MaxLength(50)]
        public string? TapsiUserId { get; set; }
    }
}
