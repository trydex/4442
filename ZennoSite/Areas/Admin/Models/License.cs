using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ZennoSite.Utils;

namespace ZennoSite.Areas.Admin.Models
{
    public class License
    {
        [Display(Name = "Id")]
        public int Id { get; set; } 

        [Display(Name = "Ключ")]
        public string Key { get; set; }

        [Display(Name = "Цена"), DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Display(Name = "Лимит железа на ключ")]
        public int HardwaresLimit { get; set; } = 1;

        [Display(Name = "Заблокирована")] 
        public bool IsBanned { get; set; }

        [Display(Name = "Дата создания"), DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        public DateTime CreationDate { get; set; } = DateTimeHelper.GetCurrentTime();

        [Display(Name = "Дата активации"), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        public DateTime? ActivationDate { get; set; }

        [Display(Name = "Дата истечения"), DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        public DateTime? ExpirationDate { get; set; }

        [Display(Name = "Дата последнего использования"), DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}")]
        public DateTime? LastUse { get; set; }

        public int ProductId { get; set; }  

        [Display(Name = "Продукт")]
        public Product Product { get; set; }

        [Display(Name = "Id клиента")]
        public int ClientId { get; set; }   

        [Display(Name = "Клиент")]  
        public Client Client { get; set; }  

        public List<LicenseHardware> LicenseHardwares { get; set; }

        public List<LicenseSession> Sessions { get; set; }

        public License()
        {
            LicenseHardwares = new List<LicenseHardware>();
            Sessions = new List<LicenseSession>();
        }
    }    
}    