using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ZennoSite.Areas.Admin.Models;

namespace ZennoSite.DAL
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Client> Clients { get; set; }
        public DbSet<License> Licenses { get; set; }
        public DbSet<Hardware> Hardwares { get; set; }
        public DbSet<LicenseHardware> LicenseHardwares { get; set; }
        public DbSet<LicenseSession> LicenseSessions { get; set; }
        public DbSet<IP> Ips { get; set; }
        public DbSet<AdminSession> AdminSessions { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<LicenseSession>().HasOne(session => session.License).WithMany(license => license.Sessions).OnDelete(DeleteBehavior.SetNull);
            builder.Entity<Hardware>().HasOne(hw => hw.Client).WithMany(client => client.Hardwares).OnDelete(DeleteBehavior.SetNull);


        }
    }
}    