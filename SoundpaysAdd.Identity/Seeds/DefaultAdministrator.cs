using Microsoft.AspNetCore.Identity;
using SoundpaysAdd.Core.Enums;
using SoundpaysAdd.Identity.Models;

namespace SoundpaysAdd.Identity.Seeds
{
    public static class DefaultAdministrator
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            //Seed Default Administrator
            var defaultUser = new ApplicationUser
            {
                UserName = "admin@SoundpaysAdd.com",
                Email = "admin@SoundpaysAdd.com",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };
            if (userManager.Users.All(u => u.Id != defaultUser.Id))
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "SoundpaysAdd#321");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Administrator.ToString());
                }

            }
        }
    }
}
