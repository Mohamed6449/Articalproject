
using Articalproject.Services.Implementations;
using Articalproject.Services.InterFaces;
using Articalproject.SharedRepository;
using Articalproject.UnitOfWorks;

namespace Articalproject.DependencyInjections
{
    public static class RepositoryDependencyInjection
    {

        public static IServiceCollection AddRepositryDependencyInjection(this IServiceCollection Services)
        {


            Services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));


            Services.AddTransient<IUnitOfWork, UnitOfWork>();

            return Services;
             
        }
    }
}
