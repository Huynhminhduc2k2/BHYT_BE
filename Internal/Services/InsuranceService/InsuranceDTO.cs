using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace BHYT_BE.Internal.Services.InsuranceService
{
    public class InsuranceDTO
    {
        public int InsuranceID { get; set; }
        public string FullName { get; set; }
        public string PersonID { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime DOB { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string Nation { get; set; }
        public string Nationality { get; set; }
        public string Sex { get; set; }
        public string Status { get; set; }

    }
    public class RegisterInsuraceDTO
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
    }
}
