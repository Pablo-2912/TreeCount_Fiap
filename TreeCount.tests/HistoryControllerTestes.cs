using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TreeCount.API.Controllers;
using TreeCount.Application.DTOs;
using TreeCount.Application.Interfaces;
using TreeCount.Application.ViewModels;
using TreeCount.Domain.Enums;
using TreeCount.Repository.Identity;
using Xunit;

public class HistoryControllerTests
{
    private readonly Mock<IHistoryService> _historyServiceMock;
    private readonly Mock<UserManager<UserModel>> _userManagerMock;
    private readonly HistoryController _controller;

    public HistoryControllerTests()
    {
        _historyServiceMock = new Mock<IHistoryService>();

        var store = new Mock<IUserStore<UserModel>>();
        _userManagerMock = new Mock<UserManager<UserModel>>(
            store.Object, null, null, null, null, null, null, null, null);

        _controller = new HistoryController(_historyServiceMock.Object, _userManagerMock.Object);

        // Simula usuário autenticado no controller
        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "123"),
            new Claim(ClaimTypes.Name, "user@example.com"),
        }, "mock"));

        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = new DefaultHttpContext() { User = user }
        };
    }

    [Fact]
    public async Task CreateAsync_ReturnsCreated_WhenValidData()
    {
        // Arrange
        var dto = new CreateHistoryDTO
        {
            Latitude = 1,
            Longitude = 2,
            PlantingRadius = 5,
            Quantity = 10,
            TreeId = 1,
            UserId = "any" // será sobrescrito no controller
        };

        var user = new UserModel { Id = "123" };
        _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

        var response = new HistoryCreateResponseViewModel
        {
            Status = HistoryCreateStatus.Success,
            Data = new HistoryResponseDTO { Id = "1" },
            Message = "Created"
        };
        _historyServiceMock
            .Setup(s => s.CreateAsync(It.IsAny<CreateHistoryDTO>()))
            .ReturnsAsync(response);

        _controller.ModelState.Clear();  // Simula ModelState válido

        // Act
        var result = await _controller.CreateAsync(dto);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(result);
        var value = Assert.IsType<HistoryCreateResponseViewModel>(createdResult.Value);
        Assert.Equal(HistoryCreateStatus.Success, value.Status);
        Assert.Equal("1", value.Data.Id);
    }


    [Fact]
    public async Task CreateAsync_ReturnsUnauthorized_WhenUserNotFound()
    {
        // Arrange
        _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((UserModel?)null);

        var dto = new CreateHistoryDTO();

        // Act
        var result = await _controller.CreateAsync(dto);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        var value = Assert.IsType<HistoryCreateResponseViewModel>(unauthorizedResult.Value);
        Assert.Equal(HistoryCreateStatus.Unauthorized, value.Status);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsNoContent_WhenSuccess()
    {
        // Arrange
        var dto = new DeleteHistoryDTO { Id = "1" };
        var response = new HistoryDeleteResponseViewModel
        {
            Status = HistoryDeleteStatus.Success,
            Message = "Deleted"
        };
        _historyServiceMock
            .Setup(s => s.DeleteAsync<DeleteHistoryDTO, HistoryDeleteResponseViewModel>(It.IsAny<DeleteHistoryDTO>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.DeleteAsync(dto);

        // Assert
        Assert.IsType<NoContentResult>(result);
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsOk_WhenFound()
    {
        // Arrange
        var id = "1";
        var response = new HistoryGetByIdResponseViewModel
        {
            Status = HistoryGetStatus.Success,
            Data = new HistoryResponseDTO { Id = id }
        };
        _historyServiceMock
            .Setup(s => s.GetByIdAsync<GetByIdHistoryDTO, HistoryGetByIdResponseViewModel>(It.IsAny<GetByIdHistoryDTO>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.GetByIdAsync(id);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsType<HistoryGetByIdResponseViewModel>(okResult.Value);
        Assert.Equal(HistoryGetStatus.Success, value.Status);
        Assert.Equal(id, value.Data.Id);
    }
    [Fact]
    public async Task GetByUserIdPaginatedAsync_ReturnsOk_WhenSuccess()
    {
        // Arrange
        var user = new UserModel { Id = "user123" };

        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id)
    };
        var identity = new ClaimsIdentity(claims, "TestAuth");
        var claimsPrincipal = new ClaimsPrincipal(identity);

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claimsPrincipal }
        };

        _userManagerMock.Setup(u => u.GetUserAsync(claimsPrincipal)).ReturnsAsync(user);
        _userManagerMock.Setup(u => u.IsInRoleAsync(user, It.IsAny<string>())).ReturnsAsync(false);

        var dtoList = new List<HistoryResponseDTO>
    {
        new HistoryResponseDTO
        {
            Id = "1",
            Latitude = -23.55052,
            Longitude = -46.633308,
            PlantingRadius = 5.0,
            Quantity = 10,
            TreeId = "tree123",
            UserId = user.Id,
            CreatedAt = DateTime.UtcNow
        }
    };

        _historyServiceMock
            .Setup(s => s.ListPaginatedAsync<GetByUserIdPaginatedDTO, HistoryResponseDTO>(It.IsAny<GetByUserIdPaginatedDTO>()))
            .ReturnsAsync(dtoList);

        // Act
        var result = await _controller.GetByUserIdPaginatedAsync(null, 1, 10);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var value = Assert.IsAssignableFrom<IEnumerable<HistoryResponseDTO>>(okResult.Value);
        Assert.Single(value);

        var history = value.First();
        Assert.Equal("1", history.Id);
        Assert.Equal(-23.55052, history.Latitude);
        Assert.Equal(-46.633308, history.Longitude);
        Assert.Equal(5.0, history.PlantingRadius);
        Assert.Equal(10, history.Quantity);
        Assert.Equal("tree123", history.TreeId);
        Assert.Equal("user123", history.UserId);
        Assert.True(history.CreatedAt <= DateTime.UtcNow);
    }


    [Fact]
    public async Task GetByUserIdPaginatedAsync_ReturnsUnauthorized_WhenUserNotAuthenticated()
    {
        // Arrange
        _userManagerMock.Setup(u => u.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((UserModel?)null);

        // Act
        var result = await _controller.GetByUserIdPaginatedAsync(null, 1, 10);

        // Assert
        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal("Usuário não autenticado.", unauthorizedResult.Value);
    }
}
