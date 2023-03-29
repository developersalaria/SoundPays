using Microsoft.EntityFrameworkCore;
using SoundpaysAdd.Core.DTO;
using SoundpaysAdd.Core.Interfaces;
using SoundpaysAdd.Core.Models;
using SoundpaysAdd.Core.Wrappers;
using SoundpaysAdd.Data;
using System.Security.Cryptography.X509Certificates;
using SoundpaysAdd.Core.Helpers;
using System.Collections.Generic;
using System.Linq;

namespace SoundpaysAdd.Services.Repositories
{
    public class CampaignRepositoryAsync : GenericRepositoryAsync<Campaign>, ICampaignService
    {
        #region Properties
        private readonly DbSet<Campaign> _campaigns;
        private readonly SoundpaysAddContext _context;
        private readonly ICurrentUserService _currentUserService;
        #endregion

        #region CTor
        public CampaignRepositoryAsync(
            SoundpaysAddContext dbContext,
            ICurrentUserService currentUserService
            ) : base(dbContext)
        {
            _campaigns = dbContext.Set<Campaign>();
            _context = dbContext;
            _currentUserService = currentUserService;
        }



        #endregion
        #region Methods
        /// <summary>
        /// Get Campaigns Datatable data
        /// </summary>
        /// <param name="jQueryDataTableParamModel"></param>
        /// <returns></returns>
        public async Task<Tuple<List<CampaignViewModel>, string, int>> GetAllDatatable(jQueryDataTableParamModel jQueryDataTableParamModel)
        {
            try
            {
                bool isAdmin = jQueryDataTableParamModel.IsSuperAdmin;
                int avertiserId = Convert.ToInt32(jQueryDataTableParamModel.OwnerId);
                var sortColumnIndex = jQueryDataTableParamModel.iSortCol_0;
                var campaignList = (from campaign in _context.Campaigns
                                    join advertiser in _context.Advertisers on campaign.AdvertiserId equals advertiser.Id into advertiserGJ
                                    from advertiser in advertiserGJ.DefaultIfEmpty()
                                    where !campaign.IsDeleted && (isAdmin || campaign.AdvertiserId == avertiserId)
                                    select new CampaignViewModel
                                    {
                                        Id = campaign.Id,
                                        AdvertiserId = campaign.AdvertiserId,
                                        ShortName = campaign.ShortName,
                                        LongName = campaign.LongName,
                                        StartDate = campaign.StartDate,
                                        StartTime = campaign.StartTime,
                                        StopTime = campaign.StopTime,
                                        StopDate = campaign.StopDate,
                                        IsActive = campaign.IsActive,
                                        IsDeleted = campaign.IsDeleted,
                                        IsPaused = campaign.IsPaused,
                                        CreatedBy = campaign.CreatedBy,
                                        ModifiedOn = campaign.ModifiedOn,
                                        ModifiedBy = campaign.ModifiedBy,
                                        Priority = campaign.Priority,
                                        CPM = campaign.CPM,
                                        MinImpressions = campaign.MinImpressions,
                                        MaxImpressions = campaign.MaxImpressions,
                                        AdvertiserShortName = advertiser == null ? "" : advertiser.ShortName,
                                        AdvertiserLongName = advertiser == null ? "" : advertiser.LongName,
                                    });

                //Get total count
                var totalRecords = campaignList.Count();
                #region Searching
                if (!string.IsNullOrEmpty(jQueryDataTableParamModel.sSearch))
                {
                    var toSearch = jQueryDataTableParamModel.sSearch.ToLower();
                    campaignList = campaignList.Where(c => c.LongName.ToLower().Contains(toSearch) ||
                    c.ShortName.ToLower().Contains(toSearch) ||
                    c.Priority.ToString().Contains(toSearch) ||
                    c.CPM.ToString().Contains(toSearch) ||
                    c.MinImpressions.ToString().Contains(toSearch) ||
                    c.MaxImpressions.ToString().Contains(toSearch));
                }
                #endregion
                #region  Sorting
                string sortOrder = jQueryDataTableParamModel.sSortDir_0;
                campaignList = sortColumnIndex switch
                {
                    0 => sortOrder switch
                    {
                        "desc" => campaignList.OrderByDescending(a => a.LongName),
                        "asc" => campaignList.OrderBy(a => a.LongName),
                        _ => campaignList.OrderBy(a => a.LongName),
                    },
                    1 => sortOrder switch
                    {
                        "desc" => campaignList.OrderByDescending(a => a.ShortName),
                        "asc" => campaignList.OrderBy(a => a.ShortName),
                        _ => campaignList.OrderBy(a => a.ShortName),
                    },
                    2 => sortOrder switch
                    {
                        "desc" => campaignList.OrderByDescending(a => a.StartDate),
                        "asc" => campaignList.OrderBy(a => a.StartDate),
                        _ => campaignList.OrderBy(a => a.StartDate),
                    },
                    3 => sortOrder switch
                    {
                        "desc" => campaignList.OrderByDescending(a => a.StartTime),
                        "asc" => campaignList.OrderBy(a => a.StartTime),
                        _ => campaignList.OrderBy(a => a.StartTime),
                    },
                    4 => sortOrder switch
                    {
                        "desc" => campaignList.OrderByDescending(a => a.StopDate),
                        "asc" => campaignList.OrderBy(a => a.StopDate),
                        _ => campaignList.OrderBy(a => a.StopDate),
                    },
                    5 => sortOrder switch
                    {
                        "desc" => campaignList.OrderByDescending(a => a.StopTime),
                        "asc" => campaignList.OrderBy(a => a.StopTime),
                        _ => campaignList.OrderBy(a => a.StopTime),
                    },
                    6 => sortOrder switch
                    {
                        "desc" => campaignList.OrderByDescending(a => a.Priority),
                        "asc" => campaignList.OrderBy(a => a.Priority),
                        _ => campaignList.OrderBy(a => a.Priority),
                    },
                    7 => sortOrder switch
                    {
                        "desc" => campaignList.OrderByDescending(a => a.CPM),
                        "asc" => campaignList.OrderBy(a => a.CPM),
                        _ => campaignList.OrderBy(a => a.CPM),
                    },
                    8 => sortOrder switch
                    {
                        "desc" => campaignList.OrderByDescending(a => a.MinImpressions),
                        "asc" => campaignList.OrderBy(a => a.MinImpressions),
                        _ => campaignList.OrderBy(a => a.MinImpressions),
                    },
                    9 => sortOrder switch
                    {
                        "desc" => campaignList.OrderByDescending(a => a.MaxImpressions),
                        "asc" => campaignList.OrderBy(a => a.MaxImpressions),
                        _ => campaignList.OrderBy(a => a.MaxImpressions),
                    },
                    _ => campaignList.OrderBy(a => a.LongName)
                };

                #endregion
                #region  Paging
                if (jQueryDataTableParamModel.iDisplayLength != -1)
                    campaignList = campaignList.Skip(jQueryDataTableParamModel.iDisplayStart).Take(jQueryDataTableParamModel.iDisplayLength);
                #endregion
                return new Tuple<List<CampaignViewModel>, string, int>(
                   await campaignList.ToListAsync(),
                   jQueryDataTableParamModel.sEcho,
                    totalRecords);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Pause or Resum a Campaign
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pause">bool</param>
        /// <returns></returns>
        public async Task<Response<bool>> PauseAsync(int id, bool pause = true)
        {
            try
            {
                var campaigns = await _context.Campaigns.Where(x => x.Id == id).FirstOrDefaultAsync();
                if (campaigns is null) return new Response<bool>(succeeded: false, message: Constants.RecordNotFound);

                campaigns.IsPaused = pause;
                campaigns.ModifiedBy = _currentUserService.UserId;
                campaigns.ModifiedOn = DateTime.Now;
                bool status = await _context.SaveChangesAsync() > 0;

                return new Response<bool>(succeeded: status, message: status ? Constants.UpdateSuccess : Constants.SomeThingWrong);
            }
            catch (Exception ex)
            {
                return new Response<bool>(succeeded: false, message: Constants.SomeThingWrong);
            }
        }

        /// <summary>
        /// Deactivate or Activate a Campaign
        /// </summary>
        /// <param name="id"></param>
        /// <param name="activate">bool</param>
        /// <returns></returns>
        public async Task<Response<bool>> ActivateAsync(int id, bool activate = true)
        {
            try
            {
                var campaigns = await _context.Campaigns.Where(x => x.Id == id).FirstOrDefaultAsync();

                if (campaigns is null) return new Response<bool>(succeeded: false, message: Constants.RecordNotFound);

                campaigns.IsActive = activate;
                campaigns.ModifiedBy = _currentUserService.UserId;
                campaigns.ModifiedOn = DateTime.Now;
                bool status = await _context.SaveChangesAsync() > 0;

                return new Response<bool>(succeeded: status, message: status ? Constants.UpdateSuccess : Constants.SomeThingWrong);
            }
            catch (Exception ex)
            {
                return new Response<bool>(succeeded: false, message: Constants.SomeThingWrong);
            }
        }

        #endregion
    }
}
