using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using TreeCount.Repository.Identity;

namespace TreeCount.Repository.Context
{
    public class AppDbContext : IdentityDbContext<UserModel>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        // public DbSet<UserModel> Users { get; set; }
        //public DbSet<RelatorioModel> Relatorios { get; set; }

    }

}
