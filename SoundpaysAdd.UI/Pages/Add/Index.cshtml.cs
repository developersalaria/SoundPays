using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SoundpaysAdd.Core.DTO;
using SoundpaysAdd.Core.Interfaces;
using SoundpaysAdd.Core.Helpers;
using SoundpaysAdd.Core.Enums;
using System;
using SoundpaysAdd.Core.Models;
using AutoMapper;
using SoundpaysAdd.Core.Wrappers;
using System.Net.Mail;
using System.Collections.Generic;

namespace SoundpaysAdd.UI.Pages.Add
{
    //[Authorize]
    public class IndexModel : PageModel
    {

        #region Properties
        private readonly ICampaignService _campaignService;
        private readonly IListService _listService;
        private readonly ICurrentUserService _currentUserService;
        private readonly IAddService _addService;
        private readonly IAttachmentService _attachmentService;
        private readonly IAddAttachmentService _addAttachmentService;
        private readonly IMapper _mapper;

        [BindProperty]
        public bool IsAdmin => _currentUserService.IsSuperAdmin;
        #endregion

        #region Ctor
        public IndexModel(ICampaignService campaignService,
            IListService listService,
            ICurrentUserService currentUserService,
            IAddService addService,
            IAttachmentService attachmentService,
            IAddAttachmentService addAttachmentService,
             IMapper mapper)
        {
            _campaignService = campaignService;
            _listService = listService;
            _currentUserService = currentUserService;
            _addService = addService;
            _attachmentService = attachmentService;
            _addAttachmentService = addAttachmentService;
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
        /// Open modal popup for add, edit or view 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="readOnly"></param>
        /// <returns></returns>
        public async Task<PartialViewResult> OnGetCreateOrEditAsync(int id = 0, bool readOnly = false)
        {
            ViewData["IsReadOnly"] = readOnly;
            ViewData["IsAdmin"] = _currentUserService.IsSuperAdmin;
            ViewData["Title"] = (id > 0) ? "Edit Add" : "Add Campaign Add";
            AddViewModel addViewModel = new();
            try
            {
                var add = await _addService.GetByIdAsync(id);
                if (add is not null)
                {
                    addViewModel = _mapper.Map<AddViewModel>(add);
                    var addAttachments = await _addAttachmentService.GetByAddIdAsync(add.Id);

                    if (addAttachments is not null)
                    { //get add attachments

                        var sdAttachment = addAttachments.Where(x => x.Format == AttachmentType.SD.ToString()).ToList();
                        var hdAttachment = addAttachments.Where(x => x.Format == AttachmentType.HD.ToString()).ToList();
                        var fhdAttachment = addAttachments.Where(x => x.Format == AttachmentType.FHD.ToString()).ToList();

                        addViewModel.AttachmentListSD = await GetAttachmentListAsync(sdAttachment, addViewModel.Id, "AttachmentListSD", readOnly);
                        addViewModel.AttachmentListHD = await GetAttachmentListAsync(hdAttachment, addViewModel.Id, "AttachmentListHD", readOnly);
                        addViewModel.AttachmentListFHD = await GetAttachmentListAsync(fhdAttachment, addViewModel.Id, "AttachmentListFHD", readOnly);
                    }
                }

                addViewModel.SoundCodeList = await _listService.GetAllSoundCodeAsync();
                addViewModel.CampaignList = await _listService.GetAllCampaignCodeAsync();
                addViewModel.AddTypeList = _listService.GetAllAddType();
                addViewModel.AddType = 1; //default selection 

                return new PartialViewResult
                {
                    ViewName = "_CreateOrEdit",
                    ViewData = new ViewDataDictionary<AddViewModel>(ViewData, addViewModel)
                };
            }
            catch (Exception ex)
            {
                return new PartialViewResult
                {
                    ViewName = "_CreateOrEdit",
                    ViewData = new ViewDataDictionary<AddViewModel>(ViewData, addViewModel)
                };
            }
        }

        private async Task<List<AttachmentViewModel>> GetAttachmentListAsync(List<AddAttachment>? attchaments, int recordId, string attachmentName, bool readOnly = false)
        {
            try
            {
                List<AttachmentViewModel> attachmentList = new List<AttachmentViewModel>();
                if (attchaments is not null && attchaments.Count > 0)
                {
                    foreach (var attchament in attchaments)
                    {
                        //AttachmentViewModel attachmentViewModel = new AttachmentViewModel();
                        var attachment = await _attachmentService.GetByIdAsync(attchament.AttachmentId);
                        if (attachment is not null)
                        {
                            var mappedAtchment = _mapper.Map<AttachmentViewModel>(attachment);
                            mappedAtchment.RecordId = recordId;
                            mappedAtchment.ActionUrl = "/Add?handler=DeleteAttachment";
                            mappedAtchment.AttachmentName = attachmentName;
                            mappedAtchment.IsReadOnly = readOnly;
                            attachmentList.Add(mappedAtchment);
                        }
                    }
                }
                return attachmentList;
            }
            catch
            {
                throw;
            }
        }

        /// <summary>
        /// Update Add
        /// </summary>
        /// <param name="campaignViewModel"></param>
        /// <returns></returns>
        public async Task<JsonResult> OnPostCreateOrEditAsync(AddViewModel addViewModel)
        {
            Core.Models.Add addItem = new();
            try
            {
                var modelValidationResult = ModelValidation.Check(ModelState);
                if (!modelValidationResult.Succeeded)
                {
                    return new JsonResult(new { success = false, message = modelValidationResult.Message });
                }
                bool isNewRecord = addViewModel.Id <= 0;
                if (isNewRecord)
                {
                    addViewModel.IsActive = false;
                    addViewModel.IsPaused = false;
                    addViewModel.IsDeleted = false;
                    addViewModel.CreatedBy = _currentUserService.UserId;
                }
                else
                {
                    addViewModel.ModifiedBy = _currentUserService.UserId;
                    addViewModel.ModifiedOn = DateTime.UtcNow;
                }

                if (isNewRecord)
                {
                    addItem = await _addService.AddAsync(_mapper.Map<Core.Models.Add>(addViewModel));
                }
                else
                {
                    addItem = await _addService.GetByIdNoTrackAsync(addViewModel.Id);
                    if (addItem == null) return new JsonResult(new { success = true, message = Constants.RecordNotFound });
                    addItem.ModifiedBy = addViewModel.ModifiedBy;
                    addItem.ModifiedOn = addViewModel.ModifiedOn;
                    addItem.SoundCodeId = addViewModel.SoundCodeId;
                    addItem.CampaignId = addViewModel.CampaignId;
                    addItem.ShortName = addViewModel.ShortName;
                    addItem.LongName = addViewModel.LongName;
                    addItem.AddType = addViewModel.AddType;
                    addItem.MinWidth = addViewModel.MinWidth;
                    addItem.MaxWidth = addViewModel.MaxWidth;
                    addItem.MinHeight = addViewModel.MinHeight;
                    addItem.MaxHeight = addViewModel.MaxHeight;
                    addItem.JSTag = addViewModel.JSTag;
                    addItem.StartDate = addViewModel.StartDate;
                    addItem.StartTime = addViewModel.StartTime;
                    addItem.StopDate = addViewModel.StopDate;
                    addItem.StopTime = addViewModel.StopTime;
                    await _addService.UpdateAsync(addItem);
                }

                //use for add new attachment
                if (addItem != null && addItem.Id > 0)
                {
                    await AddAttachmentsAsync(addViewModel.AttachmentListSD, addItem.Id, AttachmentType.SD);
                    await AddAttachmentsAsync(addViewModel.AttachmentListHD, addItem.Id, AttachmentType.HD);
                    await AddAttachmentsAsync(addViewModel.AttachmentListFHD, addItem.Id, AttachmentType.FHD);
                }
                return new JsonResult(new { success = true, message = isNewRecord ? Constants.SaveSuccess : Constants.UpdateSuccess });

            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = ex.Message });
            }

        }

        /// <summary>
        /// add attchemnets for add
        /// </summary>
        /// <param name="attachments"></param>
        /// <param name="addId"></param>
        /// <returns></returns>
        private async Task AddAttachmentsAsync(List<AttachmentViewModel> attachments, int addId, AttachmentType type)
        {
            try
            {
                if (attachments is not null && attachments.Count() > 0)
                {
                    foreach (var attachmentItem in attachments)
                    {
                        if (attachmentItem.Id == 0)
                        {
                            Core.Models.Attachment attachment = new Core.Models.Attachment();
                            attachment.CreatedBy = _currentUserService.UserId;
                            attachment.ModifiedBy = _currentUserService.UserId;
                            attachment.ModifiedOn = DateTime.UtcNow;
                            attachment.Location = attachmentItem.Location;
                            attachment.Size = attachmentItem.Size;
                            attachment.FileName = attachmentItem.FileName;
                            attachment.DummyFileName = attachmentItem.DummyFileName;
                            var addedAttachment = await _attachmentService.AddAsync(attachment);
                            if (addedAttachment != null && addedAttachment.Id > 0)
                            {
                                AddAttachment addAttachment = new AddAttachment();
                                addAttachment.CreatedBy = _currentUserService.UserId;
                                addAttachment.ModifiedBy = _currentUserService.UserId;
                                addAttachment.ModifiedOn = DateTime.UtcNow;
                                addAttachment.AddId = addId;
                                addAttachment.AttachmentId = attachment.Id;
                                addAttachment.Format = type.ToString();
                                await _addAttachmentService.AddAsync(addAttachment);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        /// <summary>
        /// On Get All add Database
        /// </summary>
        /// <param name="parms"></param>
        /// <returns></returns>
        public async Task<JsonResult> OnGetAllAddDataTable(jQueryDataTableParamModel parms)
        {
            try
            {
                parms.IsSuperAdmin = _currentUserService.IsSuperAdmin;
                parms.OwnerId = _currentUserService.AdvertiserId.ToString();
                var response = await _addService.GetAllDatatable(parms);
                return new JsonResult(new
                {
                    aaData = response.Item1,
                    iTotalRecords = response.Item3,
                    iTotalDisplayRecords = response.Item3
                });
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    aaData = new List<AddViewModel>(),
                    iTotalRecords = 0,
                    iTotalDisplayRecords = 0
                });
            }
        }

        #region actions

        /// <summary>
        /// Delete Add By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<JsonResult> OnPostDeleteAsync(int id)
        {
            try
            {
                var add = await _addService.GetByIdAsync(id);
                if (id > 0)
                {
                    await _addService.DeleteAsync(add);
                }
                return new JsonResult(new { success = true, message = Constants.ActionSuccess });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = Constants.SomeThingWrong });
            }
        }

        /// <summary>
        /// Deactivate Add By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<JsonResult> OnPostDeactivateAsync(int id)
        {
            try
            {
                var resp = await _addService.ActivateAsync(id, false);
                return new JsonResult(new { success = resp.Succeeded, message = resp.Message });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = Constants.SomeThingWrong });
            }
        }
        /// <summary>
        /// Activate Add By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<JsonResult> OnPostActivateAsync(int id)
        {
            try
            {
                var resp = await _addService.ActivateAsync(id);
                return new JsonResult(new { success = resp.Succeeded, message = resp.Message });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = Constants.SomeThingWrong });
            }
        }

        /// <summary>
        /// Pause Add By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<JsonResult> OnPostResumeAsync(int id)
        {
            try
            {
                var resp = await _addService.PauseAsync(id, false);
                return new JsonResult(new { success = resp.Succeeded, message = resp.Message });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = Constants.SomeThingWrong });
            }
        }

        /// <summary>
        /// Resume Add By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<JsonResult> OnPostPauseAsync(int id)
        {
            try
            {
                var resp = await _addService.PauseAsync(id);
                return new JsonResult(new { success = resp.Succeeded, message = resp.Message });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { success = false, message = Constants.SomeThingWrong });
            }
        }

        /// <summary>
        /// Delete Add Attchamnet
        /// </summary>
        /// <param name="recordId">addID</param>
        /// <param name="id">Attachment  id</param>
        /// <returns></returns>
        public async Task<JsonResult> OnPostDeleteAttachmentAsync(int recordId, int id)
        {
            try
            {
                var attachment = await _attachmentService.GetByIdAsync(id);
                if (id > 0)
                {
                    //delete attachment
                    await _attachmentService.DeleteAsync(attachment);
                    //get add attachment by add Id
                    var addAttachments = await _addAttachmentService.GetByAddIdAndAttachmentIdAsync(recordId, attachment.Id);
                    //delete attachments and add attachment
                    if (addAttachments != null)
                    {
                        foreach (var addAttachment in addAttachments)
                        {
                            //delete add attachment
                            await _addAttachmentService.DeleteAsync(addAttachment);
                        }
                    }
                }
                return new JsonResult(new { success = true, message = Constants.ActionSuccess });
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
