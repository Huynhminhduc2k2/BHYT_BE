using BHYT_BE.Internal.Repositories.UserRepo;
using System.Net.WebSockets;
using User = BHYT_BE.Internal.Models.User;
using BHYT_BE.Internal.Services.UserService; // Thêm namespace này

namespace BHYT_BE.Internal.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly ILogger<UserService> _logger;

        public UserService(IUserRepository userRepo, ILogger<UserService> logger)
        {
            _userRepo = userRepo;
            _logger = logger;
        }

        public List<UserDTO> GetAllUsers()
        {
            throw new NotImplementedException();
        }

        public UserDTO GetById(int id)
        {
            try
            {
                var user = _userRepo.GetById(id);
                if (user == null)
                {
                    _logger.LogInformation("User not found");
                    return null;
                }
                UserDTO userDTO = new UserDTO
                {
                    Username = user.Username, 
                    Password = user.Password,
                    Roles = new string[] { } 
                };
                return userDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting user by id");
                throw;
            }
        }


        public void Create(User user)
        {
            throw new NotImplementedException();
        }

        public void AddUser(User user)
        {
            try
            {
                // Save the user to the repository.
                _userRepo.Create(new User
                {
                    Username = user.Username,
                    Password = user.Password
                });
                _logger.LogInformation("User created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating user");
                throw;
            }
        }

        public User GetByEmail(string email)
        {
            try
            {
                var user = _userRepo.GetByEmail(email);
                if (user == null)
                {
                    _logger.LogInformation($"User not found with email: {email}");
                    return null;
                }
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while getting user by email: {email}");
                throw;
            }
        }

        public User LoginUser(string email, string passwordHash)
        {
            try
            {
                var user = GetByEmail(email);
                if (user == null)
                {
                    _logger.LogInformation($"User not found with email: {email}");
                    return null;
                }

                bool passwordMatch = BCrypt.Net.BCrypt.Verify(passwordHash, user.Password);
                if (!passwordMatch)
                {
                    _logger.LogInformation($"Invalid password for user: {email}");
                    return null;
                }

                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while attempting login for user: {email}");
                throw;
            }
        }

        public Task<User> UpdateAsync(User user)
        {
            throw new NotImplementedException();
        }

        public User Update(User user)
        {
            return _userRepo.Update(user);
        }
    }
}
