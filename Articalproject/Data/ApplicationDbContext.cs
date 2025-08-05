using Articalproject.Models;
using Articalproject.Models.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Articalproject.ViewModels.Categories;

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
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Author>().HasOne(A=>A.user)
                .WithOne(U=>U.Author)
                .HasForeignKey<Author>(A => A.UserId)
                .OnDelete(DeleteBehavior.Cascade); //  دي اللي بتخلي الحذف تلقائي
        }

        public DbSet<Author> Authors { get; set; }
        public DbSet<User> User { get; set; }

        public DbSet<Claim> claims { get; set; }

        public DbSet<Category> categorys { get; set; } 
        public DbSet<Articalproject.ViewModels.Identity.Users.GetUserByIdViewModel> GetUserByIdViewModel { get; set; } = default!;
        public DbSet<Articalproject.ViewModels.Categories.GetCategoryByIdViewModel> GetCategoryByIdViewModel { get; set; } = default!;


    }
}
