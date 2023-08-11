using API.DTOs.UserInforDTO;
using API.Entities;

namespace API.DTOs.TicketDTO
{
  public class TicketDto
  {
    public int TicketId { get; set; }

    public int StaffId { get; set; }

    public string StaffName { get; set; }

    public int? TicketTypeId { get; set; }
    public string TicketName { get; set; }

    public string TicketReason { get; set; }

    public string? TicketFile { get; set; }

    public string TicketStatus { get; set; }

    public string? ProcessNote { get; set; }

    public bool? Enable { get; set; }

    public int? RespondencesId { get; set; }
    public string? ResponsdenceName { get; set; }

    public DateTime CreateAt { get; set; }

    public DateTime? ChangeStatusTime { get; set; }

    public UserInfor UserInfor { get; set; }
  }
}