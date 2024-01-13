using BHYT_BE.Controllers.Types;
using BHYT_BE.Internal.Models;
using BHYT_BE.Internal.Services.InsuranceService;
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
        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        public IActionResult RegisterInsurance([FromBody] RegisterInsurance req)
        {
            try
            {
                _logger.LogError("Invalid request body");

                if (!ModelState.IsValid)
                {
                    _logger.LogError("Invalid request body");
                    return BadRequest("Invalid request body");
                }
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
                return StatusCode(StatusCodes.Status500InternalServerError,exception.Message) ;
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
                }, true); ;

                
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
        public IActionResult AcceptInsurance([FromBody] int insuranceID)
        {
            try
            {
                bool accepted = _service.AcceptInsurance(insuranceID);
                if (!accepted)
                {
                    return NotFound("Not found insurance");
                }
                return Ok("Accepted");
            } 
            catch (Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exception.Message);
            }
        }
        [HttpPost("private/delince")]
        public IActionResult RejectInsurance([FromBody] int insuranceID)
        {
            try
            {
                bool rejected = _service.RejectInsurance(insuranceID);
                if (!rejected)
                {
                    return NotFound("Not found insurance");
                }
                return Ok("Accepted");
            }
            catch (Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exception.Message);
            }
        }
    }
}
