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
    public class IPsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public IPsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/IPs
        public async Task<IActionResult> Index()
        {
            return View(await _context.Ips.ToListAsync());
        }

        // GET: Admin/IPs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var iP = await _context.Ips
                .SingleOrDefaultAsync(m => m.Id == id);
            if (iP == null)
            {
                return NotFound();
            }

            return View(iP);
        }

        // GET: Admin/IPs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/IPs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Address,RequestCount,IsBanned")] IP iP)
        {
            if (ModelState.IsValid)
            {
                _context.Add(iP);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(iP);
        }

        // GET: Admin/IPs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var iP = await _context.Ips.SingleOrDefaultAsync(m => m.Id == id);
            if (iP == null)
            {
                return NotFound();
            }
            return View(iP);
        }

        // POST: Admin/IPs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Address,RequestCount,IsBanned")] IP iP)
        {
            if (id != iP.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(iP);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!IPExists(iP.Id))
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
            return View(iP);
        }

        // GET: Admin/IPs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var iP = await _context.Ips
                .SingleOrDefaultAsync(m => m.Id == id);
            if (iP == null)
            {
                return NotFound();
            }

            return View(iP);
        }

        // POST: Admin/IPs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var iP = await _context.Ips.SingleOrDefaultAsync(m => m.Id == id);
            _context.Ips.Remove(iP);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool IPExists(int id)
        {
            return _context.Ips.Any(e => e.Id == id);
        }
    }
}
