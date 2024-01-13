using BHYT_BE.Internal.Models;
using BHYT_BE.Internal.Repository.InsuranceRepo;
using System.ComponentModel.DataAnnotations;
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

        public bool AcceptInsurance(int insuranceID)
        {
            try
            {
                Insurance insurance = _insuranceRepo.GetByID(insuranceID);
                if (insurance == null)
                {
                    return false;
                }
                insurance.Status = InsuranceStatus.ACCEPTED;
                insurance = _insuranceRepo.Update(insurance);
                
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while accepting insurance");
                throw; // Rethrow the exception after logging
            }
        }

        public void AddInsurance(RegisterInsuraceDTO req)
        {
            try
            {
                // TODO: check user id
                Insurance insurance = new Insurance
                {
                    UserID = req.UserID,
                    InsuranceType = req.Type,
                    Status = InsuranceStatus.PENDING
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

        public bool RejectInsurance(int insuranceID)
        {
            try
            {
                Insurance insurance = _insuranceRepo.GetByID(insuranceID);
                if (insurance == null)
                {
                    return false;
                }
                insurance.Status = InsuranceStatus.REJECTED;
                insurance = _insuranceRepo.Update(insurance);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while accepting insurance");
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

        public void UpdateInsurance(InsuranceDTO req, bool isAdmin)
        {
            try
            {
                Insurance insurance = _insuranceRepo.GetByID(req.InsuranceID);
                // TODO: check user id 
                insurance.UserID = req.UserID;
                if (isAdmin)
                {
                    insurance.Status = req.Status;
                }
                _insuranceRepo.Update(insurance);
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
