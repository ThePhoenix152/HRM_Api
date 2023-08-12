using System.ComponentModel.DataAnnotations;

namespace HumanResourceapi.Services
{
    public class PayslipInputCreationDto
    {
        [Required(ErrorMessage = "Month is required.")]
        [Range(1, 12, ErrorMessage = "Month must be between 1 and 12.")]
        public int Month { get; set; }
        public int Year { get; set; }   
        public int CreatorId { get; set; }
        public int? ChangerId { get; set; }
    }
}
