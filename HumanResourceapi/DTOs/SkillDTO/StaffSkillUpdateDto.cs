using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs.SkillDTO
{
    public class StaffSkillUpdateDto
    {
        public int UniqueId { get; set;}
        
        public string? Level { get; set; }

        public string? SkillName { get; set; }
    }
}