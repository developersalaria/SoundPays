using Microsoft.AspNetCore.Identity;
using SoundpaysAdd.Core.Enums;
using SoundpaysAdd.Core.Models;
using SoundpaysAdd.Data;
using SoundpaysAdd.Identity.Models;
using System.Security.Cryptography.X509Certificates;

namespace SoundpaysAdd.Identity.Seeds
{
    public static class DefaultAdvertiser
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, SoundpaysAddContext soundpaysAddContext)
        {
            //Seed Default Coordinator
            var defaultUser = new ApplicationUser
            {
                UserName = "advertiser@soundpays.com",
                Email = "advertiser@soundpays.com",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };
            if (userManager.Users.All(u => u.Id != defaultUser.Id))
            {
                var user = await userManager.FindByEmailAsync(defaultUser.Email);
                if (user == null)
                {
                    await userManager.CreateAsync(defaultUser, "SoundpaysAdd#321");
                    await userManager.AddToRoleAsync(defaultUser, Roles.Advertiser.ToString());
                    await soundpaysAddContext.Advertisers.AddAsync(new Advertiser
                    {
                        ShortName = "advertiser@soundpays.com",
                        LongName = "advertiser@soundpays.com",
                        Email = "advertiser@soundpays.com",
                        IsPaused = false,
                        IsActive= true,
                        UserId = defaultUser.Id
                    });
                    soundpaysAddContext.SaveChanges();
                }

            }
        }
    }
}
