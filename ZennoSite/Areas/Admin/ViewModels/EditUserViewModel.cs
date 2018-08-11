using System.Collections.Generic;
using ZennoSite.Areas.Admin.Models;

namespace ZennoSite.Areas.Admin.ViewModels
{
    public class EditUserViewModel
    {
        public string Id { get; set; }
        public string Nickname { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public List<string> Roles { get; set; }
        public List<ApplicationRole> AllRoles { get; set; }
        
        
        public EditUserViewModel()
        {
            Roles = new List<string>();
            AllRoles = new List<ApplicationRole>();
        }
    }
}