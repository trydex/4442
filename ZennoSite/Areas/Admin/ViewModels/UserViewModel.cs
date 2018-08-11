using System.Collections.Generic;
using ZennoSite.Areas.Admin.Models;

namespace ZennoSite.Areas.Admin.ViewModels
{
    public class UserViewModel
    {
        public ApplicationUser User { get; set; }
        public IEnumerable<string> UserRoles { get; set; }
    }
}