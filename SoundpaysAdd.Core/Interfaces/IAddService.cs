using SoundpaysAdd.Core.DTO;
using SoundpaysAdd.Core.Models;

namespace SoundpaysAdd.Core.Interfaces
{
    public interface IAddService : IGenericRepositoryAsync<Add>
    {
        Task<Tuple<List<AddViewModel>, string, int>> GetAllDatatable(jQueryDataTableParamModel jQueryDataTableParamModel);
        Task<Wrappers.Response<bool>> ActivateAsync(int id, bool activate = true);
        Task<Wrappers.Response<bool>> PauseAsync(int id, bool pause = true);
    }
}
