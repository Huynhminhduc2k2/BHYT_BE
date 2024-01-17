using System;
using System.ComponentModel.DataAnnotations;

namespace BHYT_BE.Internal.Services.UserService
{
    public class UserDTO
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        // Thêm thuộc tính để lưu danh sách vai trò
        [Required]
        public string[] Roles { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string PersonID { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public DateTime DOB { get; set; }

        [Required]
        public string Nation { get; set; }

        [Required]
        public string Nationality { get; set; }

        [Required]
        [MaxLength(10)]
        public string Sex { get; set; }

        [Required]
        [Phone]
        public string PhoneNumber { get; set; }
 
    }

    public class LoginResponseDto
    {
        public string token { get; set; }
    }

    public class LoginRequestDTO
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    public class AssignRoleRequestDTO
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        public string Role { get; set; }
    }
}
