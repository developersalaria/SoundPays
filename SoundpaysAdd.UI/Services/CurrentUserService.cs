using SoundpaysAdd.Core.Enums;
using SoundpaysAdd.Core.Interfaces;
using SoundpaysAdd.Data;
using System.Security.Claims;

namespace SoundpaysAdd.UI.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        #region CTor
        public CurrentUserService(IHttpContextAccessor httpContextAccessor, IServiceScopeFactory serviceScopeFactory)
        {
            UserId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);
            UserRoles = httpContextAccessor.HttpContext?.User?.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();
            IsSuperAdmin = httpContextAccessor.HttpContext?.User?.IsInRole(Roles.Administrator.ToString()) ?? false;
            IsAdvertiser = httpContextAccessor.HttpContext?.User?.IsInRole(Roles.Advertiser.ToString()) ?? false;
            if (IsAdvertiser)
            {
                using (var _scope = serviceScopeFactory.CreateScope())
                {
                    var _context = _scope.ServiceProvider.GetRequiredService<SoundpaysAddContext>();
                    AdvertiserId = _context.Advertisers.FirstOrDefault(x => x.UserId == UserId)?.Id ?? -1;
                }
            }
            else
            {
                AdvertiserId = -1;
            }
        }
        #endregion
        #region Properties
        public string? UserId { get; }
        public List<string>? UserRoles { get; }
        /// <summary>
        /// -1 for no record found
        /// </summary>
        public int AdvertiserId { get; }
        public bool IsSuperAdmin { get; }
        public bool IsAdvertiser { get; }
        #endregion
    }
}
