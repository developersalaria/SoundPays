using System.ComponentModel.DataAnnotations.Schema;

namespace SoundpaysAdd.Core.Models
{
    [Table("Devices")]
    public class Device : BaseEntity
    {
        #region Prop
        public int FirebaseId { get; set; }
        public bool IsPaused { get; set; }
        #endregion

    }
}
