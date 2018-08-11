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
    public class AdminSessionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminSessionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Admin/AdminSessions
        public async Task<IActionResult> Index()
        {
            return View(await _context.AdminSessions.OrderByDescending(s => s.Date).Include(session => session.Address).ToListAsync());
        }

        // GET: Admin/AdminSessions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adminSession = await _context.AdminSessions.Include(session => session.Address)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (adminSession == null)
            {
                return NotFound();
            }

            return View(adminSession);
        }

        // GET: Admin/AdminSessions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/AdminSessions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Date,Login,Password")] AdminSession adminSession)
        {
            if (ModelState.IsValid)
            {
                _context.Add(adminSession);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(adminSession);
        }

        // GET: Admin/AdminSessions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adminSession = await _context.AdminSessions.Include(session => session.Address).SingleOrDefaultAsync(m => m.Id == id);
            if (adminSession == null)
            {
                return NotFound();
            }
            return View(adminSession);
        }

        // POST: Admin/AdminSessions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Date,Login,Password")] AdminSession adminSession)
        {
            if (id != adminSession.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(adminSession);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdminSessionExists(adminSession.Id))
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
            return View(adminSession);
        }

        // GET: Admin/AdminSessions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var adminSession = await _context.AdminSessions.Include(session => session.Address)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (adminSession == null)
            {
                return NotFound();
            }

            return View(adminSession);
        }

        // POST: Admin/AdminSessions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var adminSession = await _context.AdminSessions.SingleOrDefaultAsync(m => m.Id == id);
            _context.AdminSessions.Remove(adminSession);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdminSessionExists(int id)
        {
            return _context.AdminSessions.Any(e => e.Id == id);
        }
    }
}
