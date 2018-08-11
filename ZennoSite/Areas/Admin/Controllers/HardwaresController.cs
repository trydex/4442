using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ZennoSite.Areas.Admin.Models;
using ZennoSite.DAL;

namespace ZennoSite.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "admin")]
    public class HardwaresController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HardwaresController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/Hardwares
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Hardwares.Include(h => h.Client);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Admin/Hardwares/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hardware = await _context.Hardwares
                .Include(h => h.Client)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (hardware == null)
            {
                return NotFound();
            }

            return View(hardware);
        }

        // GET: Admin/Hardwares/Create
        public IActionResult Create()
        {
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Id");
            return View();
        }

        // POST: Admin/Hardwares/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Value,IsBanned,ClientId")] Hardware hardware)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hardware);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Id", hardware.ClientId);
            return View(hardware);
        }

        // GET: Admin/Hardwares/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hardware = await _context.Hardwares.SingleOrDefaultAsync(m => m.Id == id);
            if (hardware == null)
            {
                return NotFound();
            }
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Id", hardware.ClientId);
            return View(hardware);
        }

        // POST: Admin/Hardwares/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Value,IsBanned,ClientId")] Hardware hardware)
        {
            if (id != hardware.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hardware);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!HardwareExists(hardware.Id))
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
            ViewData["ClientId"] = new SelectList(_context.Clients, "Id", "Id", hardware.ClientId);
            return View(hardware);
        }

        // GET: Admin/Hardwares/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var hardware = await _context.Hardwares
                .Include(h => h.Client)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (hardware == null)
            {
                return NotFound();
            }

            return View(hardware);
        }

        // POST: Admin/Hardwares/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hardware = await _context.Hardwares.SingleOrDefaultAsync(m => m.Id == id);
            _context.Hardwares.Remove(hardware);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool HardwareExists(int id)
        {
            return _context.Hardwares.Any(e => e.Id == id);
        }
    }
}
