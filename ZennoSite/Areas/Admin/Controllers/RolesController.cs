using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ZennoSite.Areas.Admin.Models;

namespace ZennoSite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class RolesController : Controller
    {
        private readonly RoleManager<ApplicationRole> _roleManager;

        public RolesController(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(_roleManager.Roles);
        }
      
        [HttpPost]
        public async Task<IActionResult> Create(string name, string displayName)
        {
            if (!string.IsNullOrWhiteSpace(name))
            {
                var result = await _roleManager.CreateAsync(new ApplicationRole
                {
                    Name = name.ToLower(), 
                    DisplayName = displayName
                });
                if (!result.Succeeded)
                {
                    AddErrorsToModelState(result);
                }
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) RedirectToAction("Index");
            
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) RedirectToAction("Index");
            
            return View(role);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(string id, string name, string displayName)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(id)) RedirectToAction("Index");
            
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) RedirectToAction("Index");
            role.Name = name;
            role.DisplayName = displayName;
            var result = await _roleManager.UpdateAsync(role);
            if (!result.Succeeded)
            {
                AddErrorsToModelState(result);
            }
            
            return RedirectToAction("Index");
        }
        
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return RedirectToAction("Index");
            
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null) return RedirectToAction("Index");
            
            var result = await _roleManager.DeleteAsync(role);
            if (!result.Succeeded)
            {
                AddErrorsToModelState(result);
            }

            return RedirectToAction("Index");
        }

        private void AddErrorsToModelState(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }
    }
}