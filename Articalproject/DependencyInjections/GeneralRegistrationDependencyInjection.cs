using Articalproject.Data;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace Articalproject.DependencyInjections
{
    public static class GeneralRegistrationDependencyInjection
    {
        public static IServiceCollection AddGeneralRegistrationDependencyInjection(this IServiceCollection Services,IConfiguration configuration)
        {
            Services.AddDbContext<ApplicationDbContext>(S => S.UseSqlServer(configuration["ConnectionStrings:dbconntext"]));

            Services.AddDistributedMemoryCache();
            Services.AddSession(option =>
            {
                option.IOTimeout = TimeSpan.FromMinutes(5);// المدة اللي اقصي مدة ياخدها السيشن وهو بيعلم عملية 
                option.IdleTimeout = TimeSpan.FromMinutes(5);//المدة بتاعة عدم التفاعل مع السيرفر 
                option.Cookie.IsEssential = true;// يعني يشتغل بدونها عادي ولا لازم كوكي 
                option.Cookie.Path = "/";
                option.Cookie.HttpOnly = true;//امان اكثر 
                option.Cookie.Name = ".Articalproject";
            });

            Services.AddHttpContextAccessor();

            Services.AddAutoMapper(Assembly.GetExecutingAssembly());







            return Services;
        }
    }
}
