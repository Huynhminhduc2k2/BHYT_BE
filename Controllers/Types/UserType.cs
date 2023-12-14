using System.ComponentModel.DataAnnotations;

namespace BHYT_BE.Controllers.Types
{
    public class UserType
    {

    }
    public class Register
    {
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Nhập lại mật khẩu là bắt buộc")]
        public string RePassword { get; set; }
    }

}
