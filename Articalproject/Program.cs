using Serilog;
using Articalproject.DependencyInjections;

var builder = WebApplication.CreateBuilder(args);




// ربط Serilog مع appsettings.json
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();
builder.Host.UseSerilog();



builder.Services.AddServiceDependencyInjection().AddRepositryDependencyInjection()
    .AddLocalizationDependencyInjection()
    .AddGeneralRegistrationDependencyInjection(builder.Configuration).AddIdentityDependencyInjection();


builder.Services.AddControllersWithViews();

var app = builder.Build();

app.AddApplicationBuillderDependencyInjection(app.Services);

app.UseDeveloperExceptionPage();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.UseSession();

app.UseEndpoints(endpoints =>
{
    _ = endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});



app.Run();
