
namespace SoundpaysAdd.Core.DTO
{
    public class ApiUserViewModel : BaseEntity
    {
        public string? ClientId { get; set; }
        public string? ClientSecret { get; set; }
        public string? ApiKey { get; set; }
        public string? ClientKey { get; set; }
        public int? AdvertiserId { get; set; }
      
    }
}
