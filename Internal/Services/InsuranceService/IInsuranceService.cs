using BHYT_BE.Controllers.Types;

namespace BHYT_BE.Internal.Services.InsuranceService
{
    public interface IInsuranceService
    {
        void AddInsurance(RegisterInsuraceDTO req);
        void UpdateInsurance(InsuranceDTO insuranceDTO);
        List<InsuranceDTO> GetAllInsurances();
        InsuranceDTO GetInsuranceByID(ulong id);
        InsuranceDTO GetInsuranceByPersonID(string personID);
    }
}
