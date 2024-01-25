using BHYT_BE.Internal.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace BHYT_BE.Internal.Repository.Data
{
    public class InsuranceRequestDBContext : DbContext
    {
        public InsuranceRequestDBContext(DbContextOptions<InsuranceRequestDBContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.UseIdentityColumns();
        }
        public DbSet<InsuranceRequest> InsuranceRequests { get; set; }
        public override int SaveChanges()
        {
            AddTimestamps();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            AddTimestamps();
            return base.SaveChangesAsync();
        }

        private void AddTimestamps()
        {
            var entities = ChangeTracker.Entries()
                .Where(x => x.Entity is BaseEntity && (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entity in entities)
            {
                var now = DateTime.UtcNow; // current datetime

                if (entity.State == EntityState.Added)
                {
                    ((BaseEntity)entity.Entity).CreatedAt = now;
                }
                ((BaseEntity)entity.Entity).UpdatedAt = now;
            }
        }
    } 
}
