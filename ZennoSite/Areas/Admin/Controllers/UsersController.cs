using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Extensions.Internal;
using ZennoSite.Areas.Admin.Models;
using ZennoSite.Areas.Admin.ViewModels;
using ZennoSite.DAL;

namespace ZennoSite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<ApplicationRole> _roleManager;

        public UsersController(ApplicationDbContext dbContext, UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }
            
        public async Task<IActionResult> Index()
        {
            var vms = new List<UserViewModel>();
            foreach (var user in _userManager.Users)    
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var vm = new UserViewModel
                {
                    User = user,
                    UserRoles = userRoles
                };
                
                vms.Add(vm);
            }    
            return View(vms);
        }

        [HttpGet]
        public async Task<IActionResult> Create(CreateUserViewModel vm)
        {
            vm = new CreateUserViewModel();
            vm.AllRoles = await _roleManager.Roles.ToListAsync();
            return View(vm);
        }    

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserViewModel vm)
        {
            if (ModelState.IsValid)
            {
                var newUser = new ApplicationUser()
                {
                    Nickname = vm.Nickname,
                    Email = vm.Login,
                    UserName = vm.Login,
                };
                
                var result = await _userManager.CreateAsync(newUser, vm.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRolesAsync(newUser, vm.Roles);
                    return RedirectToAction("Index");
                }
            }

            return RedirectToAction("Create", vm);
        }


        public async Task<IActionResult> Delete(string id)
        {
            if (!string.IsNullOrWhiteSpace(id))
            {
                var user = await _userManager.FindByIdAsync(id);
                if (user != null)
                {
                    await _userManager.DeleteAsync(user);
                }
            }
            
            return RedirectToAction("Index");
        }


        public async Task<IActionResult> Edit(EditUserViewModel vm)
        {
            var user = await _userManager.FindByIdAsync(vm.Id);
            if (user != null)
            {
                vm.Login = user.UserName;
                vm.Nickname = user.Nickname;
                vm.AllRoles = await _roleManager.Roles.ToListAsync();
                vm.Roles = new List<string>(await _userManager.GetRolesAsync(user));
            }

            return View(vm);
        }
        
        [HttpPost]    
        public async Task<IActionResult> EditUser(EditUserViewModel vm)
        {
            // получаем пользователя
            var user = await _userManager.FindByIdAsync(vm.Id);
            if(user != null)
            {
                user.UserName = vm.Login;
                user.Nickname = vm.Nickname;
                user.Email = vm.Login;
                await _userManager.UpdateAsync(user);
                
                // получем список ролей пользователя
                var userRoles = await _userManager.GetRolesAsync(user);
                // получаем все роли
                var allRoles = _roleManager.Roles.ToList();
                // получаем список ролей, которые были добавлены
                var addedRoles = vm.Roles.Except(userRoles);
                // получаем роли, которые были удалены
                var removedRoles = userRoles.Except(vm.Roles);
 
                await _userManager.AddToRolesAsync(user, addedRoles);
 
                await _userManager.RemoveFromRolesAsync(user, removedRoles);
 
                return RedirectToAction("Index");
            }
 
            return NotFound();
        }

        public async Task<IActionResult> ChangePassword(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ChangePasswordViewModel model = new ChangePasswordViewModel { Id = user.Id, Email = user.Email };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByIdAsync(model.Id);
                if (user != null)
                {
                    var _passwordValidator =
                        HttpContext.RequestServices.GetService(typeof(IPasswordValidator<ApplicationUser>)) as IPasswordValidator<ApplicationUser>;
                    var _passwordHasher =
                        HttpContext.RequestServices.GetService(typeof(IPasswordHasher<ApplicationUser>)) as IPasswordHasher<ApplicationUser>;

                    IdentityResult result =
                        await _passwordValidator.ValidateAsync(_userManager, user, model.NewPassword);
                    if (result.Succeeded)
                    {
                        user.PasswordHash = _passwordHasher.HashPassword(user, model.NewPassword);
                        await _userManager.UpdateAsync(user);
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Пользователь не найден");
                }
            }
            return View(model);
        }
    }
}