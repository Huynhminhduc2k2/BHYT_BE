using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace IS220_PROJECT.Models
{
    public class RegisterViewModel
    {
        [Key]
        public int AccountId { get; set; }
        [Display(Name = "Họ tên")]
        [Required(ErrorMessage = "Vui lòng nhập họ và tên")]
        public string FullName { get; set; }
        [MaxLength(100)]
        [Display(Name = "Email")]
        [Required(ErrorMessage = "Vui lòng nhập email")]
        [DataType(DataType.EmailAddress)]
        //[EmailAddress]
        //[Remote(action: "ValidateEmail", controller: "Account", HttpMethod = "POST",
        //ErrorMessage = "Email đã tồn tại")]
        public string Email { get; set; }
        [MaxLength(10)]
        [Display(Name = "Số điện thoại")]
        [Required(ErrorMessage = "Vui lòng nhập số điện thoại")]
        [DataType(DataType.PhoneNumber)]
        //[Remote(action: "ValidatePhoneNumber", controller: "Account")]
        public string PhoneNumber { get; set; }

        [MaxLength(100)]
        [Required(ErrorMessage = "Vui lòng nhập đúng tài khoản")]
        [Display(Name = "Nhập tài khoản")]
        public string UserName { get; set; }

        [Display(Name = "Nhập mật khẩu")]
        [Required(ErrorMessage = "Vui lòng nhập đúng mật khẩu")]
        [MinLength(5, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự")]
        public string Password { get; set; }

        [Display(Name = "Nhập lại mật khẩu")]
        [Required(ErrorMessage = "Vui lòng nhập lại mật khẩu")]
        [MinLength(5, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự")]
        public string ConfirmPassword { get; set; }
    }
}
