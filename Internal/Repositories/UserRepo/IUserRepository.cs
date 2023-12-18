using User = BHYT_BE.Internal.Models.User;

namespace BHYT_BE.Internal.Repositories.UserRepo
{
    public interface IUserRepository 
    {
        Task<List<User>> GetAll(); 
        User GetById(int id);
        void Create(User user); 
        Task<User> UpdateAsync(User user);
    }
}
