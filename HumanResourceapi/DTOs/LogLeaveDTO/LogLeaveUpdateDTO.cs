using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs.LogLeaveDTO
{
    public class LogLeaveUpdateDTO
    {
        //public int LeaveLogId { get; set; }

        //public int StaffId { get; set; }

        public int LeaveTypeId { get; set; }

        public DateTime LeaveStart { get; set; }

        public DateTime LeaveEnd { get; set; }

        public double? LeaveDays { get; set; }

        public int? LeaveHours { get; set; }

        public int? SalaryPerDay { get; set; }

        public int? Amount { get; set; }

        public string? Description { get; set; }
        //[RegularExpression("pending|approved|rejected", ErrorMessage = "Status must be 'pending', 'approved', or 'rejected'")]
        public string Status { get; set; }

        public DateTime CreateAt { get; set; }

        public string? ProcessNote { get; set; }

        public int? RespondencesId { get; set; }

        public DateTime? ChangeStatusTime { get; set; }

        public bool? Enable { get; set; }
        public LogLeaveUpdateDTO()
        {
            LeaveDays = (LeaveEnd - LeaveStart).TotalDays;
            LeaveHours = (int?)(LeaveDays * 8);
            CreateAt = DateTime.Now;
            ChangeStatusTime = DateTime.Now;
            Enable = true;
        }

        //public virtual LeaveTypeDTO? LeaveType { get; set; }

        //public virtual UserInfor Staff { get; set; } = null!;
    }
}