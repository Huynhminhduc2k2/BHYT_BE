using BCrypt.Net;
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
using BHYT_BE.Controllers.Types; // Thêm namespace này nếu IUserService ở trong namespace này

namespace BHYT_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;

        public UserController(IUserService userService)
        {
            _service = userService;
        }

        [HttpPost("register")]
        public ActionResult<User> Register(UserDTO request)
        {
            try
            {
                // Tạo ID tự động (sử dụng GUID)

                // Hash mật khẩu trước khi lưu vào cơ sở dữ liệu
                string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.PasswordHash);

                // Tạo đối tượng User từ dữ liệu đầu vào
                User user = new User
                {
                    Email = request.Email,
                    PasswordHash = passwordHash
                };

                // Gọi phương thức dịch vụ để thêm User mới
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
                // Gọi phương thức dịch vụ để kiểm tra đăng nhập
                User user = _service.LoginUser(request.Email, request.PasswordHash);

                if (user == null)
                {
                    return BadRequest("User not found or wrong password!");
                }

                // Tạo token cho người dùng đã đăng nhập
                string token = CreateToken(user);

                return Ok(token);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email)
            };

            var keyBytes = new byte[64]; // 512 bits = 64 bytes - Tạo key 

            using (var rng = new RNGCryptoServiceProvider())
            {
                rng.GetBytes(keyBytes);
            }

            var key = new SymmetricSecurityKey(keyBytes);

            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: cred
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }
        [HttpGet("user")]
        public ActionResult<UserInfo> GetUserByID([FromQuery] int id)
        {
            try
            {
                // Gọi phương thức dịch vụ để thêm User mới
                var userDTO = _service.GetById(id);
                UserInfo userInfo = new UserInfo
                {
                    Email = userDTO.Email
                };
                return Ok(userInfo);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
