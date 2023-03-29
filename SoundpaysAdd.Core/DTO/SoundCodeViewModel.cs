using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoundpaysAdd.Core.DTO
{
    public class SoundCodeViewModel : BaseEntity
    {
        #region Prop
        [Required(ErrorMessage = "Code is required")]
        public string Code { get; set; }
        [Required(ErrorMessage = "Start zone is required")]
        public decimal StartZone { get; set; }
        [Required(ErrorMessage = "End zone is required")]
        public decimal EndZone { get; set; }
        [Required]
        public bool IsPaused { get; set; }

        #endregion
    }
}
