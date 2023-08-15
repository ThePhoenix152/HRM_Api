using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HumanResoureapi.Models;

namespace HumanResourceapi.DTOs.UserInforDTO
{
  public class UserInforDto
  {
    public string StaffId { get; set; }
    public string LastName { get; set; } = null!;
    public string ImageFile { get; set; }
    public string FirstName { get; set; } = null!;

    public string FullName => $"{LastName} {FirstName}";

    public string Email { get; set; }
    public string Position { get; set; }
    public string DepartmentName { get; set; }
    public DateTime? Dob { get; set; }

    public string? Phone { get; set; }

    public bool Gender { get; set; }
    public string GioiTinh => Gender ? "Nam" : "Nữ";

    public string? Address { get; set; }

    public string? Country { get; set; }

    public string CitizenId { get; set; } = null!;

    public int? DepartmentId { get; set; }

    public bool IsManager { get; set; }

    public DateTime HireDate { get; set; }

    public string BankAccount { get; set; } = null!;

    public string BankAccountName { get; set; } = null!;

    public string Bank { get; set; } = null!;

    public int? WorkTimeByYear { get; set; }

    public bool? AccountStatus { get; set; }


    public PersonnelContract? PersonnelContract { get; set; }

  }
}