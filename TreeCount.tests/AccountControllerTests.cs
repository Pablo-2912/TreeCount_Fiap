using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json.Linq;
using TreeCount.API.Controllers;
using TreeCount.Application.DTOs;
using TreeCount.Application.Interfaces;
using TreeCount.Application.ViewModels;
using TreeCount.Domain.Enums;
using TreeCount.Repository.Identity;
using Xunit;

public class AccountControllerTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<IAccountService> _accountServiceMock;
    private readonly Mock<UserManager<UserModel>> _userManagerMock;
    private readonly AccountController _controller;

    public AccountControllerTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _accountServiceMock = new Mock<IAccountService>();
        _userManagerMock = MockUserManager<UserModel>();

        _controller = new AccountController(_userServiceMock.Object, _accountServiceMock.Object, _userManagerMock.Object);
    }

    // Helper para mock UserManager (complexo, mas esse padrão funciona)
    private static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
    {
        var store = new Mock<IUserStore<TUser>>();
        return new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
    }

    // ----- TESTE LoginAsync - sucesso
    [Fact]
    public async Task LoginAsync_ReturnsOk_WhenLoginSuccess()
    {
        var loginDto = new UserLoginDto { Email = "test@example.com", Password = "Pass123!" };
        var loginResponse = new UserLoginResponseViewModel
        {
            Status = LoginStatus.Success,
            Token = "token123",
            Nome = "User Test",
            Email = "test@example.com"
        };

        _accountServiceMock.Setup(s => s.LoginAsync(loginDto)).ReturnsAsync(loginResponse);

        var result = await _controller.LoginAsync(loginDto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<UserLoginResponseViewModel>(okResult.Value);

        Assert.Equal(LoginStatus.Success, response.Status);
        Assert.Equal("token123", response.Token);
        Assert.Equal("User Test", response.Nome);
        Assert.Equal("test@example.com", response.Email);
    }

    // ----- TESTE RegisterAsync - sucesso
    [Fact]
    public async Task RegisterAsync_ReturnsOk_WhenUserCreated()
    {
        var userCreateDto = new UserCreateDto
        {
            Email = "newuser@example.com",
            Password = "StrongP@ss1",
            ConfirmPassword = "StrongP@ss1",
            Name = "New User"
        };

        var createResponse = new UserCreateResponseViewModel
        {
            Status = CreateUserStatus.Success,
            Email = userCreateDto.Email,
            Nome = userCreateDto.Name
        };

        _userServiceMock.Setup(s => s.CreateAsync(It.IsAny<UserCreateDto>())).ReturnsAsync(createResponse);

        var result = await _controller.RegisterAsync(userCreateDto);

        var okResult = Assert.IsType<OkObjectResult>(result);

        // Usando JObject para acessar propriedades do objeto anônimo retornado
        var jObject = JObject.FromObject(okResult.Value);

        string message = jObject["message"]?.ToString();
        string email = jObject["user"]?["Email"]?.ToString();
        string nome = jObject["user"]?["Nome"]?.ToString();

        Assert.Contains("Usuário criado com sucesso", message);
        Assert.Equal(userCreateDto.Email, email);
        Assert.Equal(userCreateDto.Name, nome);
    }
    // ----- TESTE DeleteAsync - usuário não autenticado
    [Fact]
    public async Task DeleteAsync_ReturnsUnauthorized_WhenUserIsNull()
    {
        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync((UserModel)null);

        var result = await _controller.DeleteAsync();

        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        var response = Assert.IsType<UserDeleteResponseViewModel>(unauthorizedResult.Value);

        Assert.Equal(DeleteUserStatus.Unauthorized, response.Status);
        Assert.Equal("Usuário não autenticado.", response.ErrorMessage);
    }

    // ----- TESTE DeleteAsync - sucesso
    [Fact]
    public async Task DeleteAsync_ReturnsOk_WhenDeleteSuccess()
    {
        var user = new UserModel { Id = "user123", Email = "deluser@example.com" };
        _userManagerMock.Setup(um => um.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(user);

        var deleteResponse = new UserDeleteResponseViewModel
        {
            Status = DeleteUserStatus.Success,
            ErrorMessage = null
        };

        _accountServiceMock.Setup(s => s.DeleteAsync(user)).ReturnsAsync(deleteResponse);

        var result = await _controller.DeleteAsync();

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<UserDeleteResponseViewModel>(okResult.Value);

        Assert.True(response.IsSuccess);
        Assert.Equal(DeleteUserStatus.Success, response.Status);
    }

    // ----- TESTE UpdateAsync - usuário sem Id (token inválido)
    [Fact]
    public async Task UpdateAsync_ReturnsUnauthorized_WhenUserIdNotFound()
    {
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext()
        };

        var dto = new UserUpdateDto { Name = "New Name" };

        var result = await _controller.UpdateAsync(dto);

        var unauthorizedResult = Assert.IsType<UnauthorizedObjectResult>(result);
        Assert.Equal("Usuário não identificado.", unauthorizedResult.Value);
    }

    // ----- TESTE UpdateAsync - sucesso
    [Fact]
    public async Task UpdateAsync_ReturnsOk_WhenUpdateSuccess()
    {
        var userId = "user123";
        var dto = new UserUpdateDto { Name = "Updated Name", Email = "update@example.com" };

        // Simular Claim do usuário
        var userClaims = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId)
        }));

        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = userClaims }
        };

        var updateResponse = new UserUpdateResponseViewModel
        {
            Status = UpdateUserStatus.Success,
            Email = dto.Email,
            Name = dto.Name
        };

        _accountServiceMock.Setup(s => s.UpdateAsync(userId, dto)).ReturnsAsync(updateResponse);

        var result = await _controller.UpdateAsync(dto);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var response = Assert.IsType<UserUpdateResponseViewModel>(okResult.Value);

        Assert.Equal(UpdateUserStatus.Success, response.Status);
        Assert.Equal(dto.Email, response.Email);
        Assert.Equal(dto.Name, response.Name);
    }
}
