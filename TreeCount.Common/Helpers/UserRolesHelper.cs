using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeCount.Domain.Enums;

namespace TreeCount.Common.Helpers
{
    public class UserRolesHelper
    {
        public static UserRoles FromString(string roleName) => roleName.ToLowerInvariant() switch
        {
            "SUPER_ADMIN" => UserRoles.SuperAdmin,
            //"companyadmin" => UserRoles.CompanyAdmin,
            //"companyuser" => UserRoles.CompanyUser,
            "REGULAR_USER" => UserRoles.RegularUser,
            _ => UserRoles.RegularUser
        };

        public static string ToLabel(UserRoles role) => role switch
        {
            UserRoles.SuperAdmin => "Administrator Geral",
            //UserRoles.CompanyAdmin => "Company Administrator",
            //UserRoles.CompanyUser => "Company User",
            UserRoles.RegularUser => "Usuario Comum",
            _ => "Usuario Comum"
        };

        public static string ToStringRole(UserRoles role) => role switch
        {
            UserRoles.SuperAdmin => "SUPER_ADMIN",
            //UserRoles.CompanyAdmin => "companyadmin",
            //UserRoles.CompanyUser => "companyuser",
            UserRoles.RegularUser => "REGULAR_USER",
            _ => "REGULAR_USER"
        };

        public static UserRoles FromLabel(string label) => label.ToLowerInvariant() switch
        {
            "Administrator Geral" => UserRoles.SuperAdmin,
            //"company administrator" => UserRoles.CompanyAdmin,
            //"company user" => UserRoles.CompanyUser,
            "Usuario Comum" => UserRoles.RegularUser,
            _ => UserRoles.RegularUser
        };

        public static List<string> GetAllRoles()
        {
            return Enum.GetNames(typeof(UserRoles)).ToList();
        }

        public static bool IsValidRole(string roleName)
        {
            return Enum.IsDefined(typeof(UserRoles), roleName);
        }
    }
}
