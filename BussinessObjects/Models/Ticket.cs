using System;
using System.Collections.Generic;

namespace BussinessObjects.Models;

public partial class Ticket
{
    public int TicketId { get; set; }

    public int StaffId { get; set; }

    public int? TicketTypeId { get; set; }

    public string TicketReason { get; set; } = null!;

    public string? TicketFile { get; set; }

    public string TicketStatus { get; set; } = null!;

    public DateTime CreateAt { get; set; }

    public string? ProcessNote { get; set; }

    public int? RespondencesId { get; set; }

    public DateTime? ChangeStatusTime { get; set; }

    public bool? Enable { get; set; }

    public virtual UserInfor Staff { get; set; } = null!;

    public virtual TicketType? TicketType { get; set; }
}
