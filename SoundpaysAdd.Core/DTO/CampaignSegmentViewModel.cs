using System.ComponentModel.DataAnnotations.Schema;
namespace SoundpaysAdd.Core.DTO
{
    
    public class CampaignSegmentViewModel : BaseEntity
    {
        public int CampaignId { get; set; }
        public int SegmentId { get; set; }
    }
}
