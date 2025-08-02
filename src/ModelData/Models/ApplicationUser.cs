using ModelData.Models;using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModelData.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public DateTime? LastLoginAt { get; set; }

    }
}
