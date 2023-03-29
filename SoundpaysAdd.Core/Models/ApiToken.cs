using System.ComponentModel.DataAnnotations.Schema;


namespace SoundpaysAdd.Core.Models
{
    [Table("ApiTokens")]
    public class ApiToken : BaseEntity
    {
        public string ClientId { get; set; }
        public string TokenId { get; set; }
        public DateTime ValidUntil { get; set; }

    }
}
