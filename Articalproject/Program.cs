using Serilog;
using Articalproject.DependencyInjections;

var builder = WebApplication.CreateBuilder(args);



// ✳️ Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.File("Logs/log.txt", rollingInterval: RollingInterval.Day) // ملف جديد كل يوم
    .CreateLogger();

builder.Host.UseSerilog(); // استخدام Serilog بدل الـ Logger الافتراضي


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
