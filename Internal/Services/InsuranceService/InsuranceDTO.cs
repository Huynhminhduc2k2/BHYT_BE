using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using BHYT_BE.Internal.Models;
using Microsoft.AspNetCore.Http.HttpResults;

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
