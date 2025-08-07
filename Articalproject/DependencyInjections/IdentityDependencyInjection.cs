using Articalproject.Data;
using Articalproject.Models.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace Articalproject.DependencyInjections
{
    public static class IdentityDependencyInjection
    {
        public static IServiceCollection AddIdentityDependencyInjection(this IServiceCollection services)
        {
            services.AddIdentity<User, IdentityRole>(Opt=> {
                Opt.Password.RequireDigit = true;
                Opt.Password.RequireLowercase = true;
                Opt.Password.RequireUppercase = true;
                Opt.Password.RequireNonAlphanumeric= true;
                Opt.Password.RequiredLength=6;



                Opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                Opt.Lockout.MaxFailedAccessAttempts = 3;
                Opt.Lockout.AllowedForNewUsers = true;

                //user sitting
                Opt.User.RequireUniqueEmail = true;


                Opt.SignIn.RequireConfirmedPhoneNumber = false;
                Opt.SignIn.RequireConfirmedEmail = true;

   
                Opt.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(1); // مدة الإغلاق
                Opt.Lockout.MaxFailedAccessAttempts = 3; // عدد المحاولات المسموحة
                Opt.Lockout.AllowedForNewUsers = true;  // تفعيل القفل للمستخدمين الجدد

            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders() ;

            services.AddAuthorization(options =>
           {
               options.AddPolicy("UserWithClaim", policy => policy.
               RequireAssertion(context => context.User.IsInRole("Admin") && context.User.HasClaim("Create Product", "True")));
               options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
               options.AddPolicy("User", policy => policy.RequireRole("User"));
            }
            ) ;



			return services;
        }

    }
}
