using SoundpaysAdd.Core.DTO;
using SoundpaysAdd.Core.DTO.Account;
using SoundpaysAdd.Core.Models;

namespace SoundpaysAdd.Core.Interfaces
{
    public interface ISegmentService : IGenericRepositoryAsync<Segment>
    {
        Task<Tuple<List<SegmentViewModel>, string, int>> GetAllDatatable(jQueryDataTableParamModel jQueryDataTableParamModel);
        Task<Core.Wrappers.Response<bool>> DeActivateAsyncs(int id, bool activate);
        Task<Core.Wrappers.Response<bool>> PauseResumeAsyncs(int id, bool pause);
        Task<bool> IsDuplicateameAsync(string name, int? id = 0);

    }
}
