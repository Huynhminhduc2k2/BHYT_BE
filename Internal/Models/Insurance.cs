using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using System.Net.Mail;

namespace BHYT_BE.Internal.Models
{
    public enum InsuranceType
    {
        [EnumMember(Value = "STANDARD")]
        STANDARD,
        [EnumMember(Value = "ADVANDCE")]
        ADVANDCE,
        [EnumMember(Value = "PREMIUM")]
        PREMIUM,
    }
    public class InsurancePrice
    {
        public static readonly decimal STANDARD = 100.00m;
        public static readonly decimal ADVANCE = 200.00m;
        public static readonly decimal PREMIUM = 300.00m;
        public decimal Price { get; set; }
        public InsurancePrice(InsuranceType type)
        {
            switch (type)
            {
                case InsuranceType.STANDARD:
                    Price = STANDARD;
                    break;
                case InsuranceType.ADVANDCE:
                    Price = ADVANCE;
                    break;
                case InsuranceType.PREMIUM:
                    Price = PREMIUM;
                    break;
            }
        }
    }

    // InsuranceType type = InsuranceType.STANDARD;
    // InsurancePrice price = (InsurancePrice)type;
    public enum InsuranceStatus
    {
        [EnumMember(Value = "CREATED")]
        CREATED,
        [EnumMember(Value = "WAITING_PAYMENT")]
        WAITING_PAYMENT,
        [EnumMember(Value = "PENDING")]
        PENDING,
        [EnumMember(Value = "PAID")]
        PAID,
        [EnumMember(Value = "ACCEPTED")]
        ACCEPTED,
        [EnumMember(Value = "REJECTED")]
        REJECTED,
    }
    public enum InsurancePaymentMethod
    {
        [EnumMember(Value = "BANK_STRANFER")]
        BANK_STRANFER,
        [EnumMember(Value = "CREDIT_CARD")]
        CREDIT_CARD,
        [EnumMember(Value = "PAYPAL")]
        PAYPAL,
    }
    [Index(nameof(UserID))]
    public class Insurance : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InsuranceID { get; set; }
        public string UserID { get; set; }
        public InsuranceType InsuranceType { get; set; }
        [MaxLength(50)] // ACCEPTED, REJECTED, PENDINGS
        public InsuranceStatus Status { get; set; }
        [MaxLength(64)]
        public decimal PremiumAmount { get; set; } // Số tiền phí bảo hiểm
        public DateTime? StartDate { get; set; } // Ngày bắt đầu hiệu lực hợp đồng
        public DateTime? EndDate { get; set; } // Ngày kết thúc hiệu lực hợp đồng
        public DateTime? LastPaymentDate { get; set; } // Ngày thanh toán gần nhất
        public bool IsAutoRenewal { get; set; } // Có tự động gia hạn không
        public string? CreatedBy { get; set; }
        [MaxLength(64)]
        public string? UpdatedBy { get; set; }
    }
    public class InsurancePaymentHistory : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PaymentHistoryID { get; set; }
        public int InsuranceID { get; set; } // Khóa ngoại liên kết với Insurance
        public decimal AmountPaid { get; set; } // Số tiền đã thanh toán
        public DateTime PaymentDate { get; set; } // Ngày thanh toán
        public InsurancePaymentMethod PaymentMethod { get; set; } // Phương thức thanh toán (chuyển khoản, thẻ tín dụng, ...)
    }
    public class InsuranceHistory : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InsuranceHistoryID { get; set; }
        public int InsuranceID { get; set; }
        public InsuranceStatus OldStatus { get; set; }
        public InsuranceStatus NewStatus { get; set; }
        public string? Remark { get; set; }
        public string? Email { get; set; }
        [MaxLength(64)]
        public string? CreatedBy { get; set; }
        [MaxLength(64)]
        public string? UpdatedBy { get; set; }

    }
    public class InsuranceRequest : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InsuranceRequestID { get; set; }
        public int InsuranceID { get; set; } // Khóa ngoại liên kết với Insurance
        public decimal RequestedAmount { get; set; } // Số tiền yêu cầu thanh toán
        public DateTime RequestDate { get; set; } // Ngày yêu cầu thanh toán
        public string RequestedBy { get; set; } // Người yêu cầu thanh toán (có thể là ID của người dùng)
        public string MedicalRecord { get; set; } // Bản ghi y tế hoặc mô tả điều trị
        public bool? IsApproved { get; set; }
        public DateTime? ApprovalDate { get; set; }
        [MaxLength(255)]
        public string? RejectionReason { get; set; }
    }
    [Index(nameof(InsuranceRequestID), IsUnique = true)]
    public class InsuranceRequestPayment : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InsuranceRequestPaymentID { get; set; }
        public int InsuranceRequestID { get; set; }
        public decimal AmountPaid { get; set; } 
        public DateTime PaymentDate { get; set; }
        public InsurancePaymentMethod PaymentMethod { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
    }
}
