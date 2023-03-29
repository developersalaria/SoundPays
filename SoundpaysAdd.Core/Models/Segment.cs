using System.ComponentModel.DataAnnotations.Schema;
namespace SoundpaysAdd.Core.Models
{
    [Table("Segments")]
    public class Segment : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }

    }
}
