using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs.SkillDTO
{
    public class SkillCreateDto
    {
        public string? SkillName { get; set; }
        public List<StaffSkillCreateDto> StaffSkillCreateDtos { get; set; }
    }
}