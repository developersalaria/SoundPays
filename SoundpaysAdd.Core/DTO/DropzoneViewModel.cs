using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundpaysAdd.Core.DTO
{
    public class DropzoneViewModel
    {
        public int RecordId { get; set; } //id of the item with the attachment is assosiated
        public string? AttachmentName { get; set; } //Model List name
        public string DestinationControl { get; set; } //
        public string AttachmentContainer { get; set; } //id of element to  show list of attchamnets
        public string DeleteUrl { get; set; } // url that will delete the attachmnet
        public string Url { get; set; } // file upload url
        public string DropzoneId { get; set; } //dropzone id
        public string FileType { get; set; } // type of file like video, image etc
        public bool IsReadOnly { get; set; }
        public List<AttachmentViewModel> AttachmentList { get; set; } // list of the attachments
    }
}
