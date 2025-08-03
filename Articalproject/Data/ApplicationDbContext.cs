using Articalproject.Models;
using Articalproject.Models.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Articalproject.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext()
        {

        }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> User { get; set; }

        public DbSet<Claim> claims { get; set; }

        public DbSet<Category> categorys { get; set; } 
        public DbSet<Articalproject.ViewModels.Identity.Users.GetUserByIdViewModel> GetUserByIdViewModel { get; set; } = default!;


    }
}
