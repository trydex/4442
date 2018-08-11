using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ZennoSite.Areas.Admin.Models;
using ZennoSite.Areas.Admin.ViewModels;
using ZennoSite.DAL;
using ZennoSite.Utils;

namespace ZennoSite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class LicensesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LicensesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Licenses  
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Licenses.Include(l => l.Client).Include(l => l.Product);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/Licenses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var license = await _context.Licenses
                .Include(l => l.Client)
                .Include(l => l.Product)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (license == null)
            {
                return NotFound();
            }

            return View(license);
        }

        // GET: Admin/Licenses/Create
        public IActionResult Create()
        {
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Name");
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Title");
            return View();
        }

        // POST: Admin/Licenses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Key,Price,HardwaresLimit,IsBanned,CreationDate,ActivationDate,ExpirationDate,LastUse,ProductId,ClientId")] License license)
        {
            if (ModelState.IsValid)
            {
                _context.Add(license);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Name");
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Title", license.ProductId);
            return View(license);
        }

        // GET: Admin/Licenses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var license = await _context.Licenses.SingleOrDefaultAsync(m => m.Id == id);
            if (license == null)
            {
                return NotFound();
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Name");
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Title", license.ProductId);
            return View(license);
        }

        // POST: Admin/Licenses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Key,Price,HardwaresLimit,IsBanned,CreationDate,ActivationDate,ExpirationDate,LastUse,ProductId,ClientId")] License license)
        {
            if (id != license.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(license);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LicenseExists(license.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Name");
            ViewData["ProductId"] = new SelectList(_context.Products, "Id", "Title", license.ProductId);
            return View(license);
        }

        // GET: Admin/Licenses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var license = await _context.Licenses
                .Include(l => l.Client)
                .Include(l => l.Product)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (license == null)
            {
                return NotFound();
            }

            return View(license);
        }

        // POST: Admin/Licenses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var license = await _context.Licenses.SingleOrDefaultAsync(m => m.Id == id);
            _context.Licenses.Remove(license);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LicenseExists(int id)
        {
            return _context.Licenses.Any(e => e.Id == id);
        }


        public IActionResult IssueLicense()
        {
            ViewData["Users"] = new MultiSelectList(_context.Clients, "Id", "Name");
            ViewData["Products"] = new SelectList(_context.Products, "Id", "Title");

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> IssueLicense(IssueLicenseViewModel vm)
        {
            if (vm.ProductId == null)
            {
                ModelState.AddModelError("ProductId", "Не выбран продукт");
            }

            if (vm.SelectedUsers == null)
            {   
                ModelState.AddModelError("SelectedUsers", "Не выбраны пользователи");
            }

            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == vm.ProductId);
            if (product == null)
            {
                ModelState.AddModelError("ProductId", "Выбранный продукт не найден");
            }

            if (ModelState.IsValid)
            {
                foreach (int clientId in vm.SelectedUsers)
                {
                    var client = await _context.Clients.FirstOrDefaultAsync(c => c.Id == clientId);
                    if (client != null )
                    {
                        await CreateLicenseForUser(clientId);
                    }
                }

                return RedirectToAction("Index");
            }

            ViewData["Users"] = new MultiSelectList(_context.Clients, "Id", "Name");
            ViewData["Products"] = new SelectList(_context.Products, "Id", "Title");

            return View(vm);

            async Task CreateLicenseForUser(int clientId)
            {
                var license = new License
                {
                    ProductId = (int) vm.ProductId,
                    ClientId = clientId,
                    ExpirationDate = vm.ExpirationDate,
                    IsBanned = vm.IsBanned,
                    Price = vm.Price,
                    HardwaresLimit = vm.HardwaresLimit,
                    Key = GetKey()
                };

                await _context.Licenses.AddAsync(license);
                await _context.SaveChangesAsync();
            }
        }

        private string GetKey()
        {
            while (true)
            {
                string key = SerialKey.Generate().ToUpper();
                if (!_context.Licenses.Any(l => l.Key == key))
                {
                    return key;
                }
            }
        }
    }
}
