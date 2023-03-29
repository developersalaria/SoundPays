using System.ComponentModel.DataAnnotations.Schema;

namespace SoundpaysAdd.Core.Models
{
    [Table("Events")]
    public class Event : BaseEntity
    {
        #region Prop
        public int AddId { get; set; }
        public int CampaignId { get; set; }
        public int AdvertiserId { get; set; }
        public int DeviceId { get; set; }
        public string Action { get; set; }
        public DateTime StartDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public DateTime StopDate { get; set; }
        public TimeSpan StopTime { get; set; }
        #endregion

        #region ForeignKey
        [ForeignKey("AddId")]
        public Add Add { get; set; }

        [ForeignKey("CampaignId")]
        public Campaign Campaign { get; set; }
        [ForeignKey("AdvertiserId")]
        public Advertiser Advertiser { get; set; }
        [ForeignKey("DeviceId")]
        public Device Device { get; set; }
        #endregion

    }
}
