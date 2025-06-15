using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TreeCount.Domain.Enums
{
    public enum LoginStatus
    {
        Success,
        UserNotFound,
        InvalidCredentials,
        LockedOut,
        Error
    }

    public enum CreateUserStatus
    {
        Success,
        EmailAlreadyExists,
        InvalidData,
        UnknownError
    }
    public enum UpdateUserStatus
    {
        Success,
        InvalidData,
        UnknownError
    }

    public enum DeleteUserStatus
    {
        Success,
        UserNotFound,
        InvalidCredentials,
        AlreadyDeleted,
        Unauthorized,
        OperationNotAllowed,
        DependencyExists,
        DatabaseError,
        Error
    }

    public enum UserRoles
    {
        SuperAdmin = 666,
        //CompanyAdmin = 612,
        //CompanyUser = 616,
        RegularUser = 1
    }



}
