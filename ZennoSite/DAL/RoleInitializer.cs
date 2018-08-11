using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection; 
using ZennoSite.Areas.Admin.Models;

namespace ZennoSite.DAL
{
    public class RoleInitializer
    {
        public static async Task InitializeAsync(IConfiguration config, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            string adminEmail = config.GetSection("Data:AdminLogin").Value;
            string password = config.GetSection("Data:AdminPassword").Value;
            if (await roleManager.FindByNameAsync("admin") == null)
            {
                await roleManager.CreateAsync(new ApplicationRole{Name = "admin", DisplayName = "Администратор"});
            }
            
            if (await userManager.FindByNameAsync(adminEmail) == null)
            {
                var admin = new ApplicationUser { Email = adminEmail, UserName = adminEmail };
                var result = await userManager.CreateAsync(admin, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "admin");
                }
            }
        }
    }
}
