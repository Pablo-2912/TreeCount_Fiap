using Microsoft.AspNetCore.Identity;
using TreeCount.Application.DTOs;
using TreeCount.Application.ViewModels;

namespace TreeCount.Application.Interfaces
{

    public interface IUserService 
    {
        Task<UserCreateResponseViewModel> CreateAsync(UserCreateDto model);
    }

}
