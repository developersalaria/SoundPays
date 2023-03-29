using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SoundpaysAdd.Core.Enums;
using SoundpaysAdd.Core.Interfaces;
using SoundpaysAdd.Core.Models;
using SoundpaysAdd.Data;

namespace SoundpaysAdd.Services.Services
{
    public class ListService : IListService
    {

        #region Ctor

        public readonly SoundpaysAddContext _context;
        public ListService(SoundpaysAddContext context)
        {
            _context = context;
        }
        #endregion

        #region Methods

        /// <summary>
        /// Get all campaign as select list for dropdown
        /// </summary>
        /// <returns>Lits of SelectListItem</returns>
        public async Task<List<SelectListItem>> GetAllCampaignsAsync()
        {
            return await (from advertiser in _context.Advertisers
                          where !advertiser.IsDeleted && advertiser.IsActive
                          select new SelectListItem
                          {
                              Text = Convert.ToString(advertiser.LongName),
                              Value = advertiser.Id.ToString()
                          }).ToListAsync();

        }

        /// <summary>
        /// Get All Sound Code
        /// </summary>
        /// <returns></returns>
        public async Task<List<SelectListItem>> GetAllSoundCodeAsync()
        {
            return await (from soundCode in _context.SoundCodes
                          where !soundCode.IsDeleted && soundCode.IsActive
                          select new SelectListItem
                          {
                              Text = Convert.ToString(soundCode.Code),
                              Value = soundCode.Id.ToString()
                          }).ToListAsync();

        }

        /// <summary>
        /// Get All Campaign
        /// </summary>
        /// <returns></returns>
        public async Task<List<SelectListItem>> GetAllCampaignCodeAsync()
        {
            return await (from campaign in _context.Campaigns
                          where !campaign.IsDeleted && campaign.IsActive
                          select new SelectListItem
                          {
                              Text = Convert.ToString(campaign.LongName),
                              Value = campaign.Id.ToString()
                          }).ToListAsync();

        }

        /// <summary>
        /// Get add type from enums as Select List Item
        /// </summary>
        /// <returns></returns>
        public List<SelectListItem> GetAllAddType()
        {
            return Enum.GetValues(typeof(AddTypes)).Cast<AddTypes>().Select(v => new SelectListItem
            {
                Text = v.ToString(),
                Value = ((int)v).ToString()
            }).ToList();
        }

        #region Multi List
        public async Task<MultiSelectList> GetAllSegmentMultiAsync(int[] selected)
        {
            return new MultiSelectList(await _context.Segments.Where(x => !x.IsDeleted && x.IsActive).ToListAsync(), "Id", "Name", selected);
        }
        #endregion
        #endregion
    }
}
