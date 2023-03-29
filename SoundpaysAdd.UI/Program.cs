using Infrastructure.Identity.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using SoundpaysAdd.Core.Helpers;
using SoundpaysAdd.Core.Interfaces;
using SoundpaysAdd.Core.Models;
using SoundpaysAdd.Data;
using SoundpaysAdd.Identity.Contexts;
using SoundpaysAdd.Identity.Interfaces;
using SoundpaysAdd.Identity.Models;
using SoundpaysAdd.Services.Repositories;
using SoundpaysAdd.Services.Services;
using SoundpaysAdd.UI.Services;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;

builder.Services.AddDbContext<SoundpaysAddContext>(options => options.UseNpgsql(configuration.GetConnectionString("SoundpaysAddDB")));

#region Identity
builder.Services.AddDbContext<IdentityContext>(options =>
    options.UseNpgsql(
        configuration.GetConnectionString("SoundpaysAddDB"),
        b => b.MigrationsAssembly(typeof(IdentityContext).Assembly.FullName)));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = true).
AddEntityFrameworkStores<IdentityContext>().
    AddDefaultTokenProviders();
#endregion
builder.Services.AddAuthorization();
builder.Services.AddRazorPages();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
#region services Register
builder.Services.AddTransient<IAccountService, AccountService>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddTransient<IActionContextAccessor, ActionContextAccessor>();

// Register application services.
builder.Services.AddScoped<IViewRenderService, ViewRenderService>();
builder.Services.AddTransient<ISmsSender, AuthMessageSender>();
builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
builder.Services.AddTransient<ICampaignService, CampaignRepositoryAsync>();
builder.Services.AddTransient<IListService, ListService>();
builder.Services.AddTransient<ICampaignSegmentService, CampaignSegmentRepositoryAsync>();
builder.Services.AddTransient<ISegmentService, SegmentRepositoryAsync>();
builder.Services.AddTransient<IAdvertiserAsync, AdvertiserRepositoryAsync>();
builder.Services.AddTransient<IApiUserAsync, ApiUserRepositoryAsync>();
builder.Services.AddTransient<IAddService, AddRepositoryAsync>();
builder.Services.AddTransient<IAddAttachmentService, AddAttachmentRepositoryAsync>();
builder.Services.AddTransient<IAttachmentService, AttachmentRepositoryAsync>();
builder.Services.AddTransient<ISoundCodeService, SoundCodeRepositoryAsync>();
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
#endregion

//set default page
builder.Services.AddControllersWithViews().AddRazorPagesOptions(options =>
{
    options.Conventions.AddPageRoute("/Account/Login", "");
});

var app = builder.Build();

//seed default users and roles
using (var scope = app.Services.CreateScope())
{
    //run migration automatically
    var soundpaysAddIdentityDB = scope.ServiceProvider.GetRequiredService<IdentityContext>();
    soundpaysAddIdentityDB.Database.Migrate();

    var soundpaysAddDB = scope.ServiceProvider.GetRequiredService<SoundpaysAddContext>();
    soundpaysAddDB.Database.Migrate();
    var services = scope.ServiceProvider;
    try
    {
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await SoundpaysAdd.Identity.Seeds.DefaultRoles.SeedAsync(userManager, roleManager);
        await SoundpaysAdd.Identity.Seeds.DefaultAdministrator.SeedAsync(userManager, roleManager);
        await SoundpaysAdd.Identity.Seeds.DefaultAdvertiser.SeedAsync(userManager, roleManager, soundpaysAddDB);
        await SoundpaysAdd.Identity.Seeds.DefaultSoundCodes.SeedAsync(soundpaysAddDB);
    }
    catch (Exception ex)
    {

    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
//configure exception middleware
app.UseMiddleware(typeof(ExceptionHandlingMiddleware));
app.MapRazorPages();
app.MapControllers();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});
app.Run();