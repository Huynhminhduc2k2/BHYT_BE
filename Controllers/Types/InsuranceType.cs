using System.ComponentModel.DataAnnotations;

namespace BHYT_BE.Controllers.Types
{
    public class RegisterInsurance
    {
        public int UserID { get; set; }
        public string InsuranceType { get; set; }
    }
    public class EditInsuranceRequest
    {
        public int InsuranceID { get; set; }
        public int UserID { get; set; }
        public string InsuranceType { get; set; }
        public string Status { get; set; }

    }

}
