using System;
using System.Collections.Generic;

namespace HumanResoureapi.Models;

public partial class LeaveApplication
{
    public int LeaveLogId { get; set; }

    public int StaffId { get; set; }

    public int? LeaveTypeId { get; set; }

    public DateTime LeaveStart { get; set; }

    public DateTime LeaveEnd { get; set; }

    public double? LeaveDays { get; set; }

    public int? LeaveHours { get; set; }

    public int? SalaryPerDay { get; set; }

    public int? Amount { get; set; }

    public string? Description { get; set; }

    public string? Status { get; set; }

    public int? CreatorId { get; set; }

    public DateTime CreateAt { get; set; }

    public string? ProcessNote { get; set; }

    public int? RespondencesId { get; set; }

    public DateTime? ChangeStatusTime { get; set; }

    public bool? Enable { get; set; }

    public virtual LeaveType? LeaveType { get; set; }

    public virtual UserInfor Staff { get; set; } = null!;
}
