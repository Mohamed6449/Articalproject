using Articalproject.Services.Implementations;
using Articalproject.Services.InterFaces;

namespace Articalproject.DependencyInjections
{
    public static class ServiceDependencyInjection
    {


        public static IServiceCollection AddServiceDependencyInjection(this IServiceCollection Services)
        {
            
            Services.AddTransient<IFileServiece, FileServices>();

            Services.AddTransient<IClaimService, ClaimService>();

            
            
            
            return Services;

        }
    }
}
