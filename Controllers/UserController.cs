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
using BHYT_BE.Internal.Services.UserService; // Thêm namespace này nếu IUserService ở trong namespace này

using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Caching.Memory;

namespace BHYT_BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _service;
        private readonly IMemoryCache _memoryCache;

        public UserController(IUserService userService, IMemoryCache memoryCache)
        {
            _service = userService;
            _memoryCache = memoryCache;
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

                string otp = GenerateOTP();
                SendEmail(request.Email, otp);

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

        [HttpPost("sendOTP")]
        public ActionResult<string> SendOTP([FromBody] SendOTPRequest request)
        {
            // Tạo mã OTP ngẫu nhiên
            string otp = GenerateOTP();

            // Lưu giữ giá trị OTP vào MemoryCache với khóa là email người dùng
            _memoryCache.Set(request.Email, otp);

            System.Diagnostics.Debug.WriteLine($"Generated OTP: {otp}");

            // Gửi email
            if (SendEmail(request.Email, otp))
            {
                return Ok(new { Message = "OTP sent successfully." });
            }
            else
            {
                return BadRequest(new { Message = "Failed to send OTP." });
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

            string expectedEmail = GetUserByEmail(request.Email);

            System.Diagnostics.Debug.WriteLine(expectedOTP);
            System.Diagnostics.Debug.WriteLine(expectedEmail);
            if (request.OTP == expectedOTP && request.Email == expectedEmail)
            {
                return Ok("OTP verified successfully.");
            }
            else
            {
                return BadRequest("Invalid OTP.");
            }
        }

        private string GetUserByEmail(string Email)
        {
            try
            {
                // Gọi phương thức dịch vụ để lấy người dùng theo email
                User user = _service.GetUserByEmail(Email);

                if (user == null)
                {
                    System.Diagnostics.Debug.WriteLine($"User with email {Email} not found.");

                    return "User not found.";
                }

                System.Diagnostics.Debug.WriteLine($"User with email {Email} found.");
                return user.Email;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }


        private string GenerateOTP()
        {
            // Tạo mã OTP ngẫu nhiên, ví dụ: 6 chữ số
            Random random = new Random();
            string otp = random.Next(100000, 999999).ToString();
            return otp;
        }

        private bool SendEmail(string toEmail, string otp)
        {
            try
            {
                // Thiết lập thông tin SMTP
                SmtpClient client = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("huynhminhducdev@gmail.com", "qyry jthm gsou mjoc"),
                    EnableSsl = true,
                };

                // Tạo nội dung email
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress("huynhminhducdev@gmail.com");
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

                //client.Send(mailMessage);

                return true;
            }
            catch (Exception ex)
            {
                // Xử lý lỗi gửi email
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        //[HttpPost("getUserByEmail")]
        //public ActionResult<User> GetUserByEmail([FromBody] EmailDTO request)
        //{
        //    try
        //    {
        //        // Gọi phương thức dịch vụ để lấy người dùng theo email
        //        User user = _service.GetUserByEmail(request.UserEmail);

        //        if (user == null)
        //        {
        //            System.Diagnostics.Debug.WriteLine($"User with email {request.UserEmail} not found.");
                    
        //            return NotFound("User not found.");
        //        }

        //        System.Diagnostics.Debug.WriteLine($"User with email {request.UserEmail} found.");
        //        return Ok(user);
        //    }
        //    catch (Exception ex)
        //    {
        //        return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        //    }
        //}

    }
}

