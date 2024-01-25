using AutoMapper;
using BHYT_BE.Controllers.Types;
using BHYT_BE.Internal.Models;
using BHYT_BE.Internal.Services.InsuranceService;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace BHYT_BE.Controllers
{
    [ApiController]
    [Route("/v1/api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class InsuranceRequestController : ControllerBase
    {
        private readonly IInsuranceService _service;
        private readonly ILogger<InsuranceController> _logger;
        private readonly IMapper _mapper;
        public InsuranceRequestController(IInsuranceService insuranceService, ILogger<InsuranceController> logger, IMapper mapper)
        {
            _logger = logger;
            _service = insuranceService;
            _mapper = mapper;
        }
        [HttpGet("all")]
        [Authorize(Roles = Role.ADMIN)]
        public async Task<IActionResult> GetAllInsuranceRequest()
        {
            try
            {
                return StatusCode(StatusCodes.Status501NotImplemented, "Comming soon!");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (ValidationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpGet]
        [Authorize(Roles = Role.USER)]
        public async Task<IActionResult> GetAllInsuranceRequestByUser()
        {
            try
            {
                return StatusCode(StatusCodes.Status501NotImplemented, "Comming soon!");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (ValidationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpGet("detail")]
        public async Task<IActionResult> GetInsurancePaymentlByID(int insuranceRequestID)
        {
            try
            {
                return StatusCode(StatusCodes.Status501NotImplemented, "Comming soon!");

            }
            catch (ValidationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
        [HttpPost()]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(string))]
        public async Task<IActionResult> UserInsuranceRequest([FromBody] InsuranceRequestRequest req)
        {
            try
            {
                return StatusCode(StatusCodes.Status501NotImplemented, "Comming soon!");
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
        public IActionResult EditInsuranceRequest([FromBody] EditInsuranceRequestRequest req)
        {
            try
            {
                return StatusCode(StatusCodes.Status501NotImplemented, "Comming soon!");
            }
            catch (ValidationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, exception.Message);
            }
        }

        [HttpPost("private/accept")]
        [Authorize(Roles = Role.ADMIN)]
        public IActionResult AcceptInsurance([FromBody] int insuranceRequestID)
        {
            try
            {
                return StatusCode(StatusCodes.Status501NotImplemented, "Comming soon!");
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
        [HttpPost("private/reject")]
        [Authorize(Roles = Role.ADMIN)]
        public IActionResult RejectInsurance([FromBody] int insuranceRequestID, string remark)
        {
            try
            {
                return StatusCode(StatusCodes.Status501NotImplemented, "Comming soon!");
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
        [HttpGet("Payment")]
        public IActionResult GetInsuranceRequestPayment([FromBody] string insuranceRequestID)
        {
            try
            {
                return StatusCode(StatusCodes.Status501NotImplemented, "Comming soon!");
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
        [HttpPost("private/addInsuranceRequestPayment")]
        [Authorize(Roles = Role.ADMIN)]
        public IActionResult AddInsuranceRequestPayment([FromBody] AddInsuranceRequestPaymentRequest req)
        {
            try
            {
                return StatusCode(StatusCodes.Status501NotImplemented, "Comming soon!");
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
