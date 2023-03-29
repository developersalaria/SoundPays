using Microsoft.EntityFrameworkCore;
using SoundpaysAdd.Core.DTO.Account;
using SoundpaysAdd.Core.DTO;
using SoundpaysAdd.Core.Interfaces;
using SoundpaysAdd.Core.Models;
using SoundpaysAdd.Data;
using System.Security.Cryptography.X509Certificates;

namespace SoundpaysAdd.Services.Repositories
{
    public class CampaignSegmentRepositoryAsync : GenericRepositoryAsync<CampaignSegment>, ICampaignSegmentService
    {
        #region Properties
        private readonly DbSet<CampaignSegment> _campaignSegments;
        private readonly SoundpaysAddContext _context;
        #endregion

        #region CTor
        public CampaignSegmentRepositoryAsync(SoundpaysAddContext dbContext) : base(dbContext)
        {
            _campaignSegments = dbContext.Set<CampaignSegment>();
            _context = dbContext;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Get Segements By Campaign Id
        /// </summary>
        /// <param name="campaignId"></param>
        /// <returns></returns>
        public async Task<List<CampaignSegment>> GetSegementsByCampaignIdAsync(int campaignId)
        {
            try
            {
                return await _campaignSegments.Where(x => x.CampaignId == campaignId).ToListAsync();
            }
            catch (Exception ex)
            {
                return new List<CampaignSegment>();
            }
        }
        #endregion
    }
}
