using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HumanResourceapi.DTOs.PersonnelContractDTO
{
    public class SalaryTypeDTO
    {
        public int SalaryTypeId { get; set; }

        public string Name { get; set; } = null!;
    }
}