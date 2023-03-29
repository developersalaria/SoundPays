using SoundpaysAdd.Core.DTO;
using SoundpaysAdd.Core.Models;

namespace SoundpaysAdd.Core.Interfaces
{
    public interface ICampaignSegmentService : IGenericRepositoryAsync<CampaignSegment>
    {
        Task<List<CampaignSegment>> GetSegementsByCampaignIdAsync(int campaignId);
    }
}
