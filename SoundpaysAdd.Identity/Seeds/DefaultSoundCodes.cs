using SoundpaysAdd.Core.Models;
using SoundpaysAdd.Data;

namespace SoundpaysAdd.Identity.Seeds
{
    public static class DefaultSoundCodes
    {
        public static async Task SeedAsync(SoundpaysAddContext soundpaysAddContext)
        {
            //Seed Default Codes
            var soundCode = new SoundCode
            {
                IsActive = true,
                IsPaused = false,
                CreatedBy = "admin@SoundpaysAdd.com",
                IsDeleted = false,
                ModifiedOn = DateTime.Now,
                ModifiedBy = "admin@SoundpaysAdd.com",
                Code = "Dolby",
                StartZone = 100,
                EndZone = 200
            };

            var dolbySoundCodeList = soundpaysAddContext.SoundCodes.Where(x => x.Code == soundCode.Code).ToList();
            if (dolbySoundCodeList.Count() == 0)
            {
                soundpaysAddContext.Add(soundCode);
                var result = soundpaysAddContext.SaveChanges();
            }

        }
    }
}
