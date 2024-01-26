using BHYT_BE.Internal.Repositories.UserRepo;
using User = BHYT_BE.Internal.Models.User;
using Microsoft.AspNetCore.Identity;
using AutoMapper;
using Microsoft.EntityFrameworkCore;


namespace BHYT_BE.Internal.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserRepository _userRepo;
        private readonly ILogger<UserService> _logger;
        private readonly IMapper _mapper;
        public UserService(IMapper mapper, UserManager<User> userManager, IUserRepository userRepo, ILogger<UserService> logger)
        {
            _userManager = userManager;
            _userRepo = userRepo;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<List<UserDTO>> GetAllUsersAsync()
        {
            var users = await _userRepo.GetAll();

            List<UserDTO> userDTOs = new List<UserDTO>();

            foreach (var user in users)
            {
                var userDTO = _mapper.Map<UserDTO>(user);
                var rolesList = await _userManager.GetRolesAsync(user);
                userDTO.Roles = rolesList.Cast<string>().ToList();
                userDTOs.Add(userDTO);
            }

            return userDTOs;
        }

        public UserDTO GetById(string id)
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
                    UserName = user.UserName, 
                    Password = user.PasswordHash,
                    Roles = (List<string>)_userManager.GetRolesAsync(user).Result
                };
                return userDTO;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while getting user by id");
                throw;
            }
        }
        public async Task<IdentityResult> CreateUser(UserDTO req, string otp)
        {
            var user = new User
            {
                UserName = req.UserName,
                Address = req.Address,
                FullName = req.FullName,
                DOB = req.DOB,
                Email = req.Email,
                Nation = req.Nation,
                Nationality = req.Nationality,
                PersonID = req.PersonID,
                PhoneNumber = req.PhoneNumber,
                Sex = req.Sex,
                OTP = otp,
            };
            var createUserResult = await _userManager.CreateAsync(user, req.Password); 
            if (createUserResult != null && !createUserResult.Succeeded)
            {
                return createUserResult;

            }
            var currentUser = await _userManager.FindByEmailAsync(req.Email);
            var roleresult = await _userManager.AddToRolesAsync(currentUser, req.Roles);
            return roleresult;
        }
        public async Task<UserDTO> GetUserByEmail(string email)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    _logger.LogInformation($"User not found with email: {email}");
                    return null;
                }
                return new UserDTO
                {

                };
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
                var user = _userManager.FindByEmailAsync(email).Result;
                if (user == null)
                {
                    _logger.LogInformation($"User not found with email: {email}");
                    return null;
                }

                bool passwordMatch = BCrypt.Net.BCrypt.Verify(passwordHash, user.PasswordHash);
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
