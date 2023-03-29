using Microsoft.EntityFrameworkCore;
using SoundpaysAdd.Core.DTO;
using SoundpaysAdd.Core.Interfaces;
using SoundpaysAdd.Core.Models;
using SoundpaysAdd.Core.Wrappers;
using SoundpaysAdd.Data;
using SoundpaysAdd.Core.Helpers;
using SoundpaysAdd.Core.Enums;
using System.Linq;

namespace SoundpaysAdd.Services.Repositories
{
    public class AddRepositoryAsync : GenericRepositoryAsync<Add>, IAddService
    {
        #region Properties
        private readonly DbSet<Add> Adds;
        private readonly SoundpaysAddContext _context;
        private readonly ICurrentUserService _currentUserService;
        #endregion

        #region CTor
        public AddRepositoryAsync(SoundpaysAddContext dbContext, ICurrentUserService currentUserService) : base(dbContext)
        {
            Adds = dbContext.Set<Add>();
            _context = dbContext;
            _currentUserService = currentUserService;
        }



        #endregion

        #region Methods
        public async Task<Tuple<List<AddViewModel>, string, int>> GetAllDatatable(jQueryDataTableParamModel jQueryDataTableParamModel)
        {
            try
            {
                bool isAdmin = jQueryDataTableParamModel.IsSuperAdmin;
                int avertiserId = Convert.ToInt32(jQueryDataTableParamModel.OwnerId);
                var sortColumnIndex = jQueryDataTableParamModel.iSortCol_0;
                var campaignList = (from add in _context.Adds
                                    join campaign in _context.Campaigns on add.CampaignId equals campaign.Id
                                    join soundCode in _context.SoundCodes on add.SoundCodeId equals soundCode.Id
                                    where !add.IsDeleted && !campaign.IsPaused && !campaign.IsDeleted && (isAdmin || campaign.AdvertiserId == avertiserId)
                                    select new AddViewModel
                                    {
                                        Id = add.Id,
                                        CampaignId= campaign.Id,
                                        CampaignName = campaign.LongName,
                                        SoundCodeId = soundCode.Id,
                                        AddType = add.AddType,
                                        SoundCodeName = soundCode.Code,
                                        ShortName = add.ShortName,
                                        LongName = add.LongName,
                                        StartTime = add.StartTime,
                                        StartDate = add.StartDate,
                                        StopDate = add.StopDate,
                                        StopTime = add.StopTime,
                                        MinHeight= add.MinHeight,
                                        MaxHeight = add.MaxHeight,
                                        MinWidth = add.MinWidth,
                                        MaxWidth = add.MaxWidth,
                                        IsActive = add.IsActive,
                                        IsDeleted = add.IsDeleted,
                                        IsPaused = add.IsPaused,
                                        CreatedBy = add.CreatedBy,
                                        ModifiedOn = add.ModifiedOn,
                                        ModifiedBy = add.ModifiedBy,
                                    }).ToList();

                //Get total count
                var totalRecords = campaignList.Count;
                #region Searching
                if (!string.IsNullOrEmpty(jQueryDataTableParamModel.sSearch))
                {
                    var toSearch = jQueryDataTableParamModel.sSearch.ToLower();
                    campaignList = campaignList.Where(c => c.LongName.ToLower().Contains(toSearch)).ToList();

                }
                #endregion
                #region  Sorting
                switch (sortColumnIndex)
                {
                    //Sort by Name
                    case 0:
                        switch (jQueryDataTableParamModel.sSortDir_0)
                        {
                            case "desc":
                                campaignList = campaignList.OrderByDescending(a => a.LongName).ToList();
                                break;
                            case "asc":
                                campaignList = campaignList.OrderBy(a => a.LongName).ToList();
                                break;
                            default:
                                campaignList = campaignList.OrderBy(a => a.LongName).ToList();
                                break;
                        }
                        break;
                    default:
                        campaignList = campaignList.OrderBy(a => a.LongName).ToList();
                        break;
                }
                #endregion
                #region  Paging
                if (jQueryDataTableParamModel.iDisplayLength != -1)
                    campaignList = campaignList.Skip(jQueryDataTableParamModel.iDisplayStart).Take(jQueryDataTableParamModel.iDisplayLength).ToList();
                #endregion
                return new Tuple<List<AddViewModel>, string, int>(
                   campaignList,
                   jQueryDataTableParamModel.sEcho,
                    totalRecords);
            }
            catch (Exception ex)
            {
                throw;
            }
        }


        public async Task<Response<bool>> PauseAsync(int id, bool pause = true)
        {
            try
            {
                var add = await _context.Adds.Where(x => x.Id == id).FirstOrDefaultAsync();

                if (add is null) return new Response<bool>(succeeded: false, message: Constants.RecordNotFound);

                add.IsPaused = pause;
                add.ModifiedBy = _currentUserService.UserId;
                add.ModifiedOn = DateTime.Now;
                bool status = await _context.SaveChangesAsync() > 0;

                return new Response<bool>(succeeded: status, message: status ? Constants.SaveSuccess : Constants.SomeThingWrong);
            }
            catch (Exception ex)
            {
                return new Response<bool>(succeeded: false, message: Constants.SomeThingWrong);
            }
        }

        public async Task<Response<bool>> ActivateAsync(int id, bool activate = true)
        {
            try
            {
                var add = await _context.Adds.Where(x => x.Id == id).FirstOrDefaultAsync();

                if (add is null) return new Response<bool>(succeeded: false, message: Constants.RecordNotFound);

                add.IsActive = activate;
                add.ModifiedBy = _currentUserService.UserId;
                add.ModifiedOn = DateTime.Now;
                bool status = await _context.SaveChangesAsync() > 0;

                return new Response<bool>(succeeded: status, message: status ? Constants.SaveSuccess : Constants.SomeThingWrong);
            }
            catch (Exception ex)
            {
                return new Response<bool>(succeeded: false, message: Constants.SomeThingWrong);
            }
        }

        #endregion
    }
}
