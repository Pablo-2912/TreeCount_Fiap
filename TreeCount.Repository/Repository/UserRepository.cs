using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreeCount.Domain.Interfaces.Repository;
using TreeCount.Repository.Context;
using TreeCount.Repository.Identity;

namespace TreeCount.Repository.Repository
{
    public class UserRepository : RepositoryBase<UserModel>, IUserRepository<UserModel>
    {
        private readonly UserManager<UserModel> _userManager;

        public UserRepository(AppDbContext context, UserManager<UserModel> userManager) : base(context)
        {
            _userManager = userManager;
        }

        public async Task<IdentityResult> CreateUserAsync(UserModel user, string password)
        {
            user.OnCreate();
            var result = await _userManager.CreateAsync(user, password);
            return result;
        }

        public override async Task<bool> DeleteAsync(UserModel model)
        {
            try
            {
                model.Delete();

                await UpdateAsync(model);

                return true;
            }
            catch (Exception ex) {

                throw;
            }
        }
    }
}
