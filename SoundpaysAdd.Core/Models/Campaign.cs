using System.ComponentModel.DataAnnotations.Schema;
namespace SoundpaysAdd.Core.Models
{
 [Table("Campaigns")]
    public class Campaign : BaseEntity
    {
        public int AdvertiserId { get; set; }
        public string ShortName { get; set; }
        public string LongName { get; set; }
        public DateTime? StartDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public DateTime? StopDate { get; set; }
        public TimeSpan StopTime { get; set; }
        public decimal CPM { get; set; }
        public int Priority { get; set; }
        public decimal MinImpressions { get; set; }
        public decimal MaxImpressions { get; set; }
        public bool IsPaused { get; set; }       

        #region ForeignKey
        [ForeignKey("AdvertiserId")]
        public Advertiser Advertiser { get; set; }
        #endregion
    }
}
