using BHYT_BE.Internal.Repositories.UserRepo;
using User = BHYT_BE.Internal.Models.User;

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

        public Task<User> GetById(ulong id)
        {
            try
            {
                return _userRepo.GetById(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting user by id");
                throw;
            }
        }

        /*public void AddUser(User user)
        {
            try
            {
                // Implement logic to create and persist the user based on the provided User object.
                // This might involve generating an ID if necessary, using repositories, and handling potential errors.

                // Generate a unique ID for the user.
                ulong newId = GenerateUserId();

                // Set the ID on the user object.
                user.Id = newId;

                // Save the user to the repository.
                _userRepo.Create(user);
                _logger.LogInformation("User created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while creating user");
                throw;
            }
        }
*/
        public Task<User> UpdateAsync(User user)
        {
            try
            {
                return _userRepo.UpdateAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while updating user");
                throw;
            }
        }

       /* private ulong GenerateUserId()
        {
            // Implement logic to generate a unique ID
            // ...

            return newId;
        }
*/
        public void AddUser(UserDTO req)
        {
            throw new NotImplementedException();
        }

        public void Create(User user)
        {
            throw new NotImplementedException();
        }

        public void AddUser(User user)
        {
            throw new NotImplementedException();
        }

        public User LoginUser(string email, string passwordHash)
        {
            throw new NotImplementedException();
        }
    }
}
