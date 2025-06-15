using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeCount.Repository.Identity;

namespace TreeCount.Application.Interfaces
{
    public interface IJwtTokenGenerator
    {

        string GenerateToken(UserModel user);
    }
}
