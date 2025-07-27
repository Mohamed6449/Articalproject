using Articalproject.Resources;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace Articalproject.DependencyInjections
{
    public static class LocalizationDependencyInjection
    {
        public static IServiceCollection AddLocalizationDependencyInjection(this IServiceCollection Services)
        {

            Services.AddControllersWithViews().AddViewLocalization().AddDataAnnotationsLocalization(Option =>
            {
                Option.DataAnnotationLocalizerProvider = (type, factory) => factory.Create(typeof(SharedResources));
            });
            Services.AddLocalization(opt =>
            {
                opt.ResourcesPath = "";
            });
            Services.Configure<RequestLocalizationOptions>(options =>
            {
                List<CultureInfo> supportedCultures = new List<CultureInfo>
        {
            new CultureInfo("en-US"),
            new CultureInfo("ar-EG"),
        };

                options.DefaultRequestCulture = new RequestCulture("en-US");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });

            return Services;
        }
    }
}
