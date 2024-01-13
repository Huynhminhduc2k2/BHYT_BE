using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace BHYT_BE.Internal.Models
{
    public enum InsuranceType
    {
        STANDARD,
        ADVANDCE,
        PREMIUM,
    }
    public enum InsuranceStatus
    {
        CREATED,
        WAITING_PAYMENT,
        PENDING,
        ACCEPTED,
        REJECTED,
    }
    public class Insurance : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InsuranceID { get; set; }
        public int UserID { get; set; }
        public InsuranceType InsuranceType { get; set; }
        [MaxLength(50)] // ACCEPTED, REJECTED, PENDING
        public InsuranceStatus Status { get; set; }
        [MaxLength(64)]
        public string CreatedBy { get; set; }
        [MaxLength(64)]
        public string UpdatedBy { get; set; }
    }

    public class InsuranceHistory : BaseEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InsuranceHistoryID { get; set; }
        public int InsuranceID { get; set; }
        public InsuranceStatus OldStatus { get; set; }
        public InsuranceStatus NewStatus { get; set; }
        public string Remark { get; set; }
        public string Email { get; set; }
        [MaxLength(64)]
        public string CreatedBy { get; set; }
        [MaxLength(64)]
        public string UpdatedBy { get; set; }

    }
}
