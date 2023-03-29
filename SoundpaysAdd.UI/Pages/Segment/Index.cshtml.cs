using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SoundpaysAdd.Core.DTO;
using SoundpaysAdd.Core.Interfaces;
using SoundpaysAdd.Core.Helpers;
using AutoMapper;

namespace SoundpaysAdd.UI.Pages.Segment
{
    //[Authorize]
    public class IndexModel : PageModel
    {

        #region Properties
        private readonly ISegmentService _segmentService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IMapper _mapper;
        [BindProperty]
        public List<SegmentViewModel> segments { get; set; }
        #endregion

        #region Ctor
        public IndexModel(ISegmentService segmentService, ICurrentUserService currentUserService, IMapper mapper)
        {
            _segmentService = segmentService;
            _currentUserService = currentUserService;
            _mapper = mapper;
        }
        #endregion

        #region Methods

        /// <summary>
        /// On load
        /// </summary>
        public async Task OnGet()
        {
            ViewData["Title"] = "Add";
        }


        /// <summary>
        /// Get Create Or Edit Segment
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<PartialViewResult> OnGetCreateOrEditAsync(int id = 0)
        {
            ViewData["Title"] = (id > 0) ? "Edit Segment" : "Add Segment";
            SegmentViewModel segmentViewModel = new();
            if (id > 0)
            {
                var segment = await _segmentService.GetByIdAsync(id);

                if (segment != null)
                {
                    segmentViewModel = _mapper.Map<SegmentViewModel>(segment);
                }
            }
            return new PartialViewResult
            {
                ViewName = "_CreateOrEdit",
                ViewData = new ViewDataDictionary<SegmentViewModel>(ViewData, segmentViewModel)
            };
        }

        /// <summary>
        /// On Post Create Or Edit
        /// </summary>
        /// <param name="campaignViewModel"></param>
        /// <returns></returns>
        public async Task<JsonResult> OnPostCreateOrEditAsync(SegmentViewModel segmentViewModel)
        {
            try
            {
                var modelValidationResult = ModelValidation.Check(ModelState);
                if (!modelValidationResult.Succeeded)
                {
                    return new JsonResult(new { success = false, message = modelValidationResult.Message });
                }

                if (await _segmentService.IsDuplicateameAsync(segmentViewModel.Name, id: segmentViewModel.Id))
                {
                    return new JsonResult(new { success = false, message = Constants.SegmentAlreadyExist });
                }

                bool isNewRecord = segmentViewModel.Id <= 0;
                if (isNewRecord)
                {
                    segmentViewModel.IsActive = true;
                    segmentViewModel.IsDeleted = false;
                    segmentViewModel.CreatedBy = _currentUserService.UserId;
                    Core.Models.Segment segment = _mapper.Map<Core.Models.Segment>(segmentViewModel);
                    await _segmentService.AddAsync(segment);
                    return new JsonResult(new { success = true, message = Constants.CreateSuccess });
                }
                else
                {
                    var result = await _segmentService.GetByIdNoTrackAsync(segmentViewModel.Id);
                    if (result == null) return new JsonResult(new { success = true, message = Constants.RecordNotFound });
                    segmentViewModel.ModifiedBy = _currentUserService.UserId;
                    segmentViewModel.ModifiedOn = DateTime.UtcNow;
                    Core.Models.Segment segment = _mapper.Map<Core.Models.Segment>(segmentViewModel);
                    await _segmentService.UpdateAsync(segment);
                    return new JsonResult(new { success = true, message = Constants.UpdateSuccess });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }

        }

        /// <summary>
        /// On Get All Segment  Database
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        public async Task<JsonResult> OnGetAllSegmentDataTable(jQueryDataTableParamModel parms)
        {
            var response = await _segmentService.GetAllDatatable(parms);
            return new JsonResult(new
            {
                aaData = response.Item1,
                iTotalRecords = response.Item3,
                iTotalDisplayRecords = response.Item3
            });
        }

        #region actions

        /// <summary>
        /// Delete Segment By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<JsonResult> OnPostDeleteAsync(int id)
        {
            string message = "Error";
            var add = await _segmentService.GetByIdAsync(id);
            if (id > 0)
            {
                await _segmentService.DeleteAsync(add);
            }
            return new JsonResult(new
            {
                success = true,
                message = true ? Core.Helpers.Constants.ActionSuccess : message
            });
        }

        /// <summary>
        /// Deactivate Segment By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<JsonResult> OnPostDeactivateAsync(int id)
        {
            try
            {
                var resp = await _segmentService.DeActivateAsyncs(id, false);
                return new JsonResult(new { success = resp.Succeeded, message = resp.Message });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = Constants.SomeThingWrong });
            }
        }
        /// <summary>
        /// Activate Segment By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<JsonResult> OnPostActivateAsync(int id)
        {
            try
            {
                var resp = await _segmentService.DeActivateAsyncs(id, true);
                return new JsonResult(new { success = resp.Succeeded, message = resp.Message });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = Constants.SomeThingWrong });
            }
        }

        /// <summary>
        /// Pause Segment By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<JsonResult> OnPostResumeAsync(int id)
        {
            try
            {
                var resp = await _segmentService.PauseResumeAsyncs(id, false);
                return new JsonResult(new { success = resp.Succeeded, message = resp.Message });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = Core.Helpers.Constants.SomeThingWrong });
            }
        }

        /// <summary>
        /// Resume Segment By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<JsonResult> OnPostPauseAsync(int id)
        {
            try
            {
                var resp = await _segmentService.PauseResumeAsyncs(id, true);
                return new JsonResult(new { success = resp.Succeeded, message = resp.Message });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = Core.Helpers.Constants.SomeThingWrong });
            }
        }
        #endregion

        #endregion

    }
}
