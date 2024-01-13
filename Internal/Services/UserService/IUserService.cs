using BHYT_BE.Controllers.Types;
using BHYT_BE.Internal.Models;

namespace BHYT_BE.Internal.Services.UserService
{
    public interface IUserService 
    {

        List<UserDTO> GetAllUsers();

        UserDTO GetById(int id);
        void Create(User user);
        Task<User> UpdateAsync(User user);
        void AddUser(User user);
        User LoginUser(string email, string passwordHash);
        User GetByEmail(string email);

        User Update(User user);

    }
}
