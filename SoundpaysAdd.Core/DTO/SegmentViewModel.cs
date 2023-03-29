
using System.ComponentModel.DataAnnotations;

namespace SoundpaysAdd.Core.DTO
{
    public class SegmentViewModel : BaseEntity
    {
        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
        [Required(ErrorMessage = "Description is required")]
        public string Description { get; set; }
    }
}
