using SoundpaysAdd.Core.DTO;
using SoundpaysAdd.Core.DTO.Account;
using SoundpaysAdd.Core.Models;

namespace SoundpaysAdd.Core.Interfaces
{
    public interface IAddAttachmentService : IGenericRepositoryAsync<AddAttachment>
    {
        Task<List<AddAttachment>> GetByAddIdAsync(int addId);
        Task<List<AddAttachment>> GetByAddIdAndAttachmentIdAsync(int addId,int attachmentId);
    }
}
