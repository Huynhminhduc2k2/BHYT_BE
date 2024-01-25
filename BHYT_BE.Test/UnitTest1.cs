using System.Security.Claims;
using AutoMapper;
using BHYT_BE.Common.AppSetting;
using BHYT_BE.Controllers;
using BHYT_BE.Internal.Adapter;
using BHYT_BE.Internal.Models;
using BHYT_BE.Internal.Repository.InsuranceHistoryRepo;
using BHYT_BE.Internal.Repository.InsurancePaymentHistoryRepo;
using BHYT_BE.Internal.Repository.InsuranceRepo;
using BHYT_BE.Internal.Services.InsuranceService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace BHYT_BE.Test
{


    public class InsuranceControllerTests
    {
        InsuranceController _controller;
        IInsuranceService _service;
        ILogger<InsuranceController> _logger;
        IMapper _mapper;
        public InsuranceControllerTests()
        {
            var appSettingsMock = new Mock<AppSettings>();
            var emailAdapterMock = new Mock<IEmailAdapter>();
            var userManagerMock = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var roleManagerMock = new Mock<RoleManager<IdentityRole>>(Mock.Of<IRoleStore<IdentityRole>>(), null, null, null, null);
            var insuranceHistoryRepoMock = new Mock<IInsuranceHistoryRepository>();
            var insurancePaymentHistoryRepoMock = new Mock<IInsurancePaymenHistoryRepository>();
            var insuranceRepoMock = new Mock<IInsuranceRepository>();
            var loggerMock = new Mock<ILogger<InsuranceService>>();
            var mapperMock = new Mock<IMapper>();

            var insuranceService = new InsuranceService(
                appSettingsMock.Object,
                emailAdapterMock.Object,
                mapperMock.Object,
                userManagerMock.Object,
                roleManagerMock.Object,
                insuranceHistoryRepoMock.Object,
                insurancePaymentHistoryRepoMock.Object,
                insuranceRepoMock.Object,
                loggerMock.Object
            );
            _service = insuranceService;
            _mapper = mapperMock.Object;
            _logger = new Mock<ILogger<InsuranceController>>().Object;
            _controller = new InsuranceController(_service, _logger, _mapper);
        }
        [Fact]
        public async Task GetAllInsurance_Returns_OkResult()
        {
            var result = await _controller.GetAllInsurance(null);

            // Assert
            Assert.IsType<ObjectResult>(result);
        }

        [Fact]
        public async Task GetAllInsuranceByUser_Returns_NotFoundObjectResult()
        {
            // Mocking HttpContext
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.NameIdentifier, "121"),
            }));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            // Act
            var result = await _controller.GetAllInsuranceByUser();
            System.Console.WriteLine(result);
            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        // Add more unit tests for other methods in InsuranceController
        [Fact]
        public async Task GetAllInsuranceByUser_Returns_UnauthorizedObjectResult()
        {
            // Mocking HttpContext
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.NameIdentifier, ""),
            }));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            // Act
            var result = await _controller.GetAllInsuranceByUser();
            System.Console.WriteLine(result);
            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result);
        }
    }

}