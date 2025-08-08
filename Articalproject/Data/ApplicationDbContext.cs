using Articalproject.Models;
using Articalproject.Models.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Articalproject.ViewModels.Categories;
using Articalproject.ViewModels.Post;

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
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<AuthorPost>().HasOne(A=>A.Author)
                .WithMany(U=>U.authorPosts)
                .HasForeignKey(A => A.AuthorId)
                .OnDelete(DeleteBehavior.Cascade); 
            
            modelBuilder.Entity<AuthorPost>().HasOne(A=>A.Category)
                .WithMany(U=>U.AuthorPosts)
                .HasForeignKey(A => A.CategoryId)
                .OnDelete(DeleteBehavior.Cascade); //  دي اللي بتخلي الحذف تلقائي
        }

        public DbSet<Author> Authors { get; set; }
        public DbSet<User> User { get; set; }

        public DbSet<Claim> claims { get; set; }
        public DbSet<AuthorPost> AuthorPosts { get; set; }

        public DbSet<Category> categorys { get; set; } 



    }
}
