using System;
using System.Collections.Generic;

namespace HumanResoureapi.Models;

public partial class Payslip
{
    public int PayslipId { get; set; }

    public int StaffId { get; set; }

    public int? GrossStandardSalary { get; set; }

    public int? GrossActualSalary { get; set; }

    public double? StandardWorkDays { get; set; }

    public double? ActualWorkDays { get; set; }

    public double? LeaveHours { get; set; }

    public double? LeaveDays { get; set; }

    public int? OtTotal { get; set; }


    public int? SelfDeduction { get; set; }

    public int? FamilyDeduction { get; set; }



    public int? TotalAllowance { get; set; }

    public int? SalaryRecieved { get; set; }

    public int? NetStandardSalary { get; set; }

    public int? NetActualSalary { get; set; }

    public int? TotalCompInsured { get; set; }

    public int? TotalCompPaid { get; set; }

    public DateTime? CreateAt { get; set; }

    public DateTime? ChangeAt { get; set; }

    public int? CreatorId { get; set; }

    public int? ChangerId { get; set; }

    public DateTime? Payday { get; set; }

    public bool? Enable { get; set; }

    public string? Status { get; set; }

    public virtual UserInfor Staff { get; set; } = null!;
}
