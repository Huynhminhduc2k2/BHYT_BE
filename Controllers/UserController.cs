using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BHYT_BE.Internal.Models;
using BHYT_BE.Internal.Services.UserService; // Thêm namespace này nếu IUserService ở trong namespace này
using Microsoft.Extensions.Caching.Memory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using BHYT_BE.Internal.Adapter;
using BHYT_BE.Helper;
using BHYT_BE.Controllers.Types;
using AutoMapper;
using System.Net;
using System.Web;
using BHYT_BE.Common.AppSetting;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace BHYT_BE.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    { 
        private readonly AppSettings _appSettings;
        private readonly IUserService _service;
        private readonly IMemoryCache _memoryCache;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IEmailAdapter _emailAdapter;
        private readonly ILogger<UserController> _logger;
        private readonly IMapper _mapper;


        public UserController(AppSettings appSetting, IMapper mapper, IEmailAdapter emailApdater,IUserService userService, IMemoryCache memoryCache, UserManager<User> userManager, RoleManager<IdentityRole> roleManager, ILogger<UserController> logger)
        {
            _appSettings = appSetting;
            _emailAdapter = emailApdater;
            _service = userService;
            _memoryCache = memoryCache;
            _userManager = userManager;
            _roleManager = roleManager;
            _logger = logger;
            _mapper = mapper;
        }

        // API để lấy danh sách tất cả users
        [HttpGet]
        [Authorize(Roles = Role.ADMIN)]
        public async Task<IActionResult> GetAll()
        {
            var userDTOs = await _service.GetAllUsersAsync();
            var users = _mapper.Map<List<UserInfoResponse>>(userDTOs);
            return Ok(users);
        }

        // API để lấy thông tin chi tiết của một user
        [HttpGet("{id}")]
        [Authorize]
        public IActionResult GetById(Guid id)
        {
            var user = _userManager.Users.FirstOrDefault(x => x.Id == id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] Register req)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    // Log each model state error
                    var errList = new List<string>();
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        _logger.LogError($"ModelState error: {error.ErrorMessage}");
                        errList.Add(error.ErrorMessage);
                    }
                    return BadRequest($"Invalid request body, err={errList.ToList()}");
                }
                if (!ConstantHelper.AreValidValues<Role>(req.Roles))
                {
                    _logger.LogError("Invalid role");
                    return BadRequest("Invalid role");
                }
                if (!ConstantHelper.IsValidValue<Sex>(req.Sex))
                {
                    _logger.LogError("Invalid sex");
                    return BadRequest("Invalid sex");
                }
                if (!req.Password.Equals(req.RePassword)) { 
                    _logger.LogError("Password and RePassword are not the same");
                    return BadRequest("Password and RePassword are not the same"); 
                }
                Random random = new Random();
                string otp = random.Next(100000, 999999).ToString();
                var result = await _service.CreateUser(new UserDTO
                {
                    UserName = req.Email,
                    Password = req.Password,
                    Address = req.Address,
                    FullName = req.FullName,
                    DOB = req.DOB,
                    Email = req.Email,
                    Nation = req.Nation,
                    Nationality = req.Nationality,
                    PersonID = req.PersonID,
                    PhoneNumber = req.PhoneNumber,
                    Sex = req.Sex,
                    Roles = req.Roles,
                }, otp); ;

                if (result.Succeeded)
                {
                    await _emailAdapter.SendEmailAsync(req.Email, "Verification OTP", $"Your OTP is: {otp}", false);
                    _memoryCache.Set(req.Email, otp);
                    return Ok("User was created successfully");
                }
                return BadRequest(error: $"Failed to create user, err = {result.Errors.ToList()}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
        {
            try
            {
                // Tìm người dùng theo tên đăng nhập
                var user = await _userManager.FindByNameAsync(loginRequest.Username);

                if (user != null && await _userManager.CheckPasswordAsync(user, loginRequest.Password))
                {
                    // Đăng nhập thành công

                    // Tạo token JWT
                    var roles = await _userManager.GetRolesAsync(user);
                    var token = GenerateJwtToken(user, roles.ToList());
                        
                    // Gửi token về client
                    var response = new LoginResponse
                    {
                        token = token
                    };

                    return Ok(response);
                }

                // Đăng nhập không thành công
                return Unauthorized("Invalid username or password");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        private string GenerateJwtToken(User user, List<string> roles)
        {
            // Tạo Claims
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email),
            };
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Tạo key từ Secret
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Jwt.Secret));

            // Tạo SigningCredentials
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Tạo token
            var token = new JwtSecurityToken(
                issuer: _appSettings.Jwt.Issuer,
                audience: _appSettings.Jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddDays(Convert.ToDouble(_appSettings.Jwt.ExpireDays)),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        [Authorize]
        [HttpGet("GetCurrentUser")]
        public async Task<IActionResult> GetCurrentUser()
        {
            try
            {
                // Lấy thông tin người dùng từ Claims
                var userId = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                _logger.LogInformation(userId);
                if (userId == null)
                {
                    _logger.LogError("user id is null");
                    return NotFound("user id is empty");
                }

                // Tìm người dùng theo ID
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    _logger.LogError("User not found");
                    return NotFound("User not found");
                }

                // Trả về thông tin người dùng
                return Ok(new
                {
                    user.UserName,
                    user.Email,
                    user.FullName,
                    // Thêm các thông tin khác của người dùng nếu cần
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        // API để cập nhật thông tin của một user
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UserDTO userDTO)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            try
            {
                user.UserName = userDTO.UserName;

                if (!string.IsNullOrEmpty(userDTO.Password))
                {
                    var passwordHash = BCrypt.Net.BCrypt.HashPassword(userDTO.Password);
                    user.PasswordHash = passwordHash;
                }

                var updateResult = await _userManager.UpdateAsync(user);

                if (updateResult.Succeeded)
                {
                    if (userDTO.Roles != null && userDTO.Roles.Any())
                    {
                        await _userManager.AddToRolesAsync(user, userDTO.Roles);
                    }

                    return Ok("User was updated successfully");
                }

                return BadRequest("Failed to update user");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // API để xóa một user
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return NotFound();
            }
try
            {
                if (user.Id == HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value)
                {
                    return BadRequest("You cannot delete yourself.");

                }

                await _userManager.RemoveFromRolesAsync(user, await _userManager.GetRolesAsync(user));

                var deleteResult = await _userManager.DeleteAsync(user);

                if (deleteResult.Succeeded)
                {
                    return Ok("User was deleted successfully");
                }

                return BadRequest("Failed to delete user");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPost("verifyOTP")]
        [AllowAnonymous]
        public ActionResult<string> VerifyOTP([FromBody] VerifyOTPRequest request)
        {
            // Thực hiện xác minh mã OTP tại đây
            // Bạn có thể kiểm tra mã OTP có hợp lệ không và thực hiện các hành động cần thiết

            string expectedOTP = null;
            if (_memoryCache.TryGetValue<string>(request.Email, out string cachedOTP))
            {
                expectedOTP = cachedOTP;
            }

            User client = GetUserByEmail(request.Email);

            System.Diagnostics.Debug.WriteLine(expectedOTP);
            System.Diagnostics.Debug.WriteLine(client.UserName);
            if (request.OTP == expectedOTP && request.Email == client.UserName)
            {
                client.OTP = expectedOTP;
                _service.Update(client);
                return Ok("OTP verified successfully.");
            }
            else
            {
                return BadRequest("Invalid OTP.");
            }
        }

        private User GetUserByEmail(string Email)
        {
            try
            {
                // Gọi phương thức dịch vụ để lấy người dùng theo email
                User user = _userManager.FindByEmailAsync(Email).Result;

                if (user == null)
                {
                    System.Diagnostics.Debug.WriteLine($"User with email {Email} not found.");

                    throw new Exception("User not found.");
                }

                System.Diagnostics.Debug.WriteLine($"User with email {Email} found.");
                return user;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet("Confirm")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string token, string userId)
        {
            if (token == null || token == null)
            {
                return BadRequest("The link is Invalid or Expired");
            }
            //Find the User By Id
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                _logger.LogError($"The User ID {userId} is Invalid");
                return NotFound("User not found");
            }
            //Call the ConfirmEmailAsync Method which will mark the Email as Confirmed
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (result.Succeeded)
            {
                return Ok("Thank you for confirming your email");
            }
            return StatusCode(StatusCodes.Status500InternalServerError, "Confirm account failed");
        }

        [HttpPost("Logout")]
        [Authorize]
        public IActionResult Logout()
        {
            // Invalidate the current JWT token
            var token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Jwt.Secret);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false,
                ClockSkew = TimeSpan.Zero
            };

            try
            {
                tokenHandler.ValidateToken(token, tokenValidationParameters, out _);
            }
            catch (Exception)
            {
                // Token validation failed, the token is already invalidated
                return BadRequest("Token is already invalidated");
            }

            // You can add additional logic here, such as blacklisting the token, storing it in a database, etc.

            return Ok("Logout successful");
        }
    }
}
