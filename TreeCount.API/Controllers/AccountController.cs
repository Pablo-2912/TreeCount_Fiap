using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using TreeCount.Application.DTOs;
using TreeCount.Application.Interfaces;
using TreeCount.Application.Services;
using TreeCount.Application.ViewModels;
using TreeCount.Common.Helpers;
using TreeCount.Domain.Enums;
using TreeCount.Repository.Identity;

namespace TreeCount.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IAccountService _accountService;
        private readonly UserManager<UserModel> _userManager;

        public AccountController(IUserService userService, IAccountService accountService, UserManager<UserModel> userManager)
        {
            _userManager = userManager;
            _userService = userService;
            _accountService = accountService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> LoginAsync([FromBody] UserLoginDto model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Preencha todos os campos");
            }

            var loginResponse = await _accountService.LoginAsync(model);

            if (loginResponse == null)
            {
                return Unauthorized("Falha ao autenticar.");
            }

            switch (loginResponse.Status)
            {
                case LoginStatus.Success:
                    return Ok(loginResponse);

                case LoginStatus.UserNotFound:
                    return NotFound("Usuário não encontrado.");

                case LoginStatus.InvalidCredentials:
                    return Unauthorized("Credenciais inválida.");

                default:
                    return BadRequest("Erro desconhecido.");
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("[action]")]
        public async Task<IActionResult> RegisterAsync([FromBody] UserCreateDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Dados inválidos. Verifique os campos obrigatórios.");

            if (!ValidatorsHelpers.IsValidEmail(model.Email))
                return BadRequest("Formato de email inválido.");

            if (!StringHelper.SecureEquals(model.Password, model.ConfirmPassword))
                return BadRequest("As senhas não coincidem.");

            if (!ValidatorsHelpers.IsStrongPassword(model.Password))
                return BadRequest("A senha deve conter ao menos 8 caracteres, uma letra maiúscula, uma minúscula, um número e um caractere especial.");

            var userCreateDto = new UserCreateDto
            {
                Email = model.Email,
                Password = model.Password,
                ConfirmPassword = model.ConfirmPassword,
                Name = model.Name ?? model.Email.Split('@')[0]
            };

            var result = await _userService.CreateAsync(userCreateDto);

            return result.Status switch
            {
                CreateUserStatus.Success => Ok(new
                {
                    message = "Usuário criado com sucesso.",
                    user = result
                }),
                CreateUserStatus.EmailAlreadyExists => Conflict(new
                {
                    error = "Este email já está em uso."
                }),
                CreateUserStatus.InvalidData => BadRequest(new
                {
                    error = "Os dados fornecidos são inválidos. Tente novamente."
                }),
                _ => StatusCode(500, new
                {
                    error = "Ocorreu um erro inesperado ao criar o usuário."
                })
            };
        }

        [Authorize]
        [HttpDelete]
        [Route("[action]")]
        public async Task<IActionResult> DeleteAsync()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return Unauthorized(new UserDeleteResponseViewModel
                {
                    Status = DeleteUserStatus.Unauthorized,
                    ErrorMessage = "Usuário não autenticado."
                });
            }

            var result = await _accountService.DeleteAsync(user);

            return result.Status switch
            {
                DeleteUserStatus.Success => Ok(result),
                DeleteUserStatus.UserNotFound => NotFound(result),
                DeleteUserStatus.AlreadyDeleted => BadRequest(result),
                DeleteUserStatus.InvalidCredentials => BadRequest(result),
                DeleteUserStatus.Unauthorized => Unauthorized(result),
                DeleteUserStatus.OperationNotAllowed => Forbid(),
                DeleteUserStatus.DependencyExists => Conflict(result),
                DeleteUserStatus.DatabaseError => StatusCode(500, result),
                _ => StatusCode(500, new UserDeleteResponseViewModel
                {
                    Status = DeleteUserStatus.Error,
                    ErrorMessage = "Erro inesperado."
                })
            };
        }

        [Authorize]
        [HttpPut]
        [Route("")]
        public async Task<IActionResult> UpdateAsync([FromBody] UserUpdateDto dto)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Usuário não identificado.");
            }

            // Força o Id do token no serviço (pode passar junto, por exemplo)
            var result = await _accountService.UpdateAsync(userId, dto);

            return result.Status switch
            {
                UpdateUserStatus.Success => Ok(result),
                UpdateUserStatus.InvalidData => BadRequest(result),
                _ => StatusCode(500, new UserUpdateResponseViewModel
                {
                    Status = UpdateUserStatus.UnknownError,
                    ErrorMessage = "Erro inesperado ao atualizar usuário."
                })
            };
        }
    }
}
