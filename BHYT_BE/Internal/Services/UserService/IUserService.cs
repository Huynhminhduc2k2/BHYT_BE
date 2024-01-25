using BHYT_BE.Controllers.Types;
using BHYT_BE.Internal.Models;
using Microsoft.AspNetCore.Identity;

namespace BHYT_BE.Internal.Services.UserService
{
    public interface IUserService 
    {

        Task<List<UserDTO>> GetAllUsersAsync();
        UserDTO GetById(string id);
        Task<IdentityResult> CreateUser(UserDTO req, string otp);
        Task<User> UpdateAsync(User user);
        User LoginUser(string email, string passwordHash);
        Task<UserDTO> GetUserByEmail(string email);

        User Update(User user);

    }
}
