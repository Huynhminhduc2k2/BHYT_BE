using BHYT_BE.Internal.Models;

namespace BHYT_BE.Internal.Repository.InsurancePaymentHistoryRepo
{
    public interface IInsurancePaymenHistoryRepository
    {
        public Task<List<InsurancePaymentHistory>> GetInsurancePaymentHistoriesByInsuranceID(int insuranceID);
        public InsurancePaymentHistory Create(InsurancePaymentHistory insurance);
    }
}
