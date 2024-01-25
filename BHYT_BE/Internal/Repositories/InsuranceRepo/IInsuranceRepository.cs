using BHYT_BE.Internal.Models;
using Insurance = BHYT_BE.Internal.Models.Insurance;

namespace BHYT_BE.Internal.Repository.InsuranceRepo
{
    public interface IInsuranceRepository
    {
        public Task<List<Insurance>> GetAll();
        public Task<Insurance> GetByID(int id);
        public Insurance Create(Insurance insurance);
        public Insurance Update(Insurance insurance);
        Task<List<Insurance>> GetInsuranceByUserID(string userID);
    }
}
