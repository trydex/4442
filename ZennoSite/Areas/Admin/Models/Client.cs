using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ZennoSite.Utils;

namespace ZennoSite.Areas.Admin.Models
{
    public class Client
    {
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Display(Name = "Ник")]
        public string Name { get; set; }

        [Display(Name = "Откуда")]
        public string UtmSource { get; set; } = "skaldchik.com";

        [Display(Name = "Дата создания клиента"), DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime AdditionDate { get; set; } = DateTimeHelper.GetCurrentTime();

        [Display(Name = "Забанен")]
        public bool IsBanned { get; set; }  

        public List<License> Licenses { get; set; }
        public List<Hardware> Hardwares { get; set; }

        public Client() 
        {
            Licenses = new List<License>();
            Hardwares = new List<Hardware>();   
        }
    }
}
