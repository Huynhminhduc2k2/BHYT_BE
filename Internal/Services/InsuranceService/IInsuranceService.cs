using BHYT_BE.Controllers.Types;

namespace BHYT_BE.Internal.Services.InsuranceService
{
    public interface IInsuranceService
    {
        void AddInsurance(RegisterInsuraceDTO req);
        void UpdateInsurance(InsuranceDTO insuranceDTO, bool isAdmin, int adminID);
        List<InsuranceDTO> GetAllInsurances();
        InsuranceDTO GetInsuranceByID(ulong id);
        InsuranceDTO GetInsuranceByPersonID(string personID);
        bool AcceptInsurance(int insuranceID);
        bool RejectInsurance(int insuranceID);
        InsuranceDTO RequestInsurance(RequestInsuraceDTO req);
    }
}
