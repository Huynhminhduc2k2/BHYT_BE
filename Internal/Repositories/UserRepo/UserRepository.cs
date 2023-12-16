using BHYT_BE.Internal.Repositories.Data;
using Microsoft.EntityFrameworkCore;
using User = BHYT_BE.Internal.Models.User;
namespace BHYT_BE.Internal.Repositories.UserRepo
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDBContext _context;
        public UserRepository(UserDBContext context)
        {
            _context = context;
        }

        public void Create (User user)
        {
            _context.Users.Add (user);
            _context.SaveChanges();
        }

        public async Task<List<User>> GetAll()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task <User> GetById(ulong id)
        {
            if (id == 0)
            { return null; }
            return await _context.Users.FindAsync(id);
        }

        public async Task<User> UpdateAsync(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return await Task.FromResult(user);
        }

    }
}
