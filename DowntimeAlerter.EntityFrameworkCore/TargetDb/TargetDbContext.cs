using DowntimeAlerter.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DowntimeAlerter.EntityFrameworkCore.TargetDb
{
    public class TargetDbContext: DbContext
    {
        public DbSet<Target> Target { get; set; }
        public DbSet<HealthCheckResult> HealthCheckResult { get; set; }

        public TargetDbContext(DbContextOptions<TargetDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TargetDbContext).Assembly);
        }
    }
}
