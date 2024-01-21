using BHYT_BE.Controllers.Types;

namespace BHYT_BE.Internal.Services.InsuranceService
{
    public interface IInsuranceService
    {
        void AddInsurance(RegisterInsuraceDTO req);
        void UpdateInsurance(InsuranceDTO insuranceDTO, bool isAdmin, string userId);
        List<InsuranceDTO> GetAllInsurances();
        InsuranceDTO GetInsuranceByID(int id);
        bool AcceptInsurance(int insuranceID);
        bool RejectInsurance(int insuranceID);
        Task<InsuranceDTO> RequestInsuranceAsync(RequestInsuraceDTO req);
    }
}
