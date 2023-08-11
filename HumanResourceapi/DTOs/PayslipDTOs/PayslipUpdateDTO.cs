using API.DTOs.StaffDtos;
using API.Entities;

namespace API.DTOs.PayslipDTOs
{
    public class PayslipUpdateDTO
    {
        //public int PayslipId { get; set; }

        //public int StaffId { get; set; }


        public DateTime? ChangeAt { get; set; }
        public int ChangerId { get; set; }
        public bool? Enable { get; set; }

        public string Status { get; set; }

        //public virtual UserInfor Staff { get; set; } = null!;

        //public virtual ICollection<TaxDetail> TaxDetails { get; set; } = new List<TaxDetail>();
    }
}
