using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs.SkillDTO
{
    public class SkillDto
    {
        public int SkillId { get; set; }

        public string? SkillName { get; set; }

        public List<StaffSkillDto>? StaffSkills {get; set;}
    }
}