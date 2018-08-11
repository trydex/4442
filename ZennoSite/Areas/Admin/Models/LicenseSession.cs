using System;
using System.ComponentModel.DataAnnotations;

namespace ZennoSite.Areas.Admin.Models
{
    public class LicenseSession 
    {   
        [Display(Name = "Id")]
        public int Id { get; set; } 

        [Display(Name = "Id IP-адреса")]
        public int? AddressId { get; set; }

        [Display(Name = "IP-адрес")]
        public IP Address { get; set; }  

        [Display(Name = "Id железа")]
        public int? HardwareId { get; set; }

        [Display(Name = "Железо")]
        public Hardware Hardware { get; set; }  

        [Display(Name = "Id лицензии")]
        public int? LicenseId { get; set; }

        [Display(Name = "Лицензия")]
        public License License { get; set; }

        [Display(Name = "Запрос")]
        public string Query { get; set; }

        [Display(Name = "Дата"), DataType(DataType.DateTime), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
    }
}   