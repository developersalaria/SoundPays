using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoundpaysAdd.Core.Models
{
    [Table("Attachments")]
    public class Attachment : BaseEntity
    {
        #region Prop
        [Required]
        public string Location { get; set; }
        [Required]
        public string DummyFileName { get; set; }
        [Required]
        public string FileName { get; set; }
        [Required]
        public long Size { get; set; }
        #endregion

    }
}
