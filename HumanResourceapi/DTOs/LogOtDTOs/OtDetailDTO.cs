using API.Entities;

namespace API.DTOs.LogOtDTOs
{
    public class OtDetailDTO
    {
        public int OtDetailId { get; set; }

        public int? StaffId { get; set; }

        public int? OtTypeId { get; set; }

        public int? OtHours { get; set; }

        public int? OtAmount { get; set; }

        public DateTime? Time { get; set; }

        //public virtual OtType? OtType { get; set; }

        //public virtual UserInfor? Staff { get; set; }
    }
}
