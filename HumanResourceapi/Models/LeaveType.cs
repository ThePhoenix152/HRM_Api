using System;
using System.Collections.Generic;

namespace HumanResoureapi.Models;

public partial class LeaveType
{
    public int LeaveTypeId { get; set; }

    public string? LeaveTypeName { get; set; }

    public string? LeaveTypeDetail { get; set; }

    public int? LeaveTypeDay { get; set; }

    public bool? IsSalary { get; set; }

    public virtual ICollection<LeaveApplication> LeaveApplications { get; set; } = new List<LeaveApplication>();
}
