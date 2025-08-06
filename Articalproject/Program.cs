using Articalproject.DependencyInjections;
using Serilog;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;

var builder = WebApplication.CreateBuilder(args);




// ربط Serilog مع appsettings.json
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .WriteTo.MSSqlServer(
        connectionString: builder.Configuration.GetConnectionString("dbconntext"),
        sinkOptions: new MSSqlServerSinkOptions { TableName = "Logs", AutoCreateSqlTable = true })
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
        pattern: "{controller=Category}/{action=Index}/{id?}");
});



app.Run();
