namespace API.DTOs.UserInforDTO
{
    public class LeaveTypeDto
    {
        public int LeaveTypeId { get; set; }

        public string? LeaveTypeName { get; set; }

        public string? LeaveTypeDetail { get; set; }

        public int? LaveTypeMaxDay { get; set; }
        public List<LeaveDayLeftDto> LeaveDayLefts { get; set; }
    }
}