using Microsoft.EntityFrameworkCore;
using BHYT_BE.Internal.Models;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.AspNetCore.Identity;
using System.Reflection.Emit;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace BHYT_BE.Internal.Repositories.Data
{
    public class UserDBContext : IdentityDbContext<User>
    {
        public UserDBContext(DbContextOptions<UserDBContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            //builder.UseIdentityColumns();
            /// Cấu hình IdentityUserRole<string>
            base.OnModelCreating(builder);
            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                var tableName = entityType.GetTableName();
                if (tableName.StartsWith("AspNet"))
                {
                    entityType.SetTableName(tableName.Substring(6));
                }
            }
            var roles = new List<IdentityRole>()
            {
                new IdentityRole()
                {
                    Name = "Admin",
                    NormalizedName = "admin".ToUpper()
                },
                new IdentityRole()
                {
                    Name = "user",
                    NormalizedName = "user".ToUpper()
                }
            };
            builder.Entity<IdentityRole>().HasData(roles);
        }
        public DbSet<User> Users { get; set; }
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
                .Where(x => x.Entity is User && (x.State == EntityState.Added || x.State == EntityState.Modified));

            foreach (var entity in entities)
            {
                var now = DateTime.UtcNow; // current datetime

                if (entity.State == EntityState.Added)
                {
                    ((User)entity.Entity).CreatedAt = now;
                }
                ((User)entity.Entity).UpdatedAt = now;
            }
        }
    }
}

