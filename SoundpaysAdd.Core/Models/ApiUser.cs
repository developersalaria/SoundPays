using System.ComponentModel.DataAnnotations.Schema;
namespace SoundpaysAdd.Core.Models
{
    [Table("ApiUsers")]
    public class ApiUser : BaseEntity
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string ApiKey { get; set; }
        public string ClientKey { get; set; }
        public int? AdvertiserId { get; set; }

        #region ForeignKey
        [ForeignKey("AdvertiserId")]
        public Advertiser Advertiser { get; set; }
        #endregion
    }
}
