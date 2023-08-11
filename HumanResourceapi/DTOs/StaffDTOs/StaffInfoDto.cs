using API.DTOs.DepartmentDTO;
using API.Entities;

namespace API.DTOs.StaffDtos
{
  public class StaffInfoDto
  {
    public int StaffId { get; set; }

    public string Id { get; set; }

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

    public DepartmentDto Department { get; set; }
  }
}
