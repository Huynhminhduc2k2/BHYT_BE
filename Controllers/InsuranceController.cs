using BHYT_BE.Controllers.Types;
using BHYT_BE.Internal.Models;
using BHYT_BE.Internal.Services.InsuranceService;
using BHYT_BE.Internal.Services.UserService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NLog;
using System;
using System.ComponentModel.DataAnnotations;

namespace BHYT_BE.Controllers
{
    [ApiController]
    [Route("/v1/api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class InsuranceController : ControllerBase
    {
        private readonly IInsuranceService _service;
        private readonly ILogger<InsuranceController> _logger;
        public InsuranceController(IInsuranceService insuranceService, ILogger<InsuranceController> logger)
        {
            _logger = logger;
            _service = insuranceService;
            _logger.LogInformation(1, "NLog injected into HomeController");
        }
        [HttpPost("request")]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        [AllowAnonymous]
        public async Task<IActionResult> RequestInsuranceAsync([FromBody] RequestInsurance req)
        {
            try
            {
                _logger.LogError("Invalid request body");

                if (!ModelState.IsValid)
                {
                    // Log each model state error
                    var errList = new List<string>();
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        _logger.LogError($"ModelState error: {error.ErrorMessage}");
                        errList.Add(error.ErrorMessage);
                    }
                    return BadRequest($"Invalid request body, err={errList.ToList()}");
                }
                InsuranceType insuranceType;
                if (!Enum.TryParse<InsuranceType>(req.InsuranceType, out insuranceType))
                {
                    _logger.LogError("Invalid insurance type");
                    return BadRequest("Invalid insurance type");
                }
                // TODO: add new user from request

                //TODO: Send notification to email

                //TODO: Add new register insurance from new user
                await _service.RequestInsuranceAsync(new RequestInsuraceDTO
                {
                    Email = req.Email,
                    Address = req.Address,
                    DOB = req.DOB,  
                    FullName = req.FullName,
                    Nation = req.Nation,
                    Nationality = req.Nationality,
                    PersonID = req.PersonID,
                    PhoneNumber = req.PhoneNumber,
                    Sex = req.Sex,
                    InsuranceType = insuranceType,
                });
                
                return Ok("Registration successful, please check your email register insurance");
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exception.Message);
            }
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        public IActionResult RegisterInsurance([FromBody] RegisterInsurance req)
        {
            try
            {
                _logger.LogError("Invalid request body");
                InsuranceType insuranceType;
                if (!Enum.TryParse<InsuranceType>(req.InsuranceType, out insuranceType))
                {
                    _logger.LogError("Invalid insurance type");
                    return BadRequest("Invalid insurance type");
                }
                //TODO: Get user id from token or orther 
                _service.AddInsurance(new RegisterInsuraceDTO
                {
                    UserID = req.UserID,
                    Type = insuranceType,
                }); ;

                return Ok("Registration successful");
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exception.Message);
            }
        }

        [HttpPost("edit")]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        public IActionResult EditRegisterInsurance([FromBody] EditInsuranceRequest req)
        {
            try
            {
                _logger.LogError("Invalid request body");
                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid request body");
                    return BadRequest("Invalid request body");
                }
                // TODO: check admin
                InsuranceType insuranceType;
                if (!Enum.TryParse<InsuranceType>(req.InsuranceType, out insuranceType))
                {
                    _logger.LogError("Invalid insurance type");
                    return BadRequest("Invalid insurance type");
                }
                InsuranceStatus insuranceStatus;
                if (!Enum.TryParse<InsuranceStatus>(req.Status, out insuranceStatus))
                {
                    _logger.LogError("Invalid insurance status");
                    return BadRequest("Invalid insurance status");
                }   
                _service.UpdateInsurance(new InsuranceDTO
                {
                    InsuranceID = req.InsuranceID,
                    UserID = req.UserID,
                    Type = insuranceType,
                    Status = insuranceStatus,
                }, req.IsAdmin, req.AdminID);

                
                return Ok("Registration successful");
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exception.Message);
            }
        }

        [HttpPost("private/accept")]
        [Authorize(Roles = "Admin")]
        public IActionResult AcceptInsurance([FromBody] int insuranceID)
        {
            try
            {
                bool accepted = _service.AcceptInsurance(insuranceID);
                if (!accepted)
                {
                    return BadRequest("current status not pending");
                }
                return Ok("Accepted");
            }
            catch (ValidationException ve)
            {
                return NotFound(ve.Message);
            }
            catch (Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exception.Message);
            }
        }
        [HttpPost("private/delince")]
        [Authorize(Roles = "Admin")]
        public IActionResult RejectInsurance([FromBody] int insuranceID)
        {
            try
            {
                bool rejected = _service.RejectInsurance(insuranceID);
                if (!rejected)
                {
                    return BadRequest("current status not pending");
                }
                return Ok("Accepted");
            }
            catch (ValidationException ve)
            {
                return NotFound(ve.Message);
            }
            catch (Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exception.Message);
            }
        }
    }
}
