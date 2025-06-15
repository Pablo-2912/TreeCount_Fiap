using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeCount.Application.DTOs;
using TreeCount.Application.Interfaces;
using TreeCount.Application.ViewModels;
using TreeCount.Common.Helpers;
using TreeCount.Domain.Enums;
using TreeCount.Domain.Interfaces.Repository;
using TreeCount.Repository.Identity;

namespace TreeCount.Application.Services
{
    public class AccountService: IAccountService
    {

        private readonly UserManager<UserModel> _userManager;
        private readonly SignInManager<UserModel> _signInManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IUserRepository<UserModel> _userRepository;

        public AccountService(UserManager<UserModel> userManager, SignInManager<UserModel> signInManager, IJwtTokenGenerator jwtGen, IUserRepository<UserModel> userRepository)
        {
            _userRepository = userRepository;
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenGenerator = jwtGen;
        }

        public async Task<UserLoginResponseViewModel> LoginAsync(UserLoginDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                return new UserLoginResponseViewModel
                {
                    Status = LoginStatus.UserNotFound
                };
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, isPersistent: false, lockoutOnFailure: true);
            if (!result.Succeeded)
            {
                return new UserLoginResponseViewModel
                {
                    Status = LoginStatus.InvalidCredentials,
                };
            }

            var token = _jwtTokenGenerator.GenerateToken(user);

            return new UserLoginResponseViewModel
            {
                Status = LoginStatus.Success,
                Token = token,
                Nome = user.Name,
                Email = user.Email
            };
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        public async Task<UserDeleteResponseViewModel> DeleteAsync(UserModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Id))
            {
                return new UserDeleteResponseViewModel
                {
                    Status = DeleteUserStatus.InvalidCredentials,
                    ErrorMessage = "Dados inválidos fornecidos para exclusão."
                };
            }

            var user = await _userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                return new UserDeleteResponseViewModel
                {
                    Status = DeleteUserStatus.UserNotFound,
                    ErrorMessage = "Usuário não encontrado."
                };
            }

            if (user.DeletedAt is not null)
            {
                return new UserDeleteResponseViewModel
                {
                    Status = DeleteUserStatus.AlreadyDeleted,
                    ErrorMessage = "Usuário já foi excluído anteriormente."
                };
            }

            try
            {
                user.Email = $"{user.Email}_{EncryptHelper.Hash(user.PasswordHash)}";

                var _ = await _userRepository.DeleteAsync(user);// Soft delete = marcar como excluído

                if (_)
                    return new UserDeleteResponseViewModel
                    {
                        Status = DeleteUserStatus.Success
                    };
                else
                    return new UserDeleteResponseViewModel
                    {
                        Status = DeleteUserStatus.DatabaseError
                    };
            }
            catch (DbUpdateException)
            {
                return new UserDeleteResponseViewModel
                {
                    Status = DeleteUserStatus.DatabaseError,
                    ErrorMessage = "Erro de banco de dados ao excluir o usuário."
                };
            }
            catch (Exception)
            {
                return new UserDeleteResponseViewModel
                {
                    Status = DeleteUserStatus.Error,
                    ErrorMessage = "Erro inesperado ao excluir o usuário."
                };
            }
        }

        public async Task<UserUpdateResponseViewModel> UpdateAsync(string userId, UserUpdateDto model)
        {
            var response = new UserUpdateResponseViewModel();

            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    response.Status = UpdateUserStatus.InvalidData;
                    response.ErrorMessage = $"Usuário com Id '{userId}' não foi encontrado.";
                    return response;
                }

                user.UserName = model.Name ?? user.UserName;  
                user.Email = model.Email ?? user.Email;

                var identityResult = await _userManager.UpdateAsync(user);

                if (!identityResult.Succeeded)
                {
                    response.Status = UpdateUserStatus.InvalidData;
                    response.ErrorMessage = string.Join(", ", identityResult.Errors.Select(e => e.Description));
                    return response;
                }

                response.Status = UpdateUserStatus.Success;
                response.Name = user.UserName;
                response.Email = user.Email;

                return response;
            }
            catch (Exception ex)
            {
                response.Status = UpdateUserStatus.UnknownError;
                response.ErrorMessage = $"Erro inesperado: {ex.Message}";
                return response;
            }
        }



    }
}
