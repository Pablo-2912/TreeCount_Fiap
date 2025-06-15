using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeCount.Application.DTOs;
using TreeCount.Application.ViewModels;
using TreeCount.Repository.Identity;

namespace TreeCount.Application.Interfaces
{
    public interface IAccountService
    {
        Task<UserLoginResponseViewModel> LoginAsync(UserLoginDto model);

        Task<UserDeleteResponseViewModel> DeleteAsync(UserModel model);

        Task LogoutAsync();

        Task<UserUpdateResponseViewModel> UpdateAsync(string userId, UserUpdateDto dto);
    }
}
