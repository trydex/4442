using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using ZennoSite.Areas.Admin.Models;

namespace ZennoSite.Areas.Admin.ViewModels
{
    public class IssueLicenseViewModel   
    {
        public int? ProductId { get; set; }
        public List<int> SelectedUsers { get; set; }
        [Display(Name = "Пользователи")]
        public MultiSelectList Users { get; set; } 
        [Display(Name = "Продукт")]
        public SelectList AllProducts { get; set; }
        [Display(Name = "Цена")]
        public decimal Price { get; set; }
        [Display(Name = "Лимит железа на ключ")]
        public int HardwaresLimit { get; set; } = 1;
        [Display(Name = "Заблокирована")]
        public bool IsBanned { get; set; }
        [Display(Name = "Дата истечения"), DataType(DataType.Date)]
        public DateTime? ExpirationDate { get; set; }
    }
}                   