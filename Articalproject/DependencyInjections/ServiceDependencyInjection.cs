using Articalproject.Services.Implementations;
using Articalproject.Services.InterFaces;
using Articalproject.Services.InterFaces;

namespace Articalproject.DependencyInjections
{
    public static class ServiceDependencyInjection
    {


        public static IServiceCollection AddServiceDependencyInjection(this IServiceCollection Services)
        {
            
            Services.AddTransient<IFileServiece, FileServices>();
            Services.AddTransient<IAccountService,AccountService>();

            Services.AddTransient<IClaimService, ClaimService>();
            Services.AddTransient<IEmailSender, EmailSender>();
            Services.AddTransient<ICategoryServices, CategoryServices>();
            Services.AddTransient<IAuthorServices, AuthorServices>();
            Services.AddTransient<IAuthorPostServices, AuthorPostServices>();




            return Services;

        }
    }
}
