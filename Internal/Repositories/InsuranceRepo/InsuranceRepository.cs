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

        public Insurance GetByID(int id)
        {
            if (id == 0)
            {
                return null;
            }
            return _context.Insurances.Find(id);
        }



        public async Task<Insurance> GetByPersonID(string personID)
        {
            if (personID == "")
            {
                return null;
            }
            return await _context.Insurances.FirstOrDefaultAsync(insurance => insurance.PersonID == personID);
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
