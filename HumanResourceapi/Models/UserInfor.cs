using System;
using System.Collections.Generic;

namespace HumanResourceapi.Models;

public partial class UserInfor
{
    public int StaffId { get; set; }

    public string? Id { get; set; }

    public string? ImageFile { get; set; }

    public string? LastName { get; set; }

    public string? FirstName { get; set; }

    public DateTime? Dob { get; set; }

    public string? Phone { get; set; }

    public bool? Gender { get; set; }

    public string? Address { get; set; }

    public string? Country { get; set; }

    public string? CitizenId { get; set; }

    public int? DepartmentId { get; set; }

    public DateTime? HireDate { get; set; }

    public string? BankAccount { get; set; }

    public string? BankAccountName { get; set; }

    public string? Bank { get; set; }

    public int? WorkTimeByYear { get; set; }

    public bool? IsManager { get; set; }

    public bool? AccountStatus { get; set; }

    public int? UserAccountUserId { get; set; }

    public virtual Department? Department { get; set; }

    public virtual ICollection<LeaveDayDetail> LeaveDayDetails { get; set; } = new List<LeaveDayDetail>();

    public virtual ICollection<LogLeave> LogLeaves { get; set; } = new List<LogLeave>();

    public virtual ICollection<LogOt> LogOts { get; set; } = new List<LogOt>();

    public virtual ICollection<Payslip> Payslips { get; set; } = new List<Payslip>();

    public virtual ICollection<PersonnelContract> PersonnelContracts { get; set; } = new List<PersonnelContract>();

    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

    public virtual UserAccount? UserAccountUser { get; set; }
}
