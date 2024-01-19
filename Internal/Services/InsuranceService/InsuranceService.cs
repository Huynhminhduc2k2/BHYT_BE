﻿using BHYT_BE.Common.AppSetting;
using BHYT_BE.Internal.Adapter;
using BHYT_BE.Internal.Models;
using BHYT_BE.Internal.Repository.InsuranceHistoryRepo;
using BHYT_BE.Internal.Repository.InsuranceRepo;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

namespace BHYT_BE.Internal.Services.InsuranceService
{
    public class InsuranceService : IInsuranceService
    {
        private readonly AppSettings _appSettings;
        private readonly IEmailAdapter _emailAdapter;
        private readonly IInsuranceRepository _insuranceRepo;
        private readonly UserManager<User> _userManager;
        private readonly IInsuranceHistoryRepository _insuranceHistoryRepo;
        private readonly ILogger<InsuranceService> _logger;
        public InsuranceService(
            AppSettings appSettings,
            IEmailAdapter emailAdapter,
            UserManager<User> userManager,
            IInsuranceHistoryRepository insuranceHistoryRepo,
            IInsuranceRepository insuranceRepo,
            ILogger<InsuranceService> logger)
        {
            _appSettings = appSettings;
            _emailAdapter = emailAdapter;
            _userManager = userManager;
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
                User user = _userManager.FindByIdAsync(req.UserID).Result;
                insurance.InsuranceType = req.Type;
                User admin = new User
                {
                    UserName = _appSettings.SystemEmail,
                };
                if (isAdmin)
                {
                    if (user != null)
                    {
                        insurance.UserID = req.UserID;
                    }
                    if (adminID != "")
                    {
                        admin = _userManager.FindByIdAsync(adminID).Result ?? throw new Exception("Unthorization admin");
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
        public async Task<InsuranceDTO> RequestInsuranceAsync(RequestInsuraceDTO req)
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
            AddInsurance(new RegisterInsuraceDTO
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
    }
}
