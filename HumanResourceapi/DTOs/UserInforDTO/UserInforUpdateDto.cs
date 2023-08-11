using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs.UserInforDTO
{
  public class UserInforUpdateDto
  {
    public string LastName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string Email { get; set; }

    public int? DeparatmentId { get; set; }

    public DateTime? Dob { get; set; }

    public string? Phone { get; set; }

    public bool Gender { get; set; }

    public string? Address { get; set; }

    public string? Country { get; set; }

    public string CitizenId { get; set; } = null!;

    public string? BankAccount { get; set; }

    public string? BankAccountName { get; set; }

    public string? Bank { get; set; }
    public bool? AccountStatus { get; set; }
  }
}