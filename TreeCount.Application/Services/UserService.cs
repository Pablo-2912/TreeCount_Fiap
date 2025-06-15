using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeCount.Application.DTOs;
using TreeCount.Application.Interfaces;
using TreeCount.Application.ViewModels;
using TreeCount.Repository.Identity;
using TreeCount.Domain.Enums;
using TreeCount.Common.Helpers;

namespace TreeCount.Application.Services
{
    public class UserService : IUserService
    {
        private readonly UserManager<UserModel> _userManager;
        private readonly SignInManager<UserModel> _signInManager;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        public UserService(UserManager<UserModel> userManager, SignInManager<UserModel> signInManager, IJwtTokenGenerator jwtGen)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _jwtTokenGenerator = jwtGen;
        }

        public async Task<UserCreateResponseViewModel> CreateAsync(UserCreateDto model)
        {
            var existingUser = await _userManager.FindByEmailAsync(model.Email);

            if (existingUser != null)
            {
                return new UserCreateResponseViewModel
                {
                    Status = CreateUserStatus.EmailAlreadyExists,
                    ErrorMessage = "Este e-mail já está em uso."
                };
            }

            // Garante que o UserName só tenha caracteres permitidos (sem acento, sem símbolos)
            var userName = StringHelper.SanitizeString(model.Email); // ou usar Guid para anonimato

            var user = new UserModel
            {
                UserName = userName,
                Email = model.Email, // ❗ Não sanitize o e-mail
                Name = StringHelper.SanitizeString(model.Name)
            };

            user.OnCreate();

            var createResult = await _userManager.CreateAsync(user, model.Password);

            if (!createResult.Succeeded)
            {
                return new UserCreateResponseViewModel
                {
                    Status = CreateUserStatus.InvalidData,
                    ErrorMessage = string.Join("; ", createResult.Errors.Select(e => e.Description))
                };
            }

            var roleName = UserRolesHelper.ToStringRole(UserRoles.RegularUser);
            var roleResult = await _userManager.AddToRoleAsync(user, roleName);

            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);

                return new UserCreateResponseViewModel
                {
                    Status = CreateUserStatus.InvalidData,
                    ErrorMessage = $"Erro ao atribuir role ao usuário: {string.Join("; ", roleResult.Errors.Select(e => e.Description))}"
                };
            }

            return new UserCreateResponseViewModel
            {
                Status = CreateUserStatus.Success,
                Email = user.Email,
                Nome = user.Name
            };
        }



        //public async Task<UserCreateResponseViewModel> CreateCompanyAdminAsync(UserCreateDto model)
        //{
        //    var existingUser = await _userManager.FindByEmailAsync(model.Email);

        //    if (existingUser != null)
        //    {
        //        return new UserCreateResponseViewModel
        //        {
        //            Status = CreateUserStatus.EmailAlreadyExists,
        //            ErrorMessage = "Este e-mail já está em uso."
        //        };
        //    }

        //    var user = new UserModel
        //    {
        //        UserName = model.Email,
        //        Email = model.Email,
        //        Name = model.Name
        //    };

        //    user.OnCreate();

        //    var result = await _userManager.CreateAsync(user, model.Password);

        //    if (result.Succeeded)
        //    {
        //        await _userManager.AddToRoleAsync(user, UserRolesHelper.ToStringRole(UserRoles.CompanyAdmin)); // Usa o enum EmpresaAdmin traduzido para CompanyAdmin

        //        return new UserCreateResponseViewModel
        //        {
        //            Status = CreateUserStatus.Success,
        //            Email = user.Email,
        //            Nome = user.Name
        //        };
        //    }

        //    return new UserCreateResponseViewModel
        //    {
        //        Status = CreateUserStatus.InvalidData,
        //        ErrorMessage = string.Join("; ", result.Errors.Select(e => e.Description))
        //    };
        //}

    }
}
