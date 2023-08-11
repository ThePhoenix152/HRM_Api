using API.Entities;

namespace API.DTOs.LogOtDTOs
{
    public class OtTypeDTO
    {
        public int OtTypeId { get; set; }

        public string? TypeName { get; set; }

        public double? TypePercentage { get; set; }

        //public virtual ICollection<LogOt> LogOts { get; set; } = new List<LogOt>();

        //public virtual ICollection<OtDetail> OtDetails { get; set; } = new List<OtDetail>();
    }
}
