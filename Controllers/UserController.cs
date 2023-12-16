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
        [HttpPost("verifyOTP")]
        public ActionResult<string> VerifyOTP([FromBody] VerifyOTPRequest request)
        {
            // Thực hiện xác minh mã OTP tại đây
            // Bạn có thể kiểm tra mã OTP có hợp lệ không và thực hiện các hành động cần thiết

            // Ở đây, tôi sẽ giả định mã OTP cần xác minh là "123456"
            string expectedOTP = request.OTP;
            System.Diagnostics.Debug.WriteLine(expectedOTP);
            if (request.OTP == expectedOTP)
            {
                return Ok("OTP verified successfully.");
            }
            else
            {
                return BadRequest("Invalid OTP.");
            }
        }

        [HttpPost("sendOTP")]
        public ActionResult<string> SendOTP([FromBody] SendOTPRequest request)
        {
            // Tạo mã OTP ngẫu nhiên
            string otp = GenerateOTP();

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
                    Credentials = new NetworkCredential("your-email@example.com", "your-email-password"),
                    EnableSsl = true,
                };

                // Tạo nội dung email
                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress("your-email@example.com");
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
    }
}

