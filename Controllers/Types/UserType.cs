using System.ComponentModel.DataAnnotations;

namespace BHYT_BE.Controllers.Types
{
    public class UserType
    {

    }
    public class Register
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Nhập lại mật khẩu là bắt buộc")]
        public string RePassword { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
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

}
