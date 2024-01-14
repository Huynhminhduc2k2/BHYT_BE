using BHYT_BE.Common.AppSetting;
using BHYT_BE.Internal.Models;
using BHYT_BE.Internal.Repositories.UserRepo;
using BHYT_BE.Internal.Repository.InsuranceHistoryRepo;
using BHYT_BE.Internal.Repository.InsuranceRepo;
using System.ComponentModel.DataAnnotations;
using Insurance = BHYT_BE.Internal.Models.Insurance;

namespace BHYT_BE.Internal.Services.InsuranceService
{
    public class InsuranceService : IInsuranceService
    {
        private readonly AppSettings _appSettings;
        private readonly IInsuranceRepository _insuranceRepo;
        private readonly IInsuranceHistoryRepository _insuranceHistoryRepo;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<InsuranceService> _logger;
        public InsuranceService(AppSettings appSettings, IUserRepository userRepository, IInsuranceHistoryRepository insuranceHistoryRepo, IInsuranceRepository insuranceRepo, ILogger<InsuranceService> logger)
        {
            _appSettings = appSettings;
            _userRepository = userRepository;
            _insuranceHistoryRepo = insuranceHistoryRepo;
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
                    throw new ValidationException("Not found insurance");
                }
                if (insurance.Status != InsuranceStatus.PENDING)
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
                    Status = InsuranceStatus.CREATED,
                };

                insurance = _insuranceRepo.Create(insurance);

                UpdateInsurance(new InsuranceDTO
                {
                    InsuranceID = insurance.InsuranceID,
                    Status = InsuranceStatus.WAITING_PAYMENT,
                }, true, -1);

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
                    throw new ValidationException("Not found insurance");
                }
                if (insurance.Status != InsuranceStatus.PENDING)
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

        public void UpdateInsurance(InsuranceDTO req, bool isAdmin, int adminID)
        {
            try
            {
                Insurance insurance = _insuranceRepo.GetByID(req.InsuranceID);
                InsuranceStatus currentStatus = insurance.Status;
                if (currentStatus == InsuranceStatus.REJECTED || currentStatus == InsuranceStatus.ACCEPTED)
                {
                    throw new ValidationException("Insurance status is not pending");
                }
                User user = _userRepository.GetById(req.UserID) ?? throw new ValidationException("Not found user");
                insurance.InsuranceType = req.Type;
                User admin = new User
                {
                    Username = _appSettings.SystemEmail,
                };
                if (isAdmin)
                {
                    insurance.UserID = user.Id;
                    if (adminID != -1)
                    {
                        admin = _userRepository.GetById(adminID) ?? throw new Exception("Unthorization admin");
                    }
                    insurance.UpdatedBy = admin.Username;
                    insurance.Status = req.Status;
                    _insuranceRepo.Update(insurance);
                    if (req.Status != currentStatus)
                    {
                        _insuranceHistoryRepo.Create(new InsuranceHistory
                        {
                            InsuranceID = insurance.InsuranceID,
                            OldStatus = currentStatus,
                            NewStatus = insurance.Status,
                            Remark = "admin update status",
                            Email = admin.Username,
                            CreatedBy = admin.Username,
                            UpdatedBy = admin.Username,
                        });
                    }
                    
                } 
                else
                {
                    insurance.UpdatedBy = user.Username;
                    _insuranceRepo.Update(insurance);
                }
                
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
