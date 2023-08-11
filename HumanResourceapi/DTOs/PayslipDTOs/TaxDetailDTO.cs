using API.Entities;

namespace API.DTOs.PayslipDTOs
{
    public class TaxDetailDTO
    {
        public int TaxDetailId { get; set; }

        public int? PayslipId { get; set; }

        public int? TaxLevel { get; set; }

        public int? Amount { get; set; }

        //public virtual Payslip? Payslip { get; set; }

        public virtual TaxListDTO? TaxLevelNavigation { get; set; }
    }
}
