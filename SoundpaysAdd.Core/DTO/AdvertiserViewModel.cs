using System.ComponentModel.DataAnnotations;

namespace SoundpaysAdd.Core.DTO
{
    public class AdvertiserViewModel:BaseModel
    {
        
        [Required(ErrorMessage = "Please enter Short name")]
        public string ShortName { get; set; }
        [Required(ErrorMessage = "Please enter Long name")]
        public string LongName { get; set; }
        public string? UserId { get; set; }
        [Required(ErrorMessage = "Please enter email")]
        [EmailAddress(ErrorMessage = "Please enter valid email")]
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public bool IsPaused { get; set; }
        [RegularExpression(@"^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$",  
            ErrorMessage = "Password should be minimum 8 characters long and should be Alphanumeric with at least one special character")]
        [Required(ErrorMessage = "Please enter Advertiser password")]
        public string Password { get; set; }
    }
}
 