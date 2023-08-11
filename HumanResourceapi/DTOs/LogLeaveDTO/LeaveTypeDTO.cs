using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HumanResourceapi.DTOs.LogLeaveDTO {

    public class LeaveTypeDTO
    {
        public int LeaveTypeId { get; set; }

        public string? LeaveTypeName { get; set; }

        public string? LeaveTypeDetail { get; set; }

        public int? LeaveTypeMaxDay { get; set; }
    }
}
