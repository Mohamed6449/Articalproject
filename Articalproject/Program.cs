using Articalproject.DependencyInjections;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddServiceDependencyInjection().AddRepositryDependencyInjection()
    .AddLocalizationDependencyInjection()
    .AddGeneralRegistrationDependencyInjection(builder.Configuration).AddIdentityDependencyInjection();


builder.Services.AddControllersWithViews();

#region Localization

#endregion

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
