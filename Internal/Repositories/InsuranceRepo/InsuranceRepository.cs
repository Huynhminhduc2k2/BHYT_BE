using BHYT_BE.Internal.Repository.Data;
using Microsoft.EntityFrameworkCore;
using Insurance = BHYT_BE.Internal.Models.Insurance;

namespace BHYT_BE.Internal.Repository.InsuranceRepo
{
    public class InsuranceRepository : IInsuranceRepository
    {
        private readonly InsuranceDBContext _context;
        public InsuranceRepository(InsuranceDBContext context)
        {
            _context = context;
        }
        public void Create(Insurance insurance)
        {
            _context.Insurances.Add(insurance);
            _context.SaveChanges();
        }

        public async Task<List<Insurance>> GetAll()
        {
            return await _context.Insurances.ToListAsync();
        }

        public Insurance GetByID(int id)
        {
            if (id == 0)
            {
                return null;
            }
            return _context.Insurances.Find(id);
        }



        public async Task<Insurance> GetByUserID(int userID)
        {
            if (userID == 0)
            {
                return null;
            }
            return await _context.Insurances.FirstOrDefaultAsync(insurance => insurance.UserID == userID);
        }

        public Insurance Update(Insurance insurance)
        {
            try
            {
                var result = _context.Insurances.Update(insurance);
                _context.SaveChanges();
                return result.Entity;
            } 
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
