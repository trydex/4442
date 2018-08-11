using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZennoSite.Areas.Admin.Models;
using ZennoSite.Areas.Admin.ViewModels;
using ZennoSite.DAL;
using ZennoSite.ViewModels;

namespace ZennoSite.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AccountController(SignInManager<ApplicationUser> signInManager, ApplicationDbContext context)
        {
            _signInManager = signInManager;
            _context = context;
        }
        
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            var session = await SaveAuthSession(model);
            if (session.Address.AuthorizationCount > 10)
            {
                ModelState.AddModelError("", "Неправильный логин и (или) пароль");
            }

            if (ModelState.IsValid)
            {
                var result =
                    await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    session.Address.AuthorizationCount = 0;
                    _context.Entry(session.Address).State = EntityState.Modified;
                    await _context.SaveChangesAsync();

                    // проверяем, принадлежит ли URL приложению
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    return RedirectToAction("Index", "Home");
                }
                
                ModelState.AddModelError("", "Неправильный логин и (или) пароль");
            }
            return View(model);
        }

        private async Task<AdminSession> SaveAuthSession(LoginViewModel model)
        {
            var session = new AdminSession {Login = model.Email, Password = model.Password,};

            var ipString = HttpContext.Connection.RemoteIpAddress.ToString();
            var dbIp = await _context.Ips.FirstOrDefaultAsync(ip =>
                ip.Address.Equals(ipString, StringComparison.OrdinalIgnoreCase));
            if (dbIp == null)
            {
                dbIp = new IP {Address = ipString};
                await _context.Ips.AddAsync(dbIp);
                await _context.SaveChangesAsync();
            }

            session.Address = dbIp;
            dbIp.AuthorizationCount++;

            _context.Entry(dbIp).State = EntityState.Modified;
            await _context.AddAsync(session);
            await _context.SaveChangesAsync();

            return session;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LogOff()
        {
            // удаляем аутентификационные куки
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}