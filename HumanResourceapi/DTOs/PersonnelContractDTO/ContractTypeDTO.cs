using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HumanResourceapi.DTOs.PersonnelContractDTO
{
    public class ContractTypeDTO
    {
        public int ContractTypeId { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }
    }
}