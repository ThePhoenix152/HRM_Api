using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HumanResourceapi.DTOs.PayslipDTOs
{
    public class InsuranceDTO
    {
        public decimal SocialInsurance { get; set; }
        public decimal UnemploymentInsurance { get; set; }
        public decimal HealthInsurance { get; set; }

 
    }
}