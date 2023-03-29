using System.ComponentModel.DataAnnotations.Schema;
namespace SoundpaysAdd.Core.Models
{
 [Table("Advertisers")]
    public class Advertiser : BaseEntity
    {
        #region Properties

        public string ShortName { get; set; }
        public string LongName { get; set; }
        public string UserId { get; set; }
        public string Email { get; set; }
       
        public bool IsPaused { get; set; }

        #endregion

    }
}
