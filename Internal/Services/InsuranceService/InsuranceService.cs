using BHYT_BE.Internal.Repository.InsuranceRepo;

using Microsoft.AspNetCore.Http.HttpResults;
using System.Collections.Generic;
=======
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
                insurance.Status = Insurance.ACCEPTED;
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
                if (req.Sex != Insurance.FEMALE && req.Sex != Insurance.MALE)
                {
                    throw new ValidationException("Sex phải là FEMALE hoặc MALE");
                }
                Insurance insurance = new Insurance
                {
                    Address = req.Address,
                    DOB = req.DOB,
                    Nation = req.Nation,
                    PhoneNumber = req.PhoneNumber,
                    Email = req.Email,
                    Sex = req.Sex,
                    Nationality = req.Nationality,
                    FullName = req.FullName,
                    PersonID = req.PersonID,
                    Status = Insurance.PENDING
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


        public List<Insurance> GetAllInsurances()

        public bool RejectInsurance(int insuranceID)
        {
            try
            {
                Insurance insurance = _insuranceRepo.GetByID(insuranceID);
                if (insurance == null)
                {
                    return false;
                }
                insurance.Status = Insurance.REJECTED;
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
            try
            {
              var  Insurances = _insuranceRepo.GetAll();
                return Insurances;

                
                _logger.LogInformation("Get Insurance successfully");
            }
            catch (Exception ex) 
            {
                _logger.LogError(ex, "Error while returning insurance: No insurance");
                throw ex;
                
            }
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
                insurance.Address = req.Address;
                insurance.DOB = req.DOB;
                insurance.Nation = req.Nation;
                insurance.PhoneNumber = req.PhoneNumber;
                insurance.Email = req.Email;
                if (req.Sex != Insurance.FEMALE && req.Sex != Insurance.MALE)
                {
                    throw new ValidationException("Sex phải là FEMALE hoặc MALE");
                }
                insurance.Sex = req.Sex;
                insurance.Nation = req.Nation;
                insurance.Nationality = req.Nationality;
                insurance.FullName = req.FullName;
                insurance.PersonID = req.PersonID;
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
