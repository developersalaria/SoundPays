using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SoundpaysAdd.Core.DTO;
using SoundpaysAdd.Core.Interfaces;
using System.Dynamic;
using SoundpaysAdd.Core.Helpers;
using SoundpaysAdd.Core.Models;
using System.Security.Cryptography.X509Certificates;
using AutoMapper;
using SoundpaysAdd.Core.Enums;
using SoundpaysAdd.Core.Wrappers;

namespace SoundpaysAdd.UI.Pages.Campaign
{
    [Authorize(Roles = "Administrator, Advertiser")]
    public class IndexModel : PageModel
    {

        #region Properties
        private readonly ICampaignService _campaignService;
        private readonly IListService _listService;
        private readonly ICurrentUserService _currentUserService;
        private readonly ICampaignSegmentService _campaignSegmentService;
        private readonly ISegmentService _segmentService;
        private readonly IMapper _mapper;
        [BindProperty]
        public List<CampaignViewModel> campaigns { get; set; }
        [BindProperty]
        public bool IsAdmin => _currentUserService.IsSuperAdmin;

        #endregion

        #region Ctor
        public IndexModel(ICampaignService campaignService,
            IListService listService,
            ICurrentUserService currentUserService,
            ICampaignSegmentService campaignSegmentService,
            ISegmentService segmentService,
            IMapper mapper)
        {
            _campaignService = campaignService;
            _listService = listService;
            _currentUserService = currentUserService;
            _campaignSegmentService = campaignSegmentService;
            _segmentService = segmentService;
            _mapper = mapper;
        }
        #endregion

        #region Methods

        /// <summary>
        /// On load
        /// </summary>
        public async Task OnGet()
        {
            ViewData["Title"] = "Campaign";
        }

        /// <summary>
        /// Get Create Or Edit Campaign
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<PartialViewResult> OnGetCreateOrEditAsync(int id = 0, bool readOnly = false)
        {

            ViewData["IsReadOnly"] = readOnly;
            ViewData["IsAdmin"] = _currentUserService.IsSuperAdmin;
            ViewData["Title"] = (id > 0) ? "Edit Campaign" : "Add Campaign";
            CampaignViewModel campaignViewModel = new();
            if (id > 0)
            {
                var campaign = await _campaignService.GetByIdAsync(id);
                if (campaign != null)
                {
                    campaignViewModel = _mapper.Map<CampaignViewModel>(campaign);
                    var campaignSegemnts = await _campaignSegmentService.GetSegementsByCampaignIdAsync(campaignViewModel.Id);
                    if (campaignSegemnts != null)
                    {
                        campaignViewModel.SegementIdArray = campaignSegemnts.Select(x => x.SegmentId).ToArray();
                    }
                }
            }
            campaignViewModel.SegementList = await _listService.GetAllSegmentMultiAsync(campaignViewModel.SegementIdArray);
            campaignViewModel.AdvertiserList = await _listService.GetAllCampaignsAsync();
            return new PartialViewResult
            {
                ViewName = "_CreateOrEdit",
                ViewData = new ViewDataDictionary<CampaignViewModel>(ViewData, campaignViewModel)
            };
        }

        /// <summary>
        /// saving form 
        /// </summary>
        /// <param name="campaignViewModel"></param>
        /// <returns></returns>
        public async Task<JsonResult> OnPostCreateOrEditAsync(CampaignViewModel campaignViewModel)
        {
            try
            {
                var modelValidationResult = ModelValidation.Check(ModelState);
                if (!modelValidationResult.Succeeded)
                {
                    return new JsonResult(new { success = false, message = modelValidationResult.Message });
                }
                bool isNewRecord = campaignViewModel.Id <= 0;
                if (_currentUserService.IsAdvertiser)
                {
                    campaignViewModel.AdvertiserId = _currentUserService.AdvertiserId;
                }
                if (isNewRecord)
                {
                    campaignViewModel.IsActive = false;
                    campaignViewModel.IsPaused = false;
                    campaignViewModel.IsDeleted = false;
                    campaignViewModel.CreatedBy = _currentUserService.UserId;
                    Core.Models.Campaign campaign = _mapper.Map<Core.Models.Campaign>(campaignViewModel);
                    await _campaignService.AddAsync(campaign);
                    await AddRangeCampaignAsync(campaignViewModel.SegementIdArray, campaign.Id);
                }
                else
                {
                    var campaign = await _campaignService.GetByIdNoTrackAsync(campaignViewModel.Id);

                    if (campaign == null) return new JsonResult(new { success = true, message = Constants.RecordNotFound });
                    campaign.AdvertiserId = campaignViewModel.AdvertiserId;
                    campaign.ShortName = campaignViewModel.ShortName;
                    campaign.LongName = campaignViewModel.LongName;
                    campaign.StartDate = campaignViewModel.StartDate;
                    campaign.StartTime = campaignViewModel.StartTime;
                    campaign.StopDate = campaignViewModel.StopDate;
                    campaign.StopTime = campaignViewModel.StopTime;
                    campaign.CPM = campaignViewModel.CPM;
                    campaign.Priority = campaignViewModel.Priority;
                    campaign.MinImpressions = campaignViewModel.MinImpressions;
                    campaign.MaxImpressions = campaignViewModel.MaxImpressions;
                    campaign.ModifiedBy = _currentUserService.UserId;
                    campaign.ModifiedOn = DateTime.UtcNow;
                    await _campaignService.UpdateAsync(campaign);
                    var oldSegemnts = await _campaignSegmentService.GetSegementsByCampaignIdAsync(campaignViewModel.Id);
                    //chekcing if there is any sengement alreday assosiated 
                    if (oldSegemnts is not null && oldSegemnts.Count > 0)
                    {
                        var oldSegementsArray = oldSegemnts.Select(x => x.SegmentId).ToArray();
                        int[]? removedSegments = new int[] { }, newSegements = new int[] { };
                        //chicking if there is any manipulation on segement(s) of the campaign
                        if (campaignViewModel.SegementIdArray is not null && campaignViewModel.SegementIdArray.Count() > 0)
                        {
                            removedSegments = oldSegementsArray.Where(x => !campaignViewModel.SegementIdArray.Contains(x)).ToArray();
                            newSegements = campaignViewModel.SegementIdArray?.Where(x => !oldSegementsArray.Contains(x)).ToArray();
                        }
                        //removing all old segments from db if the segement are removed
                        else
                        {
                            await _campaignSegmentService.DeleteRangeAsync(oldSegemnts);
                        }

                        //removing old segment if any is removed 
                        if (removedSegments is not null && removedSegments.Count() > 0)
                        {
                            var removedSengemntsEntity = oldSegemnts.Where(x => removedSegments.Contains(x.SegmentId));
                            await _campaignSegmentService.DeleteRangeAsync(removedSengemntsEntity);
                        }
                        //if there is any new segement added 
                        if (newSegements is not null && newSegements.Count() > 0)
                        {
                            await AddRangeCampaignAsync(newSegements, campaignViewModel.Id);
                        }
                    }
                    else
                    {
                        await AddRangeCampaignAsync(campaignViewModel.SegementIdArray, campaignViewModel.Id);
                    }
                }
                return new JsonResult(new { success = true, message = Constants.UpdateSuccess });

            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }

        }

        /// <summary>
        /// Add Campaign segements
        /// </summary>
        /// <param name="ids"></param>
        /// <param name="campaignId"></param>
        /// <returns></returns>
        private async Task AddRangeCampaignAsync(int[]? ids, int campaignId)
        {
            if (ids is not null && ids.Count() > 0)
            {
                List<CampaignSegment> campaignSegmentList = new List<CampaignSegment>();
                foreach (var id in ids)
                {
                    campaignSegmentList.Add(new CampaignSegment
                    {
                        SegmentId = id,
                        CampaignId = campaignId,
                        CreatedBy = _currentUserService.UserId,
                        ModifiedBy = _currentUserService.UserId,
                        CreatedOn = DateTime.UtcNow,
                        ModifiedOn = DateTime.UtcNow
                    });
                }
                await _campaignSegmentService.AddRangeAsync(campaignSegmentList);
            }
        }

        /// <summary>
        /// On Get All Campaign Database
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        public async Task<JsonResult> OnGetAllCampaignDataTable(jQueryDataTableParamModel parms)
        {
            try
            {
                parms.IsSuperAdmin = _currentUserService.IsSuperAdmin;
                parms.OwnerId = _currentUserService.AdvertiserId.ToString();
                var response = await _campaignService.GetAllDatatable(parms);
                return new JsonResult(new { aaData = response.Item1, iTotalRecords = response.Item3, iTotalDisplayRecords = response.Item3 });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { aaData = new List<CampaignViewModel>(), iTotalRecords = 0, iTotalDisplayRecords = 0 });
            }
        }

        #region actions

        /// <summary>
        /// Delete Campaign By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<JsonResult> OnPostDeleteAsync(int id)
        {
            try
            {
                var campaign = await _campaignService.GetByIdAsync(id);
                if (campaign == null) return new JsonResult(new { success = false, message = Constants.RecordNotFound });

                await _campaignService.DeleteAsync(campaign);
                return new JsonResult(new { success = true, message = Constants.DeleteSuccess });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = Constants.SomeThingWrong });
            }
        }

        /// <summary>
        /// Deactivate Campaign By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<JsonResult> OnPostDeactivateAsync(int id)
        {
            try
            {
                var resp = await _campaignService.ActivateAsync(id, false);
                return new JsonResult(new { success = resp.Succeeded, message = resp.Message });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = Constants.SomeThingWrong });
            }
        }
        /// <summary>
        /// Activate Campaign By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<JsonResult> OnPostActivateAsync(int id)
        {
            try
            {
                var resp = await _campaignService.ActivateAsync(id);
                return new JsonResult(new { success = resp.Succeeded, message = resp.Message });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = Constants.SomeThingWrong });
            }
        }

        /// <summary>
        /// Pause Campaign By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<JsonResult> OnPostResumeAsync(int id)
        {
            try
            {
                var resp = await _campaignService.PauseAsync(id);
                return new JsonResult(new { success = resp.Succeeded, message = resp.Message });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = Constants.SomeThingWrong });
            }
        }
        /// <summary>
        /// Resume Campaign By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<JsonResult> OnPostPauseAsync(int id)
        {
            try
            {
                var resp = await _campaignService.PauseAsync(id, false);
                return new JsonResult(new { success = resp.Succeeded, message = resp.Message });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = Constants.SomeThingWrong });
            }
        }


        #endregion

        #endregion

    }
}
