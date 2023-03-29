using SoundpaysAdd.Core.DTO;
using SoundpaysAdd.Core.Models;


namespace SoundpaysAdd.Core.Interfaces
{
    public interface IApiUserAsync : IGenericRepositoryAsync<ApiUser>
    {
        Task<ApiUserViewModel> GetKeyByUserIdAsync(int id);
    }
}
