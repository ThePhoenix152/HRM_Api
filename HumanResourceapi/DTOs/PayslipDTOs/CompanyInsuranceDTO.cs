using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs.PayslipDTOs
{
    public class CompanyInsuranceDTO : InsuranceDTO
    {
        public decimal GrossSalary { get; set; }
        public decimal TotalInsurance { get; set; }
        public decimal NetSalary { get; set;}
    }
}