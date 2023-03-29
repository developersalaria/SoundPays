using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundpaysAdd.Core.DTO
{
    public class AttachmentViewModel : BaseModel
    {
        public string Location { get; set; }
        public string DummyFileName { get; set; }
        public string FileName { get; set; }
        public long Size { get; set; }
        public int? Index { get; set; }
        public int? RecordId { get; set; }
        public string? ActionUrl { get; set; }
        public string? AttachmentName { get; set; }
        public bool IsReadOnly { get; set; }
    }
}
