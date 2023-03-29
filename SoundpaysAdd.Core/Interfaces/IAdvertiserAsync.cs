using SoundpaysAdd.Core.DTO;
using SoundpaysAdd.Core.Models;

namespace SoundpaysAdd.Core.Interfaces
{
    public interface IAdvertiserAsync: IGenericRepositoryAsync<Advertiser>
    {
        Task<Tuple<List<AdvertiserViewModel>, string, int>> GetAllDatatableAsync(jQueryDataTableParamModel jQueryDataTableParamModel);
        Task<Core.Wrappers.Response<bool>> PauseAsync(int id ,bool paused = true);
        Task<Core.Wrappers.Response<bool>> ActivateAsync(int id, bool activate = true);
        Task<bool> IsDuplicateEmailAsync(string email, int? id = 0);


    }
}
