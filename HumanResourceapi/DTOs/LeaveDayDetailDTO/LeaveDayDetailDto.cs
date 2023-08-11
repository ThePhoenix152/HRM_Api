
using API.DTOs.UserInforDTO;
using API.Entities;

namespace API.DTOs.LeaveDayDetailDTO
{
    public class LeaveDayDetailDTO
    {
        public int LeaveDayDetailId { get; set; }

        public int? StaffId { get; set; }

        public int? LeaveTypeId { get; set; }

        public int? DayLeft { get; set; }

        public DateTime? ChangeAt { get; set; }

        public virtual LeaveDayDetailTypeDTO? LeaveType { get; set; }

        public virtual UserInforDto? Staff { get; set; }
    }
}
