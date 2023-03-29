using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using SoundpaysAdd.Core.DTO;
using SoundpaysAdd.Core.Enums;
using SoundpaysAdd.Core.Helpers;
using SoundpaysAdd.Core.Interfaces;
using SoundpaysAdd.UI.Services;

namespace SoundpaysAdd.UI.Controllers
{
    public class AttachmentController : Controller
    {

        private readonly ICurrentUserService _currentUserService;
        private readonly IViewRenderService _viewRenderService;
        public AttachmentController(
            ICurrentUserService currentUserService,
            IViewRenderService viewRenderService)
        {
            _currentUserService = currentUserService;
            _viewRenderService = viewRenderService;
        }



        [HttpPost]
        public async Task<IActionResult> UploadAttachmentToLocal(int index, int recordId, string deleteUrl, string attachmentListName)
        {
            try
            {
                
                var modelValidationResult = ModelValidation.Check(ModelState);
                if (!modelValidationResult.Succeeded)
                {
                    return new JsonResult(new { success = false, message = modelValidationResult.Message });
                }
                //Get File from Request
                var files = Request.Form.Files;
                string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/files");

                //create folder if not exist
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);

                
                var attachmentModel = new List<AttachmentViewModel>();
                if (files.Count > 0)
                {
                    foreach (var file in files)
                    {
                        AttachmentViewModel attachment = new AttachmentViewModel();
                        //get file extension
                        FileInfo fileInfo = new FileInfo(file.FileName);
                        string fileNameWithPath = Path.Combine(path, file.FileName);
                        attachment.Location = "/files/" + file.FileName;
                        attachment.FileName = file.FileName;
                        attachment.DummyFileName = file.FileName;
                        attachment.Size = file.Length;
                        attachment.CreatedOn = DateTime.UtcNow;
                        attachment.ModifiedOn = DateTime.UtcNow;
                        attachment.CreatedBy = _currentUserService.UserId;
                        attachment.ModifiedBy = _currentUserService.UserId;
                        using (var stream = new FileStream(fileNameWithPath, FileMode.Create))
                        {
                            file.CopyTo(stream);
                        }
                        attachment.RecordId = recordId;
                        attachment.ActionUrl = deleteUrl;
                        attachment.Index = index;
                        attachment.AttachmentName = attachmentListName;
                        attachmentModel.Add(attachment);
                        index++;
                    }
                }
                string html = await _viewRenderService.RenderToStringAsync("Template/_AttachmentRow", attachmentModel);
                return Json(new
                {
                    success = true,
                    message = "File uploaded successfully.",
                    attachment = html
                });

            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "Something went wrong",
                });
            }

        }

    }
}
