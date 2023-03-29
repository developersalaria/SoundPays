using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SoundpaysAdd.Core.DTO;
using SoundpaysAdd.Core.Interfaces;
using SoundpaysAdd.Core.Helpers;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using SoundpaysAdd.Core.Models;

namespace SoundpaysAdd.UI.Pages.SoundCode
{
    //[Authorize]
    public class IndexModel : PageModel
    {

        #region Properties
        private readonly ISoundCodeService _soundCodeService;
        private readonly ICurrentUserService _currentUserService;
        [BindProperty]
        public List<SoundCodeViewModel> soundCodes { get; set; }
        private readonly IMapper _mapper;
        #endregion

        #region Ctor
        public IndexModel(ISoundCodeService soundCodeService, ICurrentUserService currentUserService, IMapper mapper)
        {
            _soundCodeService = soundCodeService;
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
            ViewData["Title"] = "Sound Code";
        }

        /// <summary>
        /// Get Create Or Edit sound code
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<PartialViewResult> OnGetCreateOrEditAsync(int id = 0)
        {
            ViewData["Title"] = (id > 0) ? "Edit Sound Code" : "Add Sound Code";
            SoundCodeViewModel soundCodeViewModel = new();
            if (id > 0)
            {
                var soundCode = await _soundCodeService.GetByIdAsync(id);

                if (soundCode != null)
                {
                    soundCodeViewModel = _mapper.Map<SoundCodeViewModel>(soundCode);
                }
            }
            return new PartialViewResult
            {
                ViewName = "_CreateOrEdit",
                ViewData = new ViewDataDictionary<SoundCodeViewModel>(ViewData, soundCodeViewModel)
            };
        }

        /// <summary>
        /// On Post Create Or Edit
        /// </summary>
        /// <param name="campaignViewModel"></param>
        /// <returns></returns>
        public async Task<JsonResult> OnPostCreateOrEditAsync(SoundCodeViewModel soundCodeViewModel)
        {
            try
            {
                var modelValidationResult = ModelValidation.Check(ModelState);
                if (!modelValidationResult.Succeeded)
                {
                    return new JsonResult(new { success = false, message = modelValidationResult.Message });
                }
                if (await _soundCodeService.IsDuplicateCodeAsync(soundCodeViewModel.Code, id: soundCodeViewModel.Id))
                {
                    return new JsonResult(new { success = false, message = Constants.CodeAlreadyExist });
                }
                bool isNewRecord = soundCodeViewModel.Id <= 0;
                if (isNewRecord)
                {
                    soundCodeViewModel.IsActive = true;
                    soundCodeViewModel.IsPaused = false;
                    soundCodeViewModel.IsDeleted = false;
                    soundCodeViewModel.CreatedBy = _currentUserService.UserId;
                     Core.Models.SoundCode soundCode = _mapper.Map<Core.Models.SoundCode>(soundCodeViewModel);
                    await _soundCodeService.AddAsync(soundCode);
                    return new JsonResult(new { success = true, message = Constants.CreateSuccess });
                }
                else
                {
                    var result = await _soundCodeService.GetByIdNoTrackAsync(soundCodeViewModel.Id);
                    if (result == null) return new JsonResult(new { success = true, message = Constants.RecordNotFound });
                    soundCodeViewModel.ModifiedBy = _currentUserService.UserId;
                    soundCodeViewModel.ModifiedOn = DateTime.UtcNow;
                    Core.Models.SoundCode soundCode = _mapper.Map<Core.Models.SoundCode>(soundCodeViewModel);
                    await _soundCodeService.UpdateAsync(soundCode);
                    return new JsonResult(new { success = true, message = Constants.UpdateSuccess });
                }
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }

        }

        /// <summary>
        /// On Get All Sound Code  Database
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        public async Task<JsonResult> OnGetAllSoundCodeDataTable(jQueryDataTableParamModel parms)
        {
            var response = await _soundCodeService.GetAllDatatable(parms);
            return new JsonResult(new
            {
                aaData = response.Item1,
                iTotalRecords = response.Item3,
                iTotalDisplayRecords = response.Item3
            });
        }

        #region actions

        /// <summary>
        /// Delete Add By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<JsonResult> OnPostDeleteAsync(int id)
        {
            string message = "Error";
            var soundCode = await _soundCodeService.GetByIdAsync(id);
            if (id > 0)
            {
                await _soundCodeService.DeleteAsync(soundCode);
            }
            return new JsonResult(new
            {
                success = true,
                message = true ? Core.Helpers.Constants.ActionSuccess : message
            });
        }

        /// <summary>
        /// Deactivate SoundCode By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<JsonResult> OnPostDeactivateAsync(int id)
        {
            try
            {
                var resp = await _soundCodeService.DeActivateAsyncs(id, false);
                return new JsonResult(new { success = resp.Succeeded, message = resp.Message });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = Constants.SomeThingWrong });
            }
        }
        /// <summary>
        /// Activate SoundCode By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<JsonResult> OnPostActivateAsync(int id)
        {
            try
            {
                var resp = await _soundCodeService.DeActivateAsyncs(id, true);
                return new JsonResult(new { success = resp.Succeeded, message = resp.Message });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = Constants.SomeThingWrong });
            }
        }

        /// <summary>
        /// Pause SoundCode By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<JsonResult> OnPostResumeAsync(int id)
        {
            try
            {
                var resp = await _soundCodeService.PauseResumeAsyncs(id, false);
                return new JsonResult(new { success = resp.Succeeded, message = resp.Message });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = Core.Helpers.Constants.SomeThingWrong });
            }
        }

        /// <summary>
        /// Resume SoundCode By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<JsonResult> OnPostPauseAsync(int id)
        {
            try
            {
                var resp = await _soundCodeService.PauseResumeAsyncs(id, true);
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
