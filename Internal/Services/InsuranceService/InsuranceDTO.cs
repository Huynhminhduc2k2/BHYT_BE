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
        public int UserID { get; set; }
        public InsuranceType Type { get; set; }
        public InsuranceStatus Status { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

    }
    public class RegisterInsuraceDTO
    {
        public int UserID { get; set; }
        public InsuranceType Type { get; set; }
    }
}
