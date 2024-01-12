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
using BHYT_BE.Internal.Services.UserService;
using BHYT_BE.Controllers.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace BHYT_BE.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration configuration;

        public UserController(UserManager<IdentityUser> userManager, IConfiguration configuration)
        {
            this.userManager = userManager;
            this.configuration = configuration;
        }

        // API để lấy danh sách tất cả users
        [HttpGet]
        public IActionResult GetAll()
        {
            var users = userManager.Users.ToList();
            return Ok(users);
        }

        // API để lấy thông tin chi tiết của một user
        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            var user = userManager.Users.FirstOrDefault(x => x.Id == id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            return Ok(user);
        }

        // API để tạo một user mới
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] UserDTO userDTO)
        {
            try
            {
                // Tạo một password hash từ chuỗi plain text
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(userDTO.Password);

                // Tạo một user mới với password hash
                var user = new IdentityUser
                {
                    UserName = userDTO.Username,
                    PasswordHash = passwordHash
                };

                // Lưu user vào database
                var createResult = await userManager.CreateAsync(user);

                if (createResult.Succeeded)
                {
                    if (userDTO.Roles != null && userDTO.Roles.Any())
                    {
                        await userManager.AddToRolesAsync(user, userDTO.Roles);
                    }

                    return Ok("User was created successfully");
                }

                return BadRequest("Failed to create user");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        // API để cập nhật thông tin của một user
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UserDTO userDTO)
        {
            var user = await userManager.FindByIdAsync(id.ToString());
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

                var updateResult = await userManager.UpdateAsync(user);

                if (updateResult.Succeeded)
                {
                    if (userDTO.Roles != null && userDTO.Roles.Any())
                    {
                        await userManager.AddToRolesAsync(user, userDTO.Roles);
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
            var user = await userManager.FindByIdAsync(id.ToString());
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

                await userManager.RemoveFromRolesAsync(user, await userManager.GetRolesAsync(user));

                var deleteResult = await userManager.DeleteAsync(user);

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
    }
}
