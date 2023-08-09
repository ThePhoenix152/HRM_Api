using System;
using System.Collections.Generic;

namespace BussinessObjects.Models;

public partial class LeaveType
{
    public int LeaveTypeId { get; set; }

    public string? LeaveTypeName { get; set; }

    public string? LeaveTypeDetail { get; set; }

    public int? LeaveTypeDay { get; set; }

    public bool? IsSalary { get; set; }

    public virtual ICollection<LeaveDayDetail> LeaveDayDetails { get; set; } = new List<LeaveDayDetail>();

    public virtual ICollection<LogLeave> LogLeaves { get; set; } = new List<LogLeave>();
}
