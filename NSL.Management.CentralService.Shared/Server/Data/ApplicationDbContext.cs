using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NSL.Database.EntityFramework.Filter.Host;
using NSL.Management.CentralService.Shared.Models;

namespace NSL.Management.CentralService.Shared.Server.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<UserModel>(options)
    {
        public DbSet<ServerModel> Servers { get; set; }

        public DbSet<ServerLogModel> ServerLogs { get; set; }

        public DbSet<ServerMetricsModel> ServerMetrics { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.HasDbFilter();

            builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}
