using BHYT_BE.Controllers.Types;
using BHYT_BE.Internal.Models;

namespace BHYT_BE.Internal.Services.UserService
{
    public interface IUserService 
    {

        List<UserDTO> GetAllUsers();

        Task<User> GetById(ulong id);
        void Create(User user);
        Task<User> UpdateAsync(User user);
        void AddUser(User user);
        User LoginUser(string email, string passwordHash);
    }
}
