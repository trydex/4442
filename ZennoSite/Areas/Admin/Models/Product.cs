using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ZennoSite.Areas.Admin.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required, Display(Name = "Название")]
        public string Title { get; set; }

        [Display(Name = "Описание")]
        public string Description { get; set; }

        [Required]
        [Display(Name = "Цена"), DataType(DataType.Currency)]
        public decimal Price { get; set; }

        [Display(Name = "Продается")]
        public bool CanBuy { get; set; } = true;

        public List<License> Licenses { get; set; }
    }
}