using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using TreeCount.Common.Helpers;
using TreeCount.Domain.Enums;
using TreeCount.Repository.Identity;

namespace TreeCount.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeedController : ControllerBase
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<UserModel> _userManager;

        public SeedController(RoleManager<IdentityRole> roleManager, UserManager<UserModel> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [HttpPost("populate")]
        [AllowAnonymous] // ⚠️ Troque por [Authorize(Roles = "ADMIN")] após o primeiro uso
        public async Task<IActionResult> Populate()
        {
            string[] roles = { UserRolesHelper.ToStringRole(UserRoles.SuperAdmin), UserRolesHelper.ToStringRole(UserRoles.RegularUser) };

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    var result = await _roleManager.CreateAsync(new IdentityRole(role));
                    if (!result.Succeeded)
                        return BadRequest($"Falha ao criar a role: {role}");
                }
            }

            var adminEmail = "admin@admin.com";
            var adminUser = await _userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new UserModel
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    Name = "Admin Seed"
                };

                var createResult = await _userManager.CreateAsync(adminUser, "Admin@123");

                if (!createResult.Succeeded)
                    return BadRequest("Erro ao criar usuário admin: " + string.Join(", ", createResult.Errors.Select(e => e.Description)));

                await _userManager.AddToRoleAsync(adminUser, UserRolesHelper.ToStringRole(UserRoles.SuperAdmin));
            }

            return Ok("População concluída com sucesso! 🥳");
        }
    }
}

