using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Entities;

namespace API.DTOs.SkillDTO
{
    public class SkillUpdateDto
    {
        public string? SkillName { get; set; }

        public List<StaffSkillDto>? StaffSkills { get; set; }
    }
}