using SoundpaysAdd.Core.DTO;
using SoundpaysAdd.Core.Models;

namespace SoundpaysAdd.Core.Interfaces
{
    public interface ICampaignService : IGenericRepositoryAsync<Campaign>
    {
        Task<Tuple<List<CampaignViewModel>, string, int>> GetAllDatatable(jQueryDataTableParamModel jQueryDataTableParamModel);
        Task<Wrappers.Response<bool>> ActivateAsync(int id, bool activate = true );
        Task<Wrappers.Response<bool>> PauseAsync(int id, bool pause = true);
    }
}
