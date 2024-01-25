using System.ComponentModel.DataAnnotations;

namespace BHYT_BE.Controllers.Types
{
    public class UserType
    {

    }
    public class Register
    {
        [Required(ErrorMessage = "Họ và tên là bắt buộc")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Số CMND/CCCD là bắt buộc")]
        public string PersonID { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Ngày sinh là bắt buộc")]
        public DateTime DOB { get; set; }

        [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
        public string Address { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Quốc gia là bắt buộc")]
        public string Nation { get; set; }

        [Required(ErrorMessage = "Quốc tịch là bắt buộc")]
        public string Nationality { get; set; }

        [Required(ErrorMessage = "Giới tính là bắt buộc")]
        public string Sex { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Nhập lại mật khẩu là bắt buộc")]
        public string RePassword { get; set; }
        [Required]
        public List<string> Roles { get; set; }
    }
    public class UserInfoResponse
    {
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public string PersonID { get; set; }
        public string Address { get; set; }
        public DateTime DOB { get; set; }
        public string Nation { get; set; }
        public string Nationality { get; set; }
        public string Sex { get; set; }
        public string PhoneNumber { get; set; }
        public string OTP { get; set; }
        public string[] Roles { get; set; }

    }
    public class LoginRequest
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
    public class LoginResponse
    {
        public string token { get; set; }
    }
    public class AssignRoleRequest
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        public string Role { get; set; }
    }
}
