using Microsoft.AspNetCore.Mvc.Rendering;
using SoundpaysAdd.Core.Helpers;
using SoundpaysAdd.Core.Validations;
using System.ComponentModel.DataAnnotations;

namespace SoundpaysAdd.Core.DTO
{
    public class AddViewModel : BaseEntity
    {
        [Required(ErrorMessage = "Sound code is required")]
        public int SoundCodeId { get; set; }
        [Required(ErrorMessage = "Campaign is required")]
        public int CampaignId { get; set; }
        [Required(ErrorMessage = "Short name is required")]
        public string ShortName { get; set; }
        [Required(ErrorMessage = "Long name is required")]
        public string LongName { get; set; }
        [Required(ErrorMessage = "Add type is required")]
        public int AddType { get; set; }

        [Required(ErrorMessage = "Min width is required")]
        [MinMaxCheck("MaxWidth", "decimal", ErrorMessage = "Min width should be less then Max width")]
        public decimal MinWidth { get; set; }

        [Required(ErrorMessage = "Max width is required")]
        [MinMaxCheck("MinWidth", "decimal", "greater", ErrorMessage = "Max width should be greater then Min width")]
        public decimal MaxWidth { get; set; }

        [Required(ErrorMessage = "Min height is required")]
        [MinMaxCheck("MaxHeight", "decimal", ErrorMessage = "Mix height should be less then Max height")]
        public decimal MinHeight { get; set; }


        [Required(ErrorMessage = "Max height is required")]
        [MinMaxCheck("MinHeight", "decimal", "greater", ErrorMessage = "Max height should be greater then Min height")]
        public decimal MaxHeight { get; set; }
        [Required(ErrorMessage = "JS tag is required")]
        public string JSTag { get; set; }
        [Required(ErrorMessage = "Start date is required")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM-dd-yyyy}")]
        [MinMaxCheck("StopDate", "date", ErrorMessage = "Start date should be less then End date")]
        public DateTime? StartDate { get; set; }
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "Start time is required")]
        public string StartTimeStandardString => DateTimeExt.StandardDateTime(StartTime);

        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM-dd-yyyy}")]
        [Required(ErrorMessage = "Stop date is required")]
        [MinMaxCheck("StartDate", "date", "greater", ErrorMessage = "Stop date should be greater then Start date")]
        public DateTime? StopDate { get; set; }
        public TimeSpan StopTime { get; set; }

        [Required(ErrorMessage = "Stop time is required")]
        public string StopTimeStandardString => DateTimeExt.StandardDateTime(StopTime);

        public bool IsPaused { get; set; }
        public string? SoundCodeName { get; set; }
        public string? CampaignName { get; set; }
        public string? AddTypeName { get; set; }
        public string? StartDateDisplay => StartDate.HasValue ? StartDate.Value.ToString("d") : "";
        public string? StopDateDisplay => StopDate.HasValue ? StopDate.Value.ToString("d") : "";
        public List<SelectListItem>? SoundCodeList { get; set; }
        public List<SelectListItem>? CampaignList { get; set; }
        public List<SelectListItem>? AddTypeList { get; set; }
        public List<AttachmentViewModel> AttachmentListSD { get; set; }
        public List<AttachmentViewModel> AttachmentListHD { get; set; }
        public List<AttachmentViewModel> AttachmentListFHD { get; set; }
        public AddViewModel()
        {
            AttachmentListSD = new List<AttachmentViewModel>();
            AttachmentListHD = new List<AttachmentViewModel>();
            AttachmentListFHD = new List<AttachmentViewModel>();
        }
    }
}
