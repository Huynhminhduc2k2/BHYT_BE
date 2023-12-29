using BHYT_BE.Internal.Models;
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
        [Required]
        public string[] Roles { get; set; }

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


}
