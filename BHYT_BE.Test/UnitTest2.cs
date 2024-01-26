using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using BHYT_BE.Controllers;
using BHYT_BE.Controllers.Types;
using BHYT_BE.Internal.Adapter;
using BHYT_BE.Internal.Models;
using BHYT_BE.Internal.Services.UserService;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public class UserControllerTests
{
    // Test for GetAll action
    [Fact]
    public async Task GetAll_ReturnsOkResult()
    {
        // Arrange
        var userServiceMock = new Mock<IUserService>();
        var mapperMock = new Mock<IMapper>();
        var controller = new UserController(null, mapperMock.Object, null, userServiceMock.Object, null, null, null, null);

        // Act
        var result = await controller.GetAll();

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    // Test for GetById action
    [Fact]
    public void GetById_WithValidId_ReturnsOkResult()
    {
        // Arrange
        var userManagerMock = new Mock<UserManager<User>>(MockBehavior.Strict);
        userManagerMock.Setup(x => x.Users).Returns(new List<User> { new User { Id = Guid.NewGuid().ToString() } }.AsQueryable());
        var controller = new UserController(null, null, null, null, null, userManagerMock.Object, null, null);

        // Act
        var result = controller.GetById(Guid.NewGuid());

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    // Test for Register action with valid data
    [Fact]
    public async Task Register_WithValidModel_ReturnsOkResult()
    {
        // Arrange
        var userServiceMock = new Mock<IUserService>();
        userServiceMock.Setup(x => x.CreateUser(It.IsAny<UserDTO>(), It.IsAny<string>()))
                       .ReturnsAsync(IdentityResult.Success);

        var emailAdapterMock = new Mock<IEmailAdapter>();
        var loggerMock = new Mock<ILogger<UserController>>();

        var controller = new UserController(null, null, emailAdapterMock.Object, userServiceMock.Object, null, null, null, loggerMock.Object);

        // Act
        var result = await controller.Register(new Register { /* provide valid Register object */ });

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }

    // Test for Update action with valid data
    [Fact]
    public async Task Update_WithValidModel_ReturnsOkResult()
    {
        // Arrange
        var userManagerMock = new Mock<UserManager<User>>(MockBehavior.Strict);
        userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>()))
                       .ReturnsAsync(new User { Id = Guid.NewGuid().ToString() });

        userManagerMock.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                       .ReturnsAsync(IdentityResult.Success);

        var controller = new UserController(null, null, null, null, null, userManagerMock.Object, null, null);

        // Act
        var result = await controller.Update(Guid.NewGuid(), new UserDTO { /* provide valid UserDTO object */ });

        // Assert
        Assert.IsType<OkObjectResult>(result);
    }
}
