using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SoundpaysAdd.Core.Helpers;
using SoundpaysAdd.Core.Models;
using SoundpaysAdd.Core.Validations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SoundpaysAdd.Core.DTO
{
    public class CampaignViewModel : BaseEntity
    {
        [Required(ErrorMessage = " Please select Advertiser")]
        public int AdvertiserId { get; set; }
        public string? AdvertiserShortName { get; set; }
        public string? AdvertiserLongName { get; set; }
        [Required(ErrorMessage = "Please enter Short name ")]
        public string ShortName { get; set; }

        [Required(ErrorMessage = " Please enter Long name")]
        public string LongName { get; set; }

        [DisplayName("Start Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM-dd-yyyy}")]
        [Required(ErrorMessage = " Please select Start date")]
        public DateTime? StartDate { get; set; }
        [DisplayName("Start Time")]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = " Please select Start time")]
        public string StartTimeStandardString => DateTimeExt.StandardDateTime(StartTime);

        [DisplayName("Stop Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM-dd-yyyy}")]
        [Required(ErrorMessage = " please select Stop date")]
        public DateTime? StopDate { get; set; }

        public string? StartDateDisplay => StartDate.HasValue ? StartDate.Value.ToString("d") : "";
        public string? StopDateDisplay => StopDate.HasValue ? StopDate.Value.ToString("d") : "";
        [DisplayName("Stop Time")]
        public TimeSpan StopTime { get; set; }

        [Required(ErrorMessage = " Please select Stop time")]
        public string StopTimeStandardString => DateTimeExt.StandardDateTime(StopTime);


        [Required(ErrorMessage = " Please select CPM")]
        public decimal CPM { get; set; }
        [Required(ErrorMessage = " please select Priority")]
        public int Priority { get; set; }
        [MinMaxCheck("MaxImpressions", "decimal", ErrorMessage = "Min Impressions should be less then Max Impressions")]
        [Required(ErrorMessage = " Please enter Min impressions")]
        public decimal MinImpressions { get; set; }

        [MinMaxCheck("MinImpressions", "decimal", "greater", ErrorMessage = "Max Impressions should be greater then Min Impressions")]
        [Required(ErrorMessage = " Please enter Max impressions")]
        public decimal MaxImpressions { get; set; }
        public bool IsPaused { get; set; }
        public List<SelectListItem>? AdvertiserList { get; set; }
        [DisplayName("Segements")]
        public int[]? SegementIdArray { get; set; }
        public MultiSelectList? SegementList { get; set; }

    }
}
