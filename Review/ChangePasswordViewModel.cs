using System.ComponentModel.DataAnnotations;

namespace IS220_PROJECT.Models
{
    public class ChangePasswordViewModel
    {
        [Key]
        public int AccountId { get; set; }
        [Display(Name = "Nhập mật khẩu")]
        [Required(ErrorMessage = "Vui lòng nhập đúng mật khẩu")]
        [MinLength(5, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự")]
        public string OldPassword { get; set; }
        [Display(Name = "Nhập mật khẩu")]
        [Required(ErrorMessage = "Vui lòng nhập đúng mật khẩu")]
        [MinLength(5, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự")]
        public string NewPassword { get; set; }

        [Display(Name = "Nhập lại mật khẩu")]
        [Required(ErrorMessage = "Vui lòng nhập lại mật khẩu")]
        [MinLength(5, ErrorMessage = "Mật khẩu tối thiểu 6 ký tự")]
        public string ConfirmNewPassword { get; set; }
    }
}
