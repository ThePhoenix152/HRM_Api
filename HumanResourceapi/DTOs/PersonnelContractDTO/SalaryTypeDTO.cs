using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs.PersonnelContractDTO
{
    public class SalaryTypeDTO
    {
        public int SalaryTypeId { get; set; }

        public string Name { get; set; } = null!;
    }
}