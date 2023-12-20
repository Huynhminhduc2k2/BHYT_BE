using BCrypt.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BHYT_BE.Internal.Models;
using BHYT_BE.Internal.Services.UserService;
using BHYT_BE.Controllers.Types;
using Microsoft.AspNetCore.Authorization;

namespace BHYT_BE.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;
        private readonly IConfiguration _configuration;

        public UserController(IUserService userService, IConfiguration configuration)
        {
            _service = userService;
            _configuration = configuration;
        }


        [HttpPost("register")]
        public ActionResult<User> Register(UserDTO request)
        {
            try
            {

                // Hash mật khẩu trước khi lưu vào cơ sở dữ liệu
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.PasswordHash);

                User user = new User
                {
                    Email = request.Email,
                    PasswordHash = passwordHash,
                    Role = request.Role != null ? request.Role : UserRole.User 
                };

                _service.AddUser(user);

                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPost("login")]
        public ActionResult<string> Login(UserDTO request)
        {
            try
            {
                User user = _service.LoginUser(request.Email, request.PasswordHash);

                if (user == null)
                {
                    return BadRequest("User not found!");
                }

                // Tạo token JWT
                string token = CreateToken(user);

                return Ok(token);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpGet("protected")]
        public IActionResult GetProtectedData()
        {
            var user = HttpContext.User.Identity;
            if (user.IsAuthenticated)
            {
                // Kiểm tra xem user có phải là ClaimsPrincipal
                if (user is ClaimsPrincipal principal)
                {
                    // Lấy role từ claim
                    var role = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

                    // Kiểm tra role (nếu cần) và thực hiện logic phù hợp
                    if (role == UserRole.Admin.ToString())
                    {
                        return Ok("Welcome Admin!");
                    }
                    else
                    {
                        return Ok("Welcome User!");
                    }
                }
                else
                {
                    // Xử lý trường hợp user không phải là ClaimsPrincipal
                    return Unauthorized();
                }
            }
            else
            {
                return Unauthorized();
            }
        }

        private string CreateToken(User user)
        {
            // Thay đổi claim type từ Subject thành Name
            var claims = new List<Claim>
      {
        new Claim(ClaimTypes.Name, user.Email)
      };

            var key = _configuration.GetValue<string>("SecretKey");
            var keyBytes = Encoding.UTF8.GetBytes(key);
            var securityKey = new SymmetricSecurityKey(keyBytes);

            var creds = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Issuer = _configuration.GetValue<string>("Issuer"),
                Audience = _configuration.GetValue<string>("Audience"),
                Expires = DateTime.UtcNow.AddDays(_configuration.GetValue<int>("ExpireDays")),
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
    }
}