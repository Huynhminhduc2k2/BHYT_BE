using BHYT_BE.Controllers.Types;
using BHYT_BE.Internal.Models;

namespace BHYT_BE.Internal.Services.InsuranceService
{
    public interface IInsuranceService
    {
        void AddInsurance(RegisterInsuraceDTO req);

        void UpdateInsurance(InsuranceDTO insuranceDTO);
        List<Insurance> GetAllInsurances();
        void UpdateInsurance(InsuranceDTO insuranceDTO, bool isAdmin);
        List<InsuranceDTO> GetAllInsurances();

        InsuranceDTO GetInsuranceByID(ulong id);
        InsuranceDTO GetInsuranceByPersonID(string personID);
        bool AcceptInsurance(int insuranceID);
        bool RejectInsurance(int insuranceID);
    }
}
