﻿using AutoMapper;
using BHYT_BE.Common.AppSetting;
using BHYT_BE.Internal.Adapter;
using BHYT_BE.Internal.Models;
using BHYT_BE.Internal.Repository.InsurancePaymentHistoryRepo;
using BHYT_BE.Internal.Repository.InsuranceHistoryRepo;
using BHYT_BE.Internal.Repository.InsuranceRepo;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Routing.Patterns;
using BHYT_BE.Internal.Services.UserService;
using Stripe.Radar;
using System.Net.WebSockets;

namespace BHYT_BE.Internal.Services.InsuranceService
{
    public class InsuranceService : IInsuranceService
    {
        private readonly AppSettings _appSettings;
        private readonly IEmailAdapter _emailAdapter;
        private readonly IInsuranceRepository _insuranceRepo;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManger;
        private readonly IInsuranceHistoryRepository _insuranceHistoryRepo;
        private readonly IInsurancePaymenHistoryRepository _insurancePaymentHistoryRepo;
        private readonly ILogger<InsuranceService> _logger;
        private readonly IMapper _mapper;
        private AppSettings object1;
        private IEmailAdapter object2;
        private IMapper object3;
        private UserManager<User> object4;
        private RoleManager<IdentityRole> object5;
        private IInsuranceHistoryRepository object6;
        private IInsurancePaymenHistoryRepository object7;
        private ILogger<InsuranceService> object8;

        public InsuranceService(
            AppSettings appSettings,
            IEmailAdapter emailAdapter,
            IMapper mapper,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManger,
            IInsuranceHistoryRepository insuranceHistoryRepo,
            IInsurancePaymenHistoryRepository insurancePaymentHistoryRepo,
            IInsuranceRepository insuranceRepo,
            ILogger<InsuranceService> logger)
        {
            _appSettings = appSettings;
            _emailAdapter = emailAdapter;
            _userManager = userManager;
            _roleManger = roleManger;
            _insuranceHistoryRepo = insuranceHistoryRepo;
            _insurancePaymentHistoryRepo = insurancePaymentHistoryRepo;
            _insuranceRepo = insuranceRepo;
            _logger = logger;
            _mapper = mapper;
        }

        public InsuranceService(AppSettings object1, IEmailAdapter object2, IMapper object3, UserManager<User> object4, RoleManager<IdentityRole> object5, IInsuranceHistoryRepository object6, IInsurancePaymenHistoryRepository object7, ILogger<InsuranceService> object8)
        {
            this.object1 = object1;
            this.object2 = object2;
            this.object3 = object3;
            this.object4 = object4;
            this.object5 = object5;
            this.object6 = object6;
            this.object7 = object7;
            this.object8 = object8;
        }

        public bool AcceptInsurance(int insuranceID)
        {
            try
            {
                Insurance insurance = _insuranceRepo.GetByID(insuranceID).Result;
                if (insurance == null)
                {
                    throw new ValidationException("Not found insurance");
                }
                if (insurance.Status != InsuranceStatus.PENDING)
                {
                    return false;
                }
                insurance.Status = InsuranceStatus.ACCEPTED;
                insurance = _insuranceRepo.UpdateAsync(insurance).Result;

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while accepting insurance");
                throw; // Rethrow the exception after logging
            }
        }

        public async Task<InsuranceDTO> AddInsurance(RegisterInsuraceDTO req)
        {
            try
            {
                var result = await _userManager.FindByIdAsync(req.UserID);
                if (result == null)
                {
                    throw new ValidationException("Not found user");
                }
                Insurance insurance = new Insurance
                {
                    UserID = req.UserID,
                    InsuranceType = req.Type,
                    Status = InsuranceStatus.CREATED,
                    IsAutoRenewal = false,
                    PremiumAmount = new InsurancePrice(req.Type).Price,
                    CreatedBy = result.UserName,
                };

                insurance = _insuranceRepo.Create(insurance);

                await UpdateInsurance(new InsuranceDTO
                {
                    InsuranceID = insurance.InsuranceID,
                    Status = InsuranceStatus.WAITING_PAYMENT,
                    Type = insurance.InsuranceType,
                    UserID = "",
                }, true, null);
                var insuranceDTO = _mapper.Map<InsuranceDTO>(insurance);
                // Additional logic if needed
                return insuranceDTO;
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
                Insurance insurance = _insuranceRepo.GetByID(insuranceID).Result;
                if (insurance == null)
                {
                    throw new ValidationException("Not found insurance");
                }
                if (insurance.Status != InsuranceStatus.PENDING)
                {
                    return false;
                }
                insurance.Status = InsuranceStatus.REJECTED;
                insurance = _insuranceRepo.UpdateAsync(insurance).Result;

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while accepting insurance");
                throw; // Rethrow the exception after logging
            }
        }

        public async Task<List<InsuranceDTO>> GetAllInsurancesAsync(string? userID)
        {
            List<Insurance> insurances;
            List<InsuranceDTO> insuranceDTOs;
            if (string.IsNullOrEmpty(userID))
            {
                insurances = await _insuranceRepo.GetAll();   
            } 
            else
            {
                _ = await _userManager.FindByIdAsync(userID) ?? throw new ValidationException("Not found user");
                insurances = await _insuranceRepo.GetInsuranceByUserID(userID);
            }
            insuranceDTOs = _mapper.Map<List<InsuranceDTO>>(insurances);
            return insuranceDTOs;
        }

        public async Task<InsuranceDTO> GetInsuranceByID(int id)
        {
            var insurance = await _insuranceRepo.GetByID(id);
            if (insurance == null)
            {
                throw new ValidationException("Not found insurance");
            }
            var insuranceDTO = _mapper.Map<InsuranceDTO>(insurance);
            return insuranceDTO;
        }
        public async Task<InsuranceDetailDTO> GetInsuranceDetail(int id)
        {
            var insurance = await _insuranceRepo.GetByID(id);
            if (insurance == null)
            {
                throw new ValidationException("Not found insurance");
            }
            var insuranceDTO = _mapper.Map<InsuranceDTO>(insurance);

            var insuranceHistories = await _insuranceHistoryRepo.GetInsuranceHistoriesByInsuranceID(id);
            var insuranceHistoriesDTO = _mapper.Map<List<InsuranceHistoryDTO>>(insuranceHistories);

            var insurancePaymentHistories = await _insurancePaymentHistoryRepo.GetInsurancePaymentHistoriesByInsuranceID(id);
            var insurancePaymentHistoriesDTO = _mapper.Map<List<InsurancePaymentHistoryDTO>>(insurancePaymentHistories);
            return new InsuranceDetailDTO
            {
                Insurance = insuranceDTO,
                History = insuranceHistoriesDTO,
                PaymentHistory = insurancePaymentHistoriesDTO
            };
        }

        public async Task UpdateInsurance(InsuranceDTO req, bool isAdmin, string? userId)
        {
            try
            {
                Insurance insurance = _insuranceRepo.GetByID(req.InsuranceID).Result;
                InsuranceStatus currentStatus = insurance.Status;
                InsuranceType currentType = insurance.InsuranceType;
                decimal currentPrice = insurance.PremiumAmount;
                User updateUser = null;
                if (!string.IsNullOrEmpty(req.UserID))
                {
                    updateUser = await _userManager.FindByIdAsync(req.UserID) ?? throw new ValidationException("User not found"); ;
                }
                User user = new User
                {
                    UserName = _appSettings.SystemEmail,
                };
                if (userId != null)
                {
                    user = await _userManager.FindByIdAsync(userId) ?? throw new UnauthorizedAccessException("Unauthorization");
                }
                insurance.InsuranceType = req.Type;
                if (req.Type != currentType)
                {
                    insurance.PremiumAmount = new InsurancePrice(req.Type).Price;
                }
                if (req.Type != currentType && (insurance.Status == InsuranceStatus.PAID 
                    || insurance.Status == InsuranceStatus.PENDING
                    || insurance.Status == InsuranceStatus.REJECTED
                    || insurance.Status == InsuranceStatus.ACCEPTED))
                {
                    insurance.PremiumAmount = new InsurancePrice(req.Type).Price - currentPrice;
                    insurance.Status = InsuranceStatus.WAITING_PAYMENT;
                }
                if (isAdmin)
                {
                    if (updateUser != null)
                    {
                        insurance.UserID = req.UserID;
                    }
                    insurance.UpdatedBy = user.UserName;
                    insurance.Status = req.Status;
                    await _insuranceRepo.UpdateAsync(insurance);

                }
                else
                {
                    insurance.UpdatedBy = user.Email;
                    var result = await _insuranceRepo.UpdateAsync(insurance);
                    
                }
                if (req.Status != currentStatus)
                {
                    _insuranceHistoryRepo.Create(new InsuranceHistory
                    {
                        InsuranceID = insurance.InsuranceID,
                        OldStatus = currentStatus,
                        NewStatus = insurance.Status,
                        Remark = "admin update status",
                        Email = user.UserName,
                        CreatedBy = user.UserName,
                        UpdatedBy = user.UserName,
                    });
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
        private static string GenerateRandomPassword(int length)
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

            using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
            {
                byte[] randomBytes = new byte[length];
                rng.GetBytes(randomBytes);

                char[] chars = new char[length];
                for (int i = 0; i < length; i++)
                {
                    chars[i] = validChars[randomBytes[i] % validChars.Length];
                }

                return new string(chars);
            }
        }
        public async Task<InsuranceDTO> RequestInsuranceFromNewUserAsync(RequestInsuraceDTO req)
        {
            var password = GenerateRandomPassword(8);
            
            var result = await _userManager.CreateAsync(new User
            {
                UserName = req.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                Address = req.Address,
                FullName = req.FullName,
                DOB = req.DOB,
                Email = req.Email,
                Nation = req.Nation,
                Nationality = req.Nationality,
                PersonID = req.PersonID,
                PhoneNumber = req.PhoneNumber,
                Sex = req.Sex,
            });
            if (!result.Succeeded)
            {
                _logger.LogError("Error while creating user");
                return null;
            }
            var user = await _userManager.FindByEmailAsync(req.Email);
            if (user == null)
            {
                _logger.LogError("Not found user");
                return null;
            }
            var roleResult = await _userManager.AddToRoleAsync(user, Role.USER);
            if (!roleResult.Succeeded)
            {
                _logger.LogError("Error while creating user");
                return null;
            }
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var urlConfirm = _appSettings.ClientURL + "api/User/Confirm?token=" + token + "&userId=" + user.Id;
            await AddInsurance(new RegisterInsuraceDTO
            {
                UserID = user.Id,
                Type = req.InsuranceType,
            });
            await _emailAdapter.SendEmailAsync(
                email: user.Email,
                subject: "BHYT - Xác nhận đăng ký bảo hiểm y tế",
                body: $"<p><strong>Dịch vụ đã đăng ký</strong></p>" +
                      $"<p><strong>Tài khoản:</strong> {user.UserName}</p>" +
                      $"<p><strong>Mật khẩu:</strong> {password}</p>" +
                      $"<p><strong>Gói dịch vụ:</strong> {req.InsuranceType}</p>" +
                      $"<p><strong>Link xác nhận tài khoản:</strong> <a href='{urlConfirm}'>Xác nhận</a></p>" +
                      "<p>Vui lòng xác nhận tài khoản để thanh toán và đổi mật khẩu khi đăng nhập</p>",
                isHTML: true
            );

            return new InsuranceDTO
            {
                UserID = user.Id,
                Type = req.InsuranceType,
            };
        }

        public async Task<List<InsuranceDTO>> GetAllInsurancesByUserAsync(string? userID)
        {
            if (string.IsNullOrEmpty(userID))
            {
                throw new UnauthorizedAccessException("Unauthorization");
            }
            List<Insurance> insurances;
            List<InsuranceDTO> insuranceDTOs;
            var _ = await _userManager.FindByIdAsync(userID) ?? throw new ValidationException("Not found user");

            insurances = await _insuranceRepo.GetInsuranceByUserID(userID);
            _logger.LogInformation("Insurance created successfully {insurances}", insurances);
            insuranceDTOs = _mapper.Map<List<InsuranceDTO>>(insurances);
            return insuranceDTOs;
        }
    }
}
