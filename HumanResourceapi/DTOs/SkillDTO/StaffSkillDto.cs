namespace API.DTOs.SkillDTO
{
  public class StaffSkillDto
  {
    public int UniqueId { get; set; }

    public int StaffId { get; set; }

    public string StaffName { get; set; }

    public string DepartmentName { get; set; }

    public int SkillId { get; set; }

    public string SkillName { get; set; }
    public string? Level { get; set; }

  }
}