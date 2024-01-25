using BHYT_BE.Internal.Models;
using System.ComponentModel.DataAnnotations;

namespace BHYT_BE.Controllers.Types
{
    public class RequestRegisterInsurance
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
        public string InsuranceType { get; set; }
    }
    public class RegisterInsurance
    {
        public string InsuranceType { get; set; }
    }
    public class EditInsuranceRequest
    {
        public int InsuranceID { get; set; }
        public string UserID { get; set; }
        public string InsuranceType { get; set; }
        public string Status { get; set; }
    }
    public class InsuranceResponse
    {
        public int InsuranceID { get; set; }
        public string UserID { get; set; }
        public string Type { get; set; }
        public string Status { get; set; }
        public decimal PremiumAmount { get; set; } // Số tiền phí bảo hiểm
        public DateTime? StartDate { get; set; } // Ngày bắt đầu hiệu lực hợp đồng
        public DateTime? EndDate { get; set; } // Ngày kết thúc hiệu lực hợp đồng
        public DateTime? LastPaymentDate { get; set; } // Ngày thanh toán gần nhất
        public bool IsAutoRenewal { get; set; } // Có tự động gia hạn không
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    public class InsuranceHistoryResponse
    {
        public int InsuranceHistoryID { get; set; }
        public int InsuranceID { get; set; }
        public string OldStatus { get; set; }
        public string NewStatus { get; set; }
        public string? Remark { get; set; }
        public string? Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
    public class InsurancePaymentHistoryResponse
    {
        public int PaymentHistoryID { get; set; }
        public int InsuranceID { get; set; } // Khóa ngoại liên kết với Insurance
        public decimal AmountPaid { get; set; } // Số tiền đã thanh toán
        public DateTime PaymentDate { get; set; } // Ngày thanh toán
        public string PaymentMethod { get; set; } // Phương thức thanh toán (chuyển khoản, thẻ tín dụng, ...)
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    public class InsuranceDetailResponse
    {
        public InsuranceResponse InsuranceResp { get; set; }
        public List<InsuranceHistoryResponse>? InsuranceHistoryResp { get; set; }
        public List<InsurancePaymentHistoryResponse>? InsurancePaymentHistoryResp { get; set; }
    }
}