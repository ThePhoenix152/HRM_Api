namespace API.DTOs.PayslipDTOs
{
    public class TaxListDTO
    {
        public int TaxLevel { get; set; }

        public string? Description { get; set; }

        public int? TaxRange { get; set; }

        public double? TaxPercentage { get; set; }
    }
}
