using BHYT_BE.Controllers.Types;
using BHYT_BE.Internal.Services.InsuranceService;
using Microsoft.AspNetCore.Mvc;
using NLog;
using System;
using System.ComponentModel.DataAnnotations;

namespace BHYT_BE.Controllers
{
    [ApiController]
    [Route("/v1/api/[controller]")]
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
                _service.AddInsurance(new RegisterInsuraceDTO
                {
                    Address = req.Address,
                    DOB = req.DOB,
                    Email = req.Email,
                    FullName = req.FullName,
                    Nation = req.Nation,
                    Nationality = req.Nationality,
                    PersonID = req.PersonID,
                    PhoneNumber = req.PhoneNumber,
                    Sex = req.Sex
                });

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
                _service.UpdateInsurance(new InsuranceDTO
                {
                    InsuranceID = req.InsuranceID,
                    Address = req.Address,
                    DOB = req.DOB,
                    Email = req.Email,
                    FullName = req.FullName,
                    Nation = req.Nation,
                    Nationality = req.Nationality,
                    PersonID = req.PersonID,
                    PhoneNumber = req.PhoneNumber,
                    Sex = req.Sex,
                    Status = req.Status
                }, true);

                
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
