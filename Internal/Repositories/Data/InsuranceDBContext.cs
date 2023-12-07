using BHYT_BE.Internal.Models;
using Microsoft.EntityFrameworkCore;

namespace BHYT_BE.Internal.Repository.Data
{
    public class InsuranceDBContext : DbContext
    {
        public InsuranceDBContext(DbContextOptions<InsuranceDBContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.UseIdentityColumns();
        }
        public DbSet<Insurance> Insurances { get; set; }

    }
}
