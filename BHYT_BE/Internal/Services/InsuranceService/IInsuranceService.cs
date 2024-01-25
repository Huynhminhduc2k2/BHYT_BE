namespace BHYT_BE.Internal.Services.InsuranceService
{
    public interface IInsuranceService
    {
        Task<InsuranceDTO> AddInsurance(RegisterInsuraceDTO req);
        void UpdateInsurance(InsuranceDTO insuranceDTO, bool isAdmin, string userId);
        Task<List<InsuranceDTO>> GetAllInsurancesByUserAsync(string? userID);
        Task<List<InsuranceDTO>> GetAllInsurancesAsync(string? userID);

        Task<InsuranceDTO> GetInsuranceByID(int id);
        Task<InsuranceDetailDTO> GetInsuranceDetail(int id);
        bool AcceptInsurance(int insuranceID);
        bool RejectInsurance(int insuranceID);
        Task<InsuranceDTO> RequestInsuranceFromNewUserAsync(RequestInsuraceDTO req);
    }
}
