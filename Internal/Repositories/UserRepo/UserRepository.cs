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

        public void Create(User user)
        {
            _context.Users.Add(user);
            _context.SaveChanges();
        }

        public async Task<List<User>> GetAll()
        {
            return await _context.Users.ToListAsync();
        }

        public User GetByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return null;
            }

            // Tìm kiếm user dựa trên email bằng EF Core
            var user = _context.Users.FirstOrDefault(u => u.Username == email.ToLower());

            // Trả về user tìm thấy hoặc null nếu không tìm thấy
            return user;
        }

        public User GetById(int id)
        {
            if (id == 0)
            {
                return null;
            }
            var user = _context.Users.Find(id);
            return user;
        }

        public async Task<User> UpdateAsync(User user)
        {
            _context.Entry(user).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return await Task.FromResult(user);
        }

        public User GetUserByEmail(string email)
        {
            return _context.Users.FirstOrDefault(u => u.Username == email);
        }

        public User Update(User user)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Updating user with new OTP: {user.OTP}");

                _context.Entry(user).State = EntityState.Modified;
                _context.SaveChanges();

                // Reload the user from the database to get the updated state
                _context.Entry(user).Reload();

                return user;
            }
            catch (Exception ex)
            {
                // Handle the exception or log it
                Console.WriteLine($"Error updating user: {ex.Message}");
                throw; // Rethrow the exception if needed
            }
        }
    }
}
