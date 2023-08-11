using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs.SkillDTO
{
    public class StaffSkillCreateDto
    {
        public int StaffId { get; set; }
        public string SkillName { get; set; }
        public string? Level { get; set; }

    }
}