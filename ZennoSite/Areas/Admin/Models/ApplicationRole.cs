using Microsoft.AspNetCore.Identity;

namespace ZennoSite.Areas.Admin.Models
{
    public class ApplicationRole : IdentityRole
    {
        public string DisplayName { get; set; }
    }
}