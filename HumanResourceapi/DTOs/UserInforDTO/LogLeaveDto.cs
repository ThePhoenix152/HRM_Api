namespace API.DTOs.UserInforDTO
{
    public class LogLeaveDto
    {
        public int LeaveLogId { get; set; }

        public int StaffId { get; set; }

        public int? LeaveTypeId { get; set; }

        public DateTime LeaveStart { get; set; }

        public DateTime LeaveEnd { get; set; }

        public double LeaveDays { get; set; }

        public string? Description { get; set; }

        public string? Status { get; set; }

        public DateTime? CreateDate { get; set; }

        public string ResponsdenceName { get; set; }

        public virtual LeaveTypeDto? LeaveType { get; set; }
    }
}