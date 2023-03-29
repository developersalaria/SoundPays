using Microsoft.EntityFrameworkCore;
using SoundpaysAdd.Core.DTO;
using SoundpaysAdd.Core.Interfaces;
using SoundpaysAdd.Core.Models;
using SoundpaysAdd.Data;

namespace SoundpaysAdd.Services.Repositories
{
    public class ApiUserRepositoryAsync : GenericRepositoryAsync<ApiUser>, IApiUserAsync
    {
        #region Properties
        private readonly DbSet<ApiUser> _apiUsers;
        private readonly SoundpaysAddContext _context;
        #endregion

        #region CTor
        public ApiUserRepositoryAsync(SoundpaysAddContext dbContext) : base(dbContext)
        {
            _apiUsers = dbContext.Set<ApiUser>();
            _context = dbContext;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Get key by user id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<ApiUserViewModel> GetKeyByUserIdAsync(int id)
        {
            try
            {
                return await _apiUsers.Where(x => x.AdvertiserId == id).Select(apiUser => new ApiUserViewModel
                {
                    Id = apiUser.Id,
                    ClientId = apiUser.ClientId,
                    ClientSecret = apiUser.ClientSecret,
                    ApiKey = apiUser.ApiKey,
                    ClientKey = apiUser.ClientKey,
                    AdvertiserId = apiUser.AdvertiserId,
                    IsActive = apiUser.IsActive,
                    IsDeleted = apiUser.IsDeleted,
                    CreatedBy = apiUser.CreatedBy,
                    CreatedOn = apiUser.CreatedOn,
                    ModifiedBy = apiUser.ModifiedBy,
                    ModifiedOn = apiUser.ModifiedOn
                }).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion
    }
}

