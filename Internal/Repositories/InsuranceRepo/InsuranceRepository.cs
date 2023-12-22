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

        public  List<Insurance> GetAll()
        {
            return _context.Insurances.ToList();
        }

        public async Task<Insurance> GetByID(ulong id)
        {
            if (id == 0)
            {
                return null;
            }
            return await _context.Insurances.FindAsync(id);
        }



        public async Task<Insurance> GetByPersonID(string personID)
        {
            if (personID == "")
            {
                return null;
            }
            return await _context.Insurances.FirstOrDefaultAsync(insurance => insurance.PersonID == personID);
        }

        public async Task<Insurance> UpdateAsync(Insurance insurance)
        {
            _context.Entry(insurance).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return await Task.FromResult(insurance);
        }
    }
}
