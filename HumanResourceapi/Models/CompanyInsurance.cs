namespace HumanResourceapi.Models
{
    public class CompanyInsurance
    {
        public decimal SocialInsurance { get; set; }
        public decimal UnemploymentInsurance { get; set; }
        public decimal HealthInsurance { get; set; }


        public decimal GrossSalary { get; set; }
        public decimal TotalInsurance { get; set; }
        public decimal NetSalary { get; set; }
    }

}
