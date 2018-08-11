using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace ZennoSite.Areas.Admin.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Nickname { get; set; }
        
    }
}        