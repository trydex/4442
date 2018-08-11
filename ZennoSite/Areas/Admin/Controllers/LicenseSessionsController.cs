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
    public class LicenseSessionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LicenseSessionsController(ApplicationDbContext context)
        {   
            _context = context;
        }

        // GET: Admin/LicenseSessions
        public async Task<IActionResult> Index(int count = 50, string nickname = null, string license = null, string product = null, string ip = null, string hardware = null)
        {
            IQueryable<LicenseSession> licenseSessions = _context.LicenseSessions.Include(l => l.Address).Include(l => l.Hardware)
            
                .Include(l => l.License).ThenInclude(l => l.Product).Include(l => l.License).ThenInclude(l => l.Client).OrderByDescending(ls => ls.Date);
            if (nickname != null)
            {
                licenseSessions = licenseSessions.Where(l =>
                    l.License.Client.Name.Equals(nickname.Trim(), StringComparison.OrdinalIgnoreCase));
            }

            if (license != null)
            {
                licenseSessions = licenseSessions.Where(ls => ls.License.Key == license.Trim());
            }

            if (product != null)
            {
                licenseSessions = licenseSessions.Where(ls => ls.License.Product.Title == product.Trim());
            }

            if (ip != null)
            {   
                licenseSessions = licenseSessions.Where(ls => ls.Address.Address == ip.Trim());
            }

            if (hardware != null)
            {
                licenseSessions = licenseSessions.Where(ls => ls.Hardware.Value == hardware.Trim());
            }

            var sessions = await licenseSessions.Take(count).ToListAsync();
            return View(sessions);
        }

        // GET: Admin/LicenseSessions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var licenseSession = await _context.LicenseSessions.Include(l => l.Address).Include(l => l.Hardware)
                .Include(l => l.License).SingleOrDefaultAsync(m => m.Id == id);
            if (licenseSession == null)
            {
                return NotFound();
            }

            return View(licenseSession);
        }


        // GET: Admin/LicenseSessions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var licenseSession = await _context.LicenseSessions.Include(l => l.Address).Include(l => l.Hardware)
                .Include(l => l.License).SingleOrDefaultAsync(m => m.Id == id);
            if (licenseSession == null)
            {
                return NotFound();
            }

            return View(licenseSession);
        }

        // POST: Admin/LicenseSessions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var licenseSession = await _context.LicenseSessions.SingleOrDefaultAsync(m => m.Id == id);
            _context.LicenseSessions.Remove(licenseSession);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}