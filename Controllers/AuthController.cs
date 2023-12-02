using BCrypt.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BHYT_BE.Models;

namespace BHYT_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        public static User user = new User();

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("register")]
        public ActionResult<User> Register(UserDTO request)
        {
            string passwordHash = BCrypt.Net.BCrypt.HashPassword(request.Password);
            user.Username = request.Username;
            user.PasswordHash = passwordHash;

            return Ok(user);
        }

        [HttpPost("login")]
        public ActionResult<string> Login(UserDTO request)
        {
            if (user.Username != request.Username)
            {
                return BadRequest("User not found!");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            {
                return BadRequest("Wrong Password!");
            }

            string token = CreateToken(user);

            return Ok(token);
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username)
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
    }
}