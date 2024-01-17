using System.ComponentModel.DataAnnotations;

namespace IS220_PROJECT.Models
{
    public class LoginViewModel
    {
        [Key]
        [MaxLength(100)]
        [Required(ErrorMessage = "Vui lòng nhập đúng tài khoản")]
        [Display(Name = "Nhập tài khoản")]
        public string UserName { get; set; }

        [Display(Name = "Nhập mật khẩu")]
        [Required(ErrorMessage = "Vui lòng nhập đúng mật khẩu")]
        [MinLength(5, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự")]
        public string Password { get; set; }
    }
}
