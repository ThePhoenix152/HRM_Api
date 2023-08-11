using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs.LeaveTypeDTO
{
    public class LeaveTypeLogDTO
    {
        // public int LeaveLogId { get; set; }

        public int StaffId { get; set; }

        // public int? LeaveTypeId { get; set; }

        public DateTime LeaveStart { get; set; }

        public DateTime LeaveEnd { get; set; }

        public double LeaveDays { get; set; }

        public string? Description { get; set; }

        public string? Status { get; set; }

        public DateTime? CreateAt { get; set; }

        public string? ProcessNote { get; set; }

        // public virtual LeaveTypeDTO? LeaveType { get; set; }
    }
}