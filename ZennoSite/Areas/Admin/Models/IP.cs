using System.ComponentModel.DataAnnotations;

namespace ZennoSite.Areas.Admin.Models
{
    public class IP
    {
        [Display(Name = "Id")]
        public int Id { get; set; }

        [Display(Name = "Адрес")]
        public string Address { get; set; }

        [Display(Name = "Общее количество запросов")]
        public int RequestCount { get; set; }

        [Display(Name = "Заблокирован")]
        public bool IsBanned { get; set; }

        [Display(Name = "Попыток входа в админку")]
        public int AuthorizationCount { get; set; }
    }
}