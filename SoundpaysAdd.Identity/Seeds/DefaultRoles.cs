using Microsoft.AspNetCore.Identity;
using SoundpaysAdd.Identity.Models;
using SoundpaysAdd.Core.Enums;

namespace SoundpaysAdd.Identity.Seeds
{
    public static class DefaultRoles
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Roles
            await roleManager.CreateAsync(new IdentityRole(Roles.Administrator.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Roles.Advertiser.ToString()));

        }
    }
}
