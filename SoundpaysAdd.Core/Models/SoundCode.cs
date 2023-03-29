using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoundpaysAdd.Core.Models
{
    [Table("SoundCodes")]
    public class SoundCode : BaseEntity
    {
        #region Prop
        [Required]
        public string Code { get; set; }
        public decimal StartZone { get; set; }
        [Required]
        public decimal EndZone { get; set; }
        [Required]
        public bool IsPaused { get; set; }

        #endregion
      
    }
}
