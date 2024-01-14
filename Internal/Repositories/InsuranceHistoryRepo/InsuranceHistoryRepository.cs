using BHYT_BE.Internal.Repository.Data;
using Microsoft.EntityFrameworkCore;
using InsuranceHistory = BHYT_BE.Internal.Models.InsuranceHistory;

namespace BHYT_BE.Internal.Repository.InsuranceHistoryRepo
{
    public class InsuranceHistoryRepository : IInsuranceHistoryRepository
    {
        private readonly InsuranceHistoryDBContext _context;
        public InsuranceHistoryRepository(InsuranceHistoryDBContext context)
        {
            _context = context;
        }
        public InsuranceHistory Create(InsuranceHistory insurance)
        {
            _context.InsuranceHistories.Add(insurance);
            _context.SaveChanges();
            return insurance;
        }
        public List<InsuranceHistory> GetInsuranceHistoriesByInsuranceID(int insuranceID)
        {
            return _context.InsuranceHistories.Where(i => i.InsuranceID == insuranceID).ToList();
        }
    }
}
