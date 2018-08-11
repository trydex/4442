using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using ZennoSite.Utils;

namespace ZennoSite.Areas.Admin.Models
{
    public class AdminSession
    {
        public int Id { get; set; }

        [Display(Name = "Дата авторизации"), DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; } = DateTimeHelper.GetCurrentTime();

        [Display(Name = "IP")]
        public IP Address { get; set; }

        [Display(Name = "Логин")]
        public string Login { get; set; }

        [Display(Name = "Пароль")]
        public string Password { get; set; }
    }
}
