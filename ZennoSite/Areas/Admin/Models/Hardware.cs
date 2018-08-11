using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ZennoSite.Areas.Admin.Models
{
    public class Hardware
    {
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Display(Name = "Значение")]
        public string Value { get; set; }

        [Display(Name = "Заблокировано")]
        public bool IsBanned { get; set; }

        [Display(Name = "Id клиента")]
        public int? ClientId { get; set; }  
        
        [Display(Name = "Клиент")]
        public Client Client { get; set; }
        
        public List<LicenseHardware> LicenseHardwares { get; set; }
    
        public Hardware()
        {
            LicenseHardwares = new List<LicenseHardware>();
        }
    }
}    