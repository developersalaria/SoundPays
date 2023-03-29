using Microsoft.EntityFrameworkCore;
using SoundpaysAdd.Core.DTO.Account;
using SoundpaysAdd.Core.DTO;
using SoundpaysAdd.Core.Interfaces;
using SoundpaysAdd.Core.Models;
using SoundpaysAdd.Data;
using System.Security.Cryptography.X509Certificates;

namespace SoundpaysAdd.Services.Repositories
{
    public class AddAttachmentRepositoryAsync : GenericRepositoryAsync<AddAttachment>, IAddAttachmentService
    {
        #region Properties
        private readonly DbSet<AddAttachment> AddAttachments;
        private readonly SoundpaysAddContext _context;
        #endregion

        #region CTor
        public AddAttachmentRepositoryAsync(SoundpaysAddContext dbContext) : base(dbContext)
        {
            AddAttachments = dbContext.Set<AddAttachment>();
            _context = dbContext;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Get By Add Id
        /// </summary>
        /// <param name="addId"></param>
        /// <returns></returns>
        public async Task<List<AddAttachment>> GetByAddIdAsync(int addId)
        {
            return await _context.AddAttachments.Where(x => x.AddId== addId).ToListAsync();
        }

        /// <summary>
        /// Get by addId and attachmentId 
        /// </summary>
        /// <param name="addId"></param>
        /// <param name="attachmentId"></param>
        /// <returns></returns>
        public async Task<List<AddAttachment>> GetByAddIdAndAttachmentIdAsync(int addId, int attachmentId)
        {
            return await _context.AddAttachments.Where(x => x.AddId == addId && x.AttachmentId == attachmentId).ToListAsync();
        }
        #endregion
    }
}
