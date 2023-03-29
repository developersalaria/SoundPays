using Microsoft.EntityFrameworkCore;
using SoundpaysAdd.Core.Interfaces;
using SoundpaysAdd.Core.Models;
using SoundpaysAdd.Data;

namespace SoundpaysAdd.Services.Repositories
{
    public class AttachmentRepositoryAsync : GenericRepositoryAsync<Attachment>, IAttachmentService
    {
        #region Properties
        private readonly DbSet<Attachment> Attachments;
        private readonly SoundpaysAddContext _context;
        #endregion

        #region CTor
        public AttachmentRepositoryAsync(SoundpaysAddContext dbContext) : base(dbContext)
        {
            Attachments = dbContext.Set<Attachment>();
            _context = dbContext;
        }
        #endregion
    }
}
