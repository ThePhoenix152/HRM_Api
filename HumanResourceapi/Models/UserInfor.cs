using System;
using System.Collections.Generic;

namespace HumanResoureapi.Models;

public partial class UserInfor
{
    public int StaffId { get; set; }

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

    public string? Password { get; set; }

    public int? Roleid { get; set; }

    public string? Email { get; set; }

    public virtual Department? Department { get; set; }

    public virtual ICollection<LeaveApplication> LeaveApplications { get; set; } = new List<LeaveApplication>();

    public virtual ICollection<Otapplication> Otapplications { get; set; } = new List<Otapplication>();

    public virtual ICollection<Payslip> Payslips { get; set; } = new List<Payslip>();

    public virtual ICollection<PersonnelContract> PersonnelContracts { get; set; } = new List<PersonnelContract>();

    public virtual Role? Role { get; set; }
}
