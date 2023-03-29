using System.ComponentModel.DataAnnotations.Schema;
namespace SoundpaysAdd.Core.Models
{
    [Table("CampaignSegments")]
    public class CampaignSegment : BaseEntity
    {
        public int CampaignId { get; set; }
        public int SegmentId { get; set; }

        #region ForeignKey
        [ForeignKey("CampaignId")]
        public Campaign Campaign { get; set; }

        [ForeignKey("SegmentId")]
        public Segment Segment { get; set; }
        #endregion
    }
}
