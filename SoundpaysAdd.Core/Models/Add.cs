using System.ComponentModel.DataAnnotations.Schema;

namespace SoundpaysAdd.Core.Models
{
    [Table("Adds")]
    public class Add : BaseEntity
    {
        #region Prop
        public int SoundCodeId { get; set; }
        public int CampaignId { get; set; }
        public string ShortName { get; set; }
        public string LongName { get; set; }
        public int AddType { get; set; }
        public decimal MinWidth { get; set; }
        public decimal MaxWidth { get; set; }
        public decimal MinHeight { get; set; }
        public decimal MaxHeight { get; set; }
        public string JSTag { get; set; }
        public DateTime? StartDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public DateTime? StopDate { get; set; }
        public TimeSpan StopTime { get; set; }
        public bool IsPaused { get; set; }
        #endregion

        #region ForeignKey
        [ForeignKey("SoundCodeId")]
        public SoundCode SoundCode { get; set; }
        [ForeignKey("CampaignId")]
        public Campaign Campaign { get; set; }
        #endregion
    }
}
