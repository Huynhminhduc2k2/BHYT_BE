using Insurance = BHYT_BE.Internal.Models.Insurance;

namespace BHYT_BE.Internal.Repository.InsuranceRepo
{
    public interface IInsuranceRepository
    {
        public Task<List<Insurance>> GetAll();
        public Insurance GetByID(int id);
        public Task<Insurance> GetByUserID(int userID);
        public void Create(Insurance insurance);
        public Insurance Update(Insurance insurance);

    }
}
