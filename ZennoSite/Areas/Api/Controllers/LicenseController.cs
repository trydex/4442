using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ZennoFramework.Api.Common.Messages;
using ZennoFramework.Api.Common.Utils;
using ZennoFramework.Generator;
using ZennoSite.Areas.Admin.Models;
using ZennoSite.DAL;
using ZennoSite.Utils;

namespace ZennoSite.Areas.Api.Controllers
{
    //[Produces("application/json")]
    [Route("api/[controller]")]
    public class LicenseController : Controller
    {
        private static DateTime? _lastSessionCleaningDate;
        private readonly ApplicationDbContext _context;

        public LicenseController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> GenerateCode()
        {
            string key;
            string hw;
            RequestMessage requestMessage;
            string requestBody;
            LicenseSession session;
            try
            {
                requestBody = await GetBody();
                session = await SaveSession(StringUtils.Decrypt(requestBody));
                requestMessage = Cryptographer.Decrypt<RequestMessage>(requestBody);
                hw = requestMessage.Hardware;
                key = requestMessage.Key;
            }
            catch (Exception e)
            {
                return Json(new ResponseMessage { ServerError = "Некорректный формат запроса" });
            }

            if (session.Address.IsBanned)
            {
                return Json(new ResponseMessage { ServerError = "IP заблокирован" });
            }

            if (string.IsNullOrWhiteSpace(hw))
            {
                return Json(new ResponseMessage { ServerError = "Не указан идентификатор железа" });
            }

            if (string.IsNullOrWhiteSpace(key))
            {
                return Json(new ResponseMessage { ServerError = "Не указан лицензионный ключ" });
            }

            var license = await _context.Licenses
                .Include(l => l.Client)
                .Include(l => l.LicenseHardwares)
                .ThenInclude(lh => lh.Hardware)
                .ThenInclude(h => h.Client)
                .Include(l => l.Sessions)
                .FirstOrDefaultAsync(l => l.Key.Equals(key, StringComparison.OrdinalIgnoreCase));

            if (license == null)
            {
                return Json(new ResponseMessage { ServerError = "Лицензия не найдена"});
            }

            if (license.IsBanned)
            {
                return Json(new ResponseMessage { ServerError = "Лицензия заблокирована" });
            }

            if (license.ExpirationDate < DateTime.Now)
            {
                return Json(new ResponseMessage { ServerError = "Лицензия истекла" });
            }

            if (license.Client.IsBanned)
            {
                return Json(new ResponseMessage { ServerError = "Пользователь заблокирован" });
            }

            if (license.LicenseHardwares.Count == license.HardwaresLimit && !license.LicenseHardwares.Any(l =>
                    l.Hardware.Value.Equals(hw, StringComparison.OrdinalIgnoreCase)))
            {
                return Json(new ResponseMessage { ServerError = "Неизвестный идентификатор железа"});
            }

            if (license.ActivationDate == null)
            {
                license.ActivationDate = DateTimeHelper.GetCurrentTime();
            }

            var licenseHw = license.LicenseHardwares.FirstOrDefault(lh =>
                lh.Hardware.Value.Equals(hw, StringComparison.OrdinalIgnoreCase));
            if (licenseHw != null)
            {
                if (licenseHw.Hardware.IsBanned)
                {
                    return Json(new ResponseMessage { ServerError = "Идентификатор железа заблокирован"});
                }

                licenseHw.LastUse = DateTimeHelper.GetCurrentTime();
                _context.Entry(licenseHw).State = EntityState.Modified;
            }
            else if(license.LicenseHardwares.Count < license.HardwaresLimit)
            {
                var newHardware = new Hardware {ClientId = license.ClientId, Value = hw};
                licenseHw = new LicenseHardware
                {
                    Hardware = newHardware,
                    License = license,
                    LastUse = DateTimeHelper.GetCurrentTime(),
                    HardwareAttachingDate = DateTimeHelper.GetCurrentTime()
                };
                license.LicenseHardwares.Add(licenseHw);

                await _context.Hardwares.AddAsync(newHardware);
                await _context.LicenseHardwares.AddAsync(licenseHw);
                await _context.SaveChangesAsync();
            }

            session.Hardware = licenseHw.Hardware;
            session.LicenseId = license.Id;
            license.LastUse = DateTimeHelper.GetCurrentTime();

            _context.Entry(session).State = EntityState.Modified;
            _context.Entry(license).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            await RemoveOldSession();


            string code;
            try
            {
                code = CodeGenerator.Generate(requestMessage.GenerationData);
            }
            catch (Exception e)
            {
                return Json(new ResponseMessage { GeneratorError = "ExceptionMessage: " +  e.Message + Environment.NewLine + Environment.NewLine + "StackTrace:" + Environment.NewLine + e.StackTrace});
            }
            
            return Json(new ResponseMessage {Code = code});
        }

        private async Task<LicenseSession> SaveSession(string requestBody)
        {
            var ipString = HttpContext.Connection.RemoteIpAddress.ToString();
            var clientIp =
                await _context.Ips.FirstOrDefaultAsync(ip => ip.Address.Equals(ipString, StringComparison.OrdinalIgnoreCase));
            if (clientIp == null)
            {
                clientIp = new IP {Address = ipString};
                await _context.Ips.AddAsync(clientIp);
                await _context.SaveChangesAsync();
            }

            clientIp.RequestCount++;
            _context.Entry(clientIp).State = EntityState.Modified;

            var session = new LicenseSession {Address = clientIp, Date = DateTimeHelper.GetCurrentTime(), Query = requestBody };

            await _context.AddAsync(session);
            await _context.SaveChangesAsync();

            return session;
        }

        private async Task<string> GetBody()
        {
            using (var reader = new StreamReader(Request.Body, Encoding.UTF8))
            {
                return await reader.ReadToEndAsync();
            }
        }

        private async Task RemoveOldSession()
        {
            if (!_lastSessionCleaningDate.HasValue ||
                DateTime.Now - _lastSessionCleaningDate.Value > TimeSpan.FromDays(1))
            {
                _lastSessionCleaningDate = DateTimeHelper.GetCurrentTime();
                var limitDate = DateTimeHelper.GetCurrentTime().AddDays(-7);
                var oldSessions = await _context.LicenseSessions.Where(s => s.Date < limitDate).ToListAsync();
                _context.RemoveRange(oldSessions);
                await _context.SaveChangesAsync();
            }   
        }
    }
}