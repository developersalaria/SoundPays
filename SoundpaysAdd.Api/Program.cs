using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using SoundpaysAdd.Core.Interfaces;
using SoundpaysAdd.Data;
using SoundpaysAdd.Identity.Contexts;
using SoundpaysAdd.Identity.Models;
using SoundpaysAdd.Services.Repositories;

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

// Add services to the container.

builder.Services.AddControllers();

#region Services

/// <summary>
/// add all repositories here 
/// </summary>
/// <param name="services"></param>
builder.Services.AddTransient<IApiUserAsync, ApiUserRepositoryAsync>();
#endregion 



// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SoundpaysAdd.Api", Version = "v1" });
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
    }
    catch (Exception ex)
    {

    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
