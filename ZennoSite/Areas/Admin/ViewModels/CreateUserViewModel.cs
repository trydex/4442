using System.Collections.Generic;
using ZennoSite.Areas.Admin.Models;

namespace ZennoSite.Areas.Admin.ViewModels
{
    public class CreateUserViewModel
    {
        public string Nickname { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public List<string> Roles { get; set; }
        public List<ApplicationRole> AllRoles { get; set; }
    
        
        public CreateUserViewModel()
        {
            Roles = new List<string>();
            AllRoles = new List<ApplicationRole>();
        }
    }
}