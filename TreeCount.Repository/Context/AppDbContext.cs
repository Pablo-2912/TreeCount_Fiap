using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using TreeCount.Repository.Identity;
using TreeCount.Domain.Models;

namespace TreeCount.Repository.Context
{
    public class AppDbContext : IdentityDbContext<UserModel>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }
        public DbSet<UserModel> User { get; set; }
        public DbSet<HistoryModel> Hisory { get; set; }
        public DbSet<TreeModel> Tree { get; set; }

    }

}
