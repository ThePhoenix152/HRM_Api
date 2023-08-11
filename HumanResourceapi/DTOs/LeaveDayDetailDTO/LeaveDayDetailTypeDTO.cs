namespace API.DTOs.LeaveDayDetailDTO
{
    public class LeaveDayDetailTypeDTO
    {
        public int LeaveTypeId { get; set; }

        public string? LeaveTypeName { get; set; }

        public string? LeaveTypeDetail { get; set; }

        public int? LeaveTypeMaxDay { get; set; }
    }
}
