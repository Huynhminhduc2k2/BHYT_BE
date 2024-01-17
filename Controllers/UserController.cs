using BCrypt.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BHYT_BE.Internal.Models;
using BHYT_BE.Internal.Services.UserService; // Thêm namespace này nếu IUserService ở trong namespace này
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using BHYT_BE.Controllers.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace BHYT_BE.Controllers
{
   /* [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]*/
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;
        private readonly IMemoryCache _memoryCache;
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;


        public UserController(IUserService userService, IMemoryCache memoryCache, IConfiguration configuration, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _service = userService;
            _memoryCache = memoryCache;
            _configuration = configuration;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // API để lấy danh sách tất cả users
        [HttpGet]
        public IActionResult GetAll()
        {
            var users = _userManager.Users.ToList();
            return Ok(users);
        }

        // API để lấy thông tin chi tiết của một user
        [HttpGet("{id}")]
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
        public async Task<IActionResult> Create([FromBody] UserDTO userDTO)
        {
            try
            {
                // Tạo một password hash từ chuỗi plain text
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(userDTO.Password);

                // Tạo một user mới với password hash
                var user = new User
                {
                    UserName = userDTO.Username,
                    PasswordHash = passwordHash,
                    Address = userDTO.Address,
                    FullName = userDTO.FullName,
                    DOB = userDTO.DOB,
                    Email = userDTO.Email,
                    Nation = userDTO.Nation,
                    Nationality = userDTO.Nationality,
                    PersonID = userDTO.PersonID,
                    PhoneNumber = userDTO.PhoneNumber,
                    Sex = userDTO.Sex,
                    OTP = GenerateOTP(),
                };

                var createUserResult = await _userManager.CreateAsync(user, userDTO.Password);

                if (createUserResult.Succeeded)
                {
                    var currentUser = await _userManager.FindByEmailAsync(user.Email);
                    var roleresult = await _userManager.AddToRoleAsync(currentUser, UserRole.USER.ToString().ToLower());
                    if (roleresult.Succeeded)
                    {
                        string otp = GenerateOTP();
                        SendEmail(userDTO.Username, otp);
                        _memoryCache.Set(userDTO.Username, otp);
                        return Ok("User was created successfully");
                    } //await SignInAsync(user, isPersistent: false);
                }
                
                // Xử lý lỗi tạo người dùng không thành công
                // Đây có thể là do yêu cầu mật khẩu không đủ mạnh, email không hợp lệ, v.v.
                // Bạn có thể trả về các thông báo lỗi chi tiết nếu cần.
                return BadRequest("Failed to create user");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }






        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDTO loginRequest)
        {
            try
            {
                // Tìm người dùng theo tên đăng nhập
                var user = await _userManager.FindByNameAsync(loginRequest.Username);

                if (user != null && await _userManager.CheckPasswordAsync(user, loginRequest.Password))
                {
                    // Đăng nhập thành công

                    // Tạo token JWT
                    var token = GenerateJwtToken(user);

                    // Gửi token về client
                    var response = new LoginResponseDto
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

        private string GenerateJwtToken(User user)
        {
            // Tạo Claims
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Name, user.UserName)
        // Bạn có thể thêm các Claims khác tùy thuộc vào yêu cầu của bạn
    };

            // Tạo key từ Secret
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Secret"]));

            // Tạo SigningCredentials
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // Tạo token
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(Convert.ToDouble(_configuration["Jwt:ExpireDays"])),
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
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (userId == null)
                {
                    return NotFound("User not found.");
                }

                // Tìm người dùng theo ID
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return NotFound("User not found.");
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


        [HttpPost("AssignRoleByEmail")]
        public async Task<IActionResult> AssignRoleByEmail([FromBody] EmailDTO emailDTO)
        {
            try
            {
                // Tìm người dùng theo email
                var user = await _userManager.FindByEmailAsync(emailDTO.UserEmail);

                if (user == null)
                {
                    return NotFound($"User with email {emailDTO.UserEmail} not found.");
                }

                // Kiểm tra xem vai trò "User" đã tồn tại hay chưa
                var defaultRole = "User";
                var roleExists = await _roleManager.RoleExistsAsync(defaultRole);

                if (!roleExists)
                {
                    // Nếu vai trò chưa tồn tại, tạo mới
                    await _roleManager.CreateAsync(new IdentityRole(defaultRole));
                }

                // Gán vai trò "User" cho người dùng
                var result = await _userManager.AddToRoleAsync(user, defaultRole);

                if (result.Succeeded)
                {
                    return Ok($"User {user.UserName} assigned to role {defaultRole} successfully.");
                }
                else
                {
                    return BadRequest("Failed to assign role to user.");
                }
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
                user.UserName = userDTO.Username;

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
                User user = _service.GetByEmail(Email);

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


        private string GenerateOTP()
        {
            // Tạo mã OTP ngẫu nhiên, ví dụ: 6 chữ số
            Random random = new Random();
            string otp = random.Next(100000, 999999).ToString();
            
            System.Diagnostics.Debug.WriteLine($"Generated OTP: {otp}");

            return otp;
        }

        private bool SendEmail(string toEmail, string otp)
        {
            try
            {
                // Retrieve email settings from IConfiguration
                string smtpServer = _configuration["EmailSettings:SmtpServer"];
                int port = int.Parse(_configuration["EmailSettings:Port"]);
                string userName = _configuration["EmailSettings:UserName"];
                string password = _configuration["EmailSettings:Password"];
                bool enableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"]);

                // Thiết lập thông tin SMTP
                //SmtpClient client = new SmtpClient("smtp.gmail.com")
                SmtpClient client = new SmtpClient(smtpServer)
                {
                    Port = port,
                    //Credentials = new NetworkCredential("huynhminhducdev@gmail.com", "qyry jthm gsou mjoc"),
                    Credentials = new NetworkCredential(userName, password),
                    EnableSsl = enableSsl,
                };

                // Tạo nội dung email
                MailMessage mailMessage = new MailMessage();
                //mailMessage.From = new MailAddress("huynhminhducdev@gmail.com");
                mailMessage.From = new MailAddress(userName);
                mailMessage.To.Add(toEmail);
                mailMessage.Subject = "Verification OTP";
                mailMessage.Body = $"Your OTP is: {otp}";

                // Gửi email
                try
                {
                    client.Send(mailMessage);
                    System.Diagnostics.Debug.WriteLine("Email sent successfully.");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error sending email: {ex.Message}");
                }
                return true;
            } 
            catch (Exception ex)
            {
                // Xử lý lỗi gửi email
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
