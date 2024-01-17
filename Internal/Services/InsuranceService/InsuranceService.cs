/*using BHYT_BE.Common.AppSetting;
using BHYT_BE.Internal.Adapter;
using BHYT_BE.Internal.Models;
using BHYT_BE.Internal.Repositories.UserRepo;
using BHYT_BE.Internal.Repository.InsuranceHistoryRepo;
using BHYT_BE.Internal.Repository.InsuranceRepo;
using Stripe;
using System.ComponentModel.DataAnnotations;
using Insurance = BHYT_BE.Internal.Models.Insurance;

namespace BHYT_BE.Internal.Services.InsuranceService
{
    public class InsuranceService : IInsuranceService
    {
        private readonly AppSettings _appSettings;
        private readonly IInsuranceRepository _insuranceRepo;
        private readonly EmailAdapter _emailAdapter;
        private readonly IInsuranceHistoryRepository _insuranceHistoryRepo;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<InsuranceService> _logger;
        public InsuranceService(
            AppSettings appSettings,
            EmailAdapter emailAdapter,
            IUserRepository userRepository, 
            IInsuranceHistoryRepository insuranceHistoryRepo, 
            IInsuranceRepository insuranceRepo, 
            ILogger<InsuranceService> logger)
        {
            _appSettings = appSettings;
            _emailAdapter = emailAdapter;
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
                }, true, "");

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

        public void UpdateInsurance(InsuranceDTO req, bool isAdmin, string adminID)
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
                    UserName = _appSettings.SystemEmail,
                };
                if (isAdmin)
                {
                    insurance.UserID = user.Id;
                    if (adminID != "")
                    {
                        admin = _userRepository.GetById(adminID) ?? throw new Exception("Unthorization admin");
                    }
                    insurance.UpdatedBy = admin.UserName;
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
                            Email = admin.UserName,
                            CreatedBy = admin.UserName,
                            UpdatedBy = admin.UserName,
                        });
                    }
                    
                } 
                else
                {
                    insurance.UpdatedBy = user.Email;
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

        public InsuranceDTO RequestInsurance(RequestInsuraceDTO req)
        {
            var user = new User
            {
                UserName = req.Email,
                PasswordHash = "123",
                Roles = { new Role { Name = "User" } },
            };
            _userRepository.Create(user);
            AddInsurance(new RegisterInsuraceDTO
            {
                UserID = user.Id,
                Type = req.InsuranceType,
            }); ;
            _emailAdapter.SendEmail(
                user.UserName,
                "BHYT - Xác nhận đăng ký bảo hiểm y tế",
                $"Dịch vụ đã đăng ký\n" +
                $"Tài khoản: {user.UserName}\n" +
                $"Mật khẩu: {user.PasswordHash}\n" +
                $"Gói dịch vụ {req.InsuranceType}\n" +
                $"Vui lòng xác nhận tài khoản để thanh toán và đổi mật khẩu khi đăng nhập", 
                false);
            return new InsuranceDTO
            {
                UserID = user.Id,
                Type = req.InsuranceType,
            };
        }
    }
}
*/