using System;
using System.Collections.Generic;

namespace BussinessObjects.Models;

public partial class TicketType
{
    public int TicketTypeId { get; set; }

    public string? TicketName { get; set; }

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}
