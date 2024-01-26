using System.Security.Claims;
using AutoMapper;
using BHYT_BE.Common.AppSetting;
using BHYT_BE.Controllers;
using BHYT_BE.Controllers.Types;
using BHYT_BE.Internal.Adapter;
using BHYT_BE.Internal.Models;
using BHYT_BE.Internal.Repository.Data;
using BHYT_BE.Internal.Repository.InsuranceHistoryRepo;
using BHYT_BE.Internal.Repository.InsurancePaymentHistoryRepo;
using BHYT_BE.Internal.Repository.InsuranceRepo;
using BHYT_BE.Internal.Services.InsuranceService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
        InsuranceDBContext _dbContext;
        InsuranceHistoryDBContext _insuranceHistoryDbContext;
        InsurancePaymentHistoryDBContext _insurancePaymentHistoryDBContext;
        public InsuranceControllerTests()
        {
            var options = new DbContextOptionsBuilder<InsuranceDBContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _dbContext = new InsuranceDBContext(options);
            _insuranceHistoryDbContext = new InsuranceHistoryDBContext(new DbContextOptionsBuilder<InsuranceHistoryDBContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options);
            _insurancePaymentHistoryDBContext = new InsurancePaymentHistoryDBContext(new DbContextOptionsBuilder<InsurancePaymentHistoryDBContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options);

            var insuranceRepo = new InsuranceRepository(_dbContext);
            var insuranceHistoryRepo = new InsuranceHistoryRepository(_insuranceHistoryDbContext);
            var insurancePaymentHistoryRepo = new InsurancePaymenHistoryRepository(_insurancePaymentHistoryDBContext);

            var userManagerMock = new Mock<UserManager<User>>(Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            var roleManagerMock = new Mock<RoleManager<IdentityRole>>(Mock.Of<IRoleStore<IdentityRole>>(), null, null, null, null);
            var appSettingsMock = new Mock<AppSettings>();
            var emailAdapterMock = new Mock<IEmailAdapter>();
            var loggerMock = new Mock<ILogger<InsuranceService>>();
            var mapperMock = new Mock<IMapper>();

            
            var insuranceService = new InsuranceService(
                appSettingsMock.Object,
                emailAdapterMock.Object,
                mapperMock.Object,
                userManagerMock.Object,
                roleManagerMock.Object,
                insuranceHistoryRepo,
                insurancePaymentHistoryRepo,
                insuranceRepo,
                loggerMock.Object
            );
            _service = insuranceService;
            _mapper = mapperMock.Object;
            _logger = new Mock<ILogger<InsuranceController>>().Object;
            _controller = new InsuranceController(_service, _logger, _mapper);

            SeedTestUserData(userManagerMock, roleManagerMock);
            SeedTestData();
        }
        private void SeedTestUserData(Mock<UserManager<User>> userManagerMock, Mock<RoleManager<IdentityRole>> roleManagerMock)
        {
            // Add sample roles to the RoleManager for testing
            var roles = new List<IdentityRole>
        {
            new IdentityRole { Name = "ADMIN", NormalizedName = "ADMIN" },
            new IdentityRole { Name = "USER", NormalizedName = "USER" }
            // Add more sample roles as needed
        };

            foreach (var role in roles)
            {
                _ = roleManagerMock.Setup(r => r.FindByNameAsync(role.Name)).ReturnsAsync(role);
            }

            // Add sample users to the UserManager for testing
            var users = new List<User>
        {
            new User
            {
                Id = "1",
                UserName = "user1@example.com",
            },
            new User
            {
                Id = "2",
                UserName = "user2@example.com",
            },
            // Add more sample users as needed
        };

            foreach (var user in users)
            {
                userManagerMock.Setup(u => u.FindByIdAsync(user.Id)).ReturnsAsync(user);

                // Assign roles to users
                userManagerMock.Setup(u => u.AddToRoleAsync(user, "USER"));
                if (user.Id == "1")
                {
                    userManagerMock.Setup(u => u.AddToRoleAsync(user, "ADMIN"));
                }
            }
        }

        private void SeedTestData()
        {
            // Add sample data to the in-memory database for testing
            var insurances = new List<Insurance>
            {
                new Insurance
                {
                    UserID = "1",
                    InsuranceType = InsuranceType.STANDARD,
                    Status = InsuranceStatus.CREATED,
                    PremiumAmount = 100.00m,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddYears(1),
                    LastPaymentDate = DateTime.Now,
                    IsAutoRenewal = true,
                    CreatedBy = "user1@example.com",
                    UpdatedBy = null
                },
                new Insurance
                {
                    UserID = "2",
                    InsuranceType = InsuranceType.ADVANDCE,
                    Status = InsuranceStatus.WAITING_PAYMENT,
                    PremiumAmount = 200.00m,
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddYears(1),
                    LastPaymentDate = DateTime.Now,
                    IsAutoRenewal = false,
                    CreatedBy = "user2@example.com",
                    UpdatedBy = null
                },
                // Add more sample data as needed
            };

            _dbContext.Insurances.AddRange(insurances);
            _dbContext.SaveChanges();

            // Seed InsurancePaymentHistory data
            var paymentHistories = new List<InsurancePaymentHistory>
            {
                new InsurancePaymentHistory
                {
                    InsuranceID = 1, // Assuming you have an Insurance record with ID 1
                    AmountPaid = 50.00m, // Example amount
                    PaymentDate = DateTime.Now.AddDays(-5), // Example payment date
                    PaymentMethod = InsurancePaymentMethod.BANK_STRANFER // Example payment method
                },
                // Add more payment history data as needed
            };

            _insurancePaymentHistoryDBContext.InsurancePaymentHistories.AddRange(paymentHistories);
            _insurancePaymentHistoryDBContext.SaveChanges();

            // Seed InsuranceHistory data
            var insuranceHistories = new List<InsuranceHistory>
            {
                new InsuranceHistory
                {
                    InsuranceID = 1, // Assuming you have an Insurance record with ID 1
                    OldStatus = InsuranceStatus.CREATED, // Example old status
                    NewStatus = InsuranceStatus.PAID, // Example new status
                    Remark = "Payment completed", // Example remark
                    Email = "user1@example.com", // Example email
                },
                // Add more insurance history data as needed
            };

            _insuranceHistoryDbContext.InsuranceHistories.AddRange(insuranceHistories);
            _insuranceHistoryDbContext.SaveChanges();
        }
        [Fact]
        public async Task GetAllInsurance_Returns_OkResult()
        {
            var result = await _controller.GetAllInsurance(null);
            // Assert
            Assert.IsType<OkObjectResult>(result);
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

        [Fact]
        public async Task GetAllInsuranceByUser_WithValidUser_Returns_ObjectResult()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            }));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            // Act
            var result = await _controller.GetAllInsuranceByUser();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            // Add more specific assertions based on your expected behavior
        }
        [Fact]
        public async Task GetAllInsuranceByUser_WithValidUser_Returns_UnauhtorizeObjectResult()
        {
            // Arrange
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

            // Assert
            var unauthorizedObjectResult = Assert.IsType<UnauthorizedObjectResult>(result);
            Assert.Equal(StatusCodes.Status401Unauthorized, unauthorizedObjectResult.StatusCode);
            // Add more specific assertions based on your expected behavior
        }
        [Theory]
        [InlineData("121", typeof(NotFoundObjectResult), StatusCodes.Status404NotFound)]
        [InlineData("", typeof(UnauthorizedObjectResult), StatusCodes.Status401Unauthorized)]
        public async Task GetAllInsuranceByUser_Returns_CorrectResult(string userId, Type expectedResultType, int expectedStatusCode)
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
            new Claim(ClaimTypes.NameIdentifier, userId),
            }));
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            // Act
            var result = await _controller.GetAllInsuranceByUser();

            // Assert
            Assert.IsType(expectedResultType, result);
            Assert.Equal(expectedStatusCode, (result as ObjectResult)?.StatusCode);
            // Add more specific assertions based on your expected behavior
        }
        [Fact]
        public async Task GetAllInsurance_ByAdmin_Returns_OkResult()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "2"), // Admin user ID
                new Claim(ClaimTypes.Role, "ADMIN") // User has the ADMIN role
            }));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            // Act
            var result = await _controller.GetAllInsurance(null);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            // Add more specific assertions based on your expected behavior
        }
        [Fact]
        public async Task GetInsuranceDetail_ByAdmin_Returns_OkResult()
        {
            // Arrange
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "2"), // Admin user ID
                new Claim(ClaimTypes.Role, "ADMIN") // User has the ADMIN role
            }));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };

            // Assuming you have an insurance ID, you want to test the detail for
            int insuranceIdToTest = 1;

            // Act
            var result = await _controller.GetInsuranceDetailByID(insuranceIdToTest);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
            // Add more specific assertions based on your expected behavior
            // You can deserialize the response to get the actual data returned by the API
            var insuranceDetailResponse = (InsuranceDetailResponse)okResult.Value;
            // Add assertions based on the expected data in the response
            Assert.NotNull(insuranceDetailResponse);
            // ...
        }
        [Fact]
        public async Task GetInsuranceDetail_WithInvalidID_Returns_NotFoundResult()
        {
            // Arrange
            var invalidInsuranceID = 999; // Assuming there's no insurance with ID 999

            // Act
            var result = await _controller.GetInsuranceDetailByID(invalidInsuranceID);

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
            // You can add more assertions based on your expected behavior
        }
        [Fact]
        public async Task GetInsuranceDetail_ByNonAdminUser_Returns_OkResult()
        {
            // Arrange
            var nonAdminUser = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"), // Non-admin user ID
                new Claim(ClaimTypes.Role, "USER") // User does not have the ADMIN role
            }));

            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext() { User = nonAdminUser }
            };

            // Assuming you have an insurance ID, you want to test the detail for
            int insuranceIdToTest = 1;

            // Act
            var result = await _controller.GetInsuranceDetailByID(insuranceIdToTest);

            // Assert
            Assert.IsType<OkObjectResult>(result);
            // You can add more assertions based on your expected behavior
        }

    }
}