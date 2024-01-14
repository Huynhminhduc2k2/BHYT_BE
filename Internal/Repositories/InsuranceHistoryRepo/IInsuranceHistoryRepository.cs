using BHYT_BE.Internal.Models;
using InsuranceHistory = BHYT_BE.Internal.Models.InsuranceHistory;

namespace BHYT_BE.Internal.Repository.InsuranceHistoryRepo
{
    public interface IInsuranceHistoryRepository
    {
        public List<InsuranceHistory> GetInsuranceHistoriesByInsuranceID(int insuranceID);
        public InsuranceHistory Create(InsuranceHistory insurance);
    }
}
