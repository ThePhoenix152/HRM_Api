using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs.LeaveTypeDTO
{
    public class LeaveTypeWithLogLeavesDTO
    {
        public int LeaveTypeId { get; set; }

        public string? LeaveTypeName { get; set; }

        public string? LeaveTypeDetail { get; set; }

        public int? LeaveTypeMaxDay { get; set; }

        public virtual ICollection<LeaveTypeLogDTO> LogLeaves { get; set; } = new List<LeaveTypeLogDTO>();
    }
}