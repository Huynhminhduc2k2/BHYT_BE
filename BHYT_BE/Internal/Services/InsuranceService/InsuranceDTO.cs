using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using BHYT_BE.Internal.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using BHYT_BE.Controllers.Types;

namespace BHYT_BE.Internal.Services.InsuranceService
{
    public class InsuranceDTO
    {
        public int InsuranceID { get; set; }
        public string UserID { get; set; }
        public InsuranceType Type { get; set; }
        public InsuranceStatus Status { get; set; }
        public decimal PremiumAmount { get; set; } // Số tiền phí bảo hiểm
        public DateTime? StartDate { get; set; } // Ngày bắt đầu hiệu lực hợp đồng
        public DateTime? EndDate { get; set; } // Ngày kết thúc hiệu lực hợp đồng
        public DateTime? LastPaymentDate { get; set; } // Ngày thanh toán gần nhất
        public bool IsAutoRenewal { get; set; } // Có tự động gia hạn không
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        internal object Select(Func<object, InsuranceResponse> value)
        {
            throw new NotImplementedException();
        }
    }

    public class InsuranceHistoryDTO
    {
        public int InsuranceHistoryID { get; set; }
        public int InsuranceID { get; set; }
        public InsuranceStatus OldStatus { get; set; }
        public InsuranceStatus NewStatus { get; set; }
        public string? Remark { get; set; }
        public string? Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

    }
    public class InsurancePaymentHistoryDTO
    {
        public int PaymentHistoryID { get; set; }
        public int InsuranceID { get; set; } // Khóa ngoại liên kết với Insurance
        public decimal AmountPaid { get; set; } // Số tiền đã thanh toán
        public DateTime PaymentDate { get; set; } // Ngày thanh toán
        public InsurancePaymentMethod PaymentMethod { get; set; } // Phương thức thanh toán (chuyển khoản, thẻ tín dụng, ...)
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    public class InsuranceDetailDTO
    {
        public InsuranceDTO Insurance { get; set; }
        public List<InsuranceHistoryDTO> History { get; set; }
        public List<InsurancePaymentHistoryDTO> PaymentHistory { get; set; }
    }
    public class RegisterInsuraceDTO
    {
        public string UserID { get; set; }
        public InsuranceType Type { get; set; }
    }

    public class RequestInsuraceDTO
    {
        public string FullName { get; set; }
        public string PersonID { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DOB { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Nation { get; set; }
        public string Nationality { get; set; }
        public string Sex { get; set; }
        public InsuranceType InsuranceType { get; set; }
    }
}
