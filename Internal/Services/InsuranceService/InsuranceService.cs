using BHYT_BE.Internal.Repository.InsuranceRepo;
using Insurance = BHYT_BE.Internal.Models.Insurance;

namespace BHYT_BE.Internal.Services.InsuranceService
{
    public class InsuranceService : IInsuranceService
    {
        private readonly IInsuranceRepository _insuranceRepo;
        private readonly ILogger<InsuranceService> _logger;

        public InsuranceService(IInsuranceRepository insuranceRepo, ILogger<InsuranceService> logger)
        {
            _insuranceRepo = insuranceRepo;
            _logger = logger;
        }

        public void AddInsurance(RegisterInsuraceDTO req)
        {
            try
            {
                Insurance insurance = new Insurance
                {
                    Address = req.Address,
                    DOB = req.DOB,
                    Nation = req.Nation,
                    PhoneNumeber = req.PhoneNumber,
                    Email = req.Email,
                    Sex = req.Sex,
                    Nationality = req.Nationality,
                    FullName = req.FullName,
                    PersonID = req.PersonID
                };

                _insuranceRepo.Create(insurance);

                // Additional logic if needed

                _logger.LogInformation("Insurance created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding insurance");
                throw; // Rethrow the exception after logging
            }
        }

        public List<InsuranceDTO> GetAllInsurances()
        {
            throw new NotImplementedException();
        }

        public InsuranceDTO GetInsuranceByID(ulong id)
        {
            throw new NotImplementedException();
        }

        public InsuranceDTO GetInsuranceByPersonID(string personID)
        {
            throw new NotImplementedException();
        }

        public void UpdateInsurance(InsuranceDTO req)
        {
            try
            {
                Insurance insurance = new Insurance();
                    //Address = req.Address,
                    //DOB = req.DOB,
                    //Nation = req.Nation,
                    //PhoneNumeber = req.PhoneNumber,
                    //Email = req.Email,
                    //Sex = req.Sex,
                    //Nationality = req.Nationality,
                    //FullName = req.FullName,
                    //PersonID = req.PersonID

                _insuranceRepo.Create(insurance);

                // Additional logic if needed

                _logger.LogInformation("Insurance created successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while adding insurance");
                throw; // Rethrow the exception after logging
            }
        }
    }
}
