using Microsoft.EntityFrameworkCore;
using BHYT_BE.Internal.Models;

namespace BHYT_BE.Internal.Repositories.Data
{
    public class UserDBContext : DbContext
    {
        public UserDBContext(DbContextOptions<UserDBContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.UseIdentityColumns();
        }
        public DbSet<User> Users { get; set; }
    }
}

