using BHYT_BE.Controllers.Types;
using BHYT_BE.Internal.Models;

namespace BHYT_BE.Internal.Services.InsuranceService
{
    public interface IInsuranceService
    {
        void AddInsurance(RegisterInsuraceDTO req);
        void UpdateInsurance(InsuranceDTO insuranceDTO);
        List<Insurance> GetAllInsurances();
        InsuranceDTO GetInsuranceByID(ulong id);
        InsuranceDTO GetInsuranceByPersonID(string personID);
    }
}
