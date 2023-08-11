using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs.PayslipDTOs
{
    public class TaxRateDTO
    {
        public int TaxRate5M { get; set; }
        public int TaxRate5MTo10M { get; set; }
        public int TaxRate10MTo18M { get; set; }
        public int TaxRate18MTo23M { get; set; }
        public int TaxRate23MTo52M { get; set; }
        public int TaxRate52MTo82M { get; set; }
        public int TaxRateOver82M { get; set; }
        public int Total()
        {
            return TaxRate5M + TaxRate5MTo10M + TaxRate10MTo18M + TaxRate18MTo23M + TaxRate23MTo52M + TaxRate52MTo82M + TaxRateOver82M;
        }
    }
}