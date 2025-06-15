using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeCount.Domain.Enums;

namespace TreeCount.Application.ViewModels
{
    public class UserLoginResponseViewModel
    {
        public string Token { get; set; }
        public string Nome { get; set; }
        public string Email { get; set; }
        public LoginStatus Status { get; set; }
        public string? ErrorMessage { get; set; }
    }

    public class UserCreateResponseViewModel
    {
        public CreateUserStatus Status { get; set; }
        public string? ErrorMessage { get; set; }
        public string? Email { get; set; }
        public string? Nome { get; set; }
    }

    public class UserDeleteResponseViewModel
    {
        public DeleteUserStatus Status { get; set; }
        public string? ErrorMessage { get; set; }

        public bool IsSuccess => Status == DeleteUserStatus.Success;
    }

    public class UserUpdateResponseViewModel
    {
        public UpdateUserStatus Status { get; set; }
        public string? ErrorMessage { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
    }

}
