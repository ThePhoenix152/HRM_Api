using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.CandidateSkillDTO;

namespace API.DTOs.SkillDTO
{
    public class SkillCandidateCreateDto
    {
        public string? SkillName { get; set; }
        
        public int StaffId { get; set; }

        public string Level { get; set; }
    }
}