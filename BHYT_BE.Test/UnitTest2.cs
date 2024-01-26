using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using BHYT_BE.Common.AppSetting;
using BHYT_BE.Controllers;
using BHYT_BE.Controllers.Types;
using BHYT_BE.Internal.Adapter;
using BHYT_BE.Internal.Models;
using BHYT_BE.Internal.Repositories.Data;
using BHYT_BE.Internal.Repositories.UserRepo;
using BHYT_BE.Internal.Repository.Data;
using BHYT_BE.Internal.Repository.InsuranceHistoryRepo;
using BHYT_BE.Internal.Repository.InsurancePaymentHistoryRepo;
using BHYT_BE.Internal.Repository.InsuranceRepo;
using BHYT_BE.Internal.Services.InsuranceService;
using BHYT_BE.Internal.Services.UserService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;

public class UserControllerTests
{
    UserController _controller;
    IUserService _service;
    ILogger<UserController> _logger;
    Mock<UserManager<User>> _userManagerMock;
    Mock<RoleManager<IdentityRole>> _roleManagerMock;
    IUserRepository _userRepo;
    UserDBContext _dbContext;

    public UserControllerTests()
    {
        var options = new DbContextOptionsBuilder<UserDBContext>()
            .UseInMemoryDatabase(databaseName: "TestDatabase")
            .Options;
        _dbContext = new UserDBContext(options);
        var roleStore = new RoleStore<IdentityRole>(_dbContext);

        _userManagerMock = new Mock<UserManager<User>>(
            new UserStore<User>(_dbContext),
            Mock.Of<IOptions<IdentityOptions>>(),
            Mock.Of<IPasswordHasher<User>>(),
            new List<IUserValidator<User>>(),
            new List<IPasswordValidator<User>>(),
            Mock.Of<ILookupNormalizer>(),
            Mock.Of<IdentityErrorDescriber>(),
            Mock.Of<IServiceProvider>(),
            Mock.Of<ILogger<UserManager<User>>>()
        );

        _roleManagerMock = new Mock<RoleManager<IdentityRole>>(
            roleStore,
            new List<IRoleValidator<IdentityRole>>(),
            Mock.Of<ILookupNormalizer>(),
            Mock.Of<IdentityErrorDescriber>(),
            Mock.Of<ILogger<RoleManager<IdentityRole>>>()
        );

        var loggerMock = new Mock<ILogger<UserService>>();
        var mapperMock = new Mock<IMapper>();
        _logger = new Mock<ILogger<UserController>>().Object;
        var memoryCacheMock = new Mock<IMemoryCache>();
        var appSettingsMock = new Mock<AppSettings>();
        var emailAdapterMock = new Mock<IEmailAdapter>();
        _userRepo = new UserRepository(_dbContext);
        var userService = new UserService(
            mapperMock.Object,
            _userManagerMock.Object,
            _userRepo,
            loggerMock.Object
        );
        _service = userService;
        _controller = new UserController(
            appSettingsMock.Object,
            mapperMock.Object,
            emailAdapterMock.Object,
            userService,
            memoryCacheMock.Object,
            _userManagerMock.Object,
            _roleManagerMock.Object,
            _logger
            );

        SeedTestUserData(_userManagerMock, _roleManagerMock);
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
                Id = "2426486c-ebbd-41a9-bb14-2d40e75a9d7b",
                UserName = "user1@example.com",
            },
            new User
            {
                Id = "ac13e5e4-d2a0-448c-a626-2fbd291b14a4",
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
    // Test for GetAll action
    [Fact]
    public async Task GetAll_ReturnsOkResult()
    {
        // Act
        var result = await _controller.GetAll();

        // Assert
        Assert.IsType<OkObjectResult>(result);
        // You might want to further assert the content of the result, depending on your implementation
    }

    // Test for GetById action
    [Fact]
    public void GetById_WithValidId_ReturnsOkResult()
    {
        // Arrange
        var userId = "2426486c-ebbd-41a9-bb14-2d40e75a9d7b"; // Use a valid user ID

        // Act
        var result = _controller.GetById(Guid.Parse(userId));

        // Assert
        Assert.IsType<OkObjectResult>(result);
        // You might want to further assert the content of the result, depending on your implementation
    }

    // Test for Register action with valid data
    [Fact]
    public async Task Register_WithValidModel_ReturnsOkResult()
    {
        // Arrange
        var registerRequest = new Register
        {
            FullName = "John Doe",
            PersonID = "123456789",
            PhoneNumber = "1234567890",
            DOB = new DateTime(1990, 1, 1), // Set a valid date of birth
            Address = "123 Main St, City",
            Email = "john.doe@example.com",
            Nation = "Vietnam",
            Nationality = "Vietnamese",
            Sex = "MALE",
            Password = "P@ssw0rd", // Set a valid password
            RePassword = "P@ssw0rd", // Set the same password for confirmation
            Roles = new List<string> { "USER" } // Set valid roles
        };

        // Act
        var result = await _controller.Register(registerRequest);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        // You might want to further assert the content of the result, depending on your implementation
    }

    // Test for Update action with valid data
    [Fact]
    public async Task Update_WithValidModel_ReturnsOkResult()
    {
        // Arrange
        var userId = "2426486c-ebbd-41a9-bb14-2d40e75a9d7b"; // Use a valid user ID
        var userDTO = new UserDTO
        {
            UserID = userId,
            UserName = "john.doe", // Set a valid user name
            Password = "NewP@ssw0rd", // Set a valid password
            Roles = new List<string> { "USER" }, // Set valid roles
            Email = "john.doe@example.com", // Set a valid email
            FullName = "John Doe",
            PersonID = "123456789",
            Address = "123 Main St, City",
            DOB = new DateTime(1990, 1, 1), // Set a valid date of birth
            Nation = "Vietnam",
            Nationality = "Vietnamese",
            Sex = "MALE",
            PhoneNumber = "1234567890", // Set a valid phone number
            OTP = "123456" // Set a valid OTP
        };
        // Act
        var result = await _controller.Update(Guid.Parse(userId), userDTO);

        // Assert
        Assert.IsType<OkObjectResult>(result);
        // You might want to further assert the content of the result, depending on your implementation
    }
}
