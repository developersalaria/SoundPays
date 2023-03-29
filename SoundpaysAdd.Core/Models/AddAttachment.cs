using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SoundpaysAdd.Core.Models
{
    [Table("AddAttachments")]
    public class AddAttachment : BaseEntity
    {
        #region Prop
        public int AddId { get; set; }
        public string Format { get; set; }
        public int AttachmentId { get; set; }
        #endregion

        #region ForeignKey
        [ForeignKey("AddId")]
        public Add Add { get; set; }

        [ForeignKey("AttachmentId")]
        public Attachment Attachment { get; set; }
        #endregion

    }
}
