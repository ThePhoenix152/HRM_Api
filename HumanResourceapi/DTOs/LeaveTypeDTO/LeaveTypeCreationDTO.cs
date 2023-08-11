using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs.LeaveTypeDTO
{
    public class LeaveTypeCreationDTO
    {
        // public int LeaveTypeId { get; set; }

        public string? LeaveTypeName { get; set; }

        public string? LeaveTypeDetail { get; set; }

        public int? LeaveTypeMaxDay { get; set; }
    }
}