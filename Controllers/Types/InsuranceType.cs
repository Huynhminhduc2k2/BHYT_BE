using System.ComponentModel.DataAnnotations;

namespace BHYT_BE.Controllers.Types
{
    public class InsuranceType
    {
    }
    public class RegisterInsurance
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
    }
}
