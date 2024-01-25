using BHYT_BE.Internal.Models;
using BHYT_BE.Internal.Repository.Data;
using Microsoft.EntityFrameworkCore;

namespace BHYT_BE.Internal.Repository.InsurancePaymentHistoryRepo
{
    public class InsurancePaymenHistoryRepository : IInsurancePaymenHistoryRepository
    {
        private readonly InsurancePaymentHistoryDBContext _context;
        public InsurancePaymenHistoryRepository(InsurancePaymentHistoryDBContext context)
        {
            _context = context;
        }
        public InsurancePaymentHistory Create(InsurancePaymentHistory insurance)
        {
            _context.InsurancePaymentHistories.Add(insurance);
            _context.SaveChanges();
            return insurance;
        }
        public async Task<List<InsurancePaymentHistory>> GetInsurancePaymentHistoriesByInsuranceID(int insuranceID)
        {
            return await _context.InsurancePaymentHistories.Where(i => i.InsuranceID == insuranceID).ToListAsync();
        }
    }
}
