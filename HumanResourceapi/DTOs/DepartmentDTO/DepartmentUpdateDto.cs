using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.UserInforDTO;

namespace API.DTOs.DepartmentDTO
{
  public class DepartmentUpdateDto
  {
    public string? DepartmentName { get; set; }
    public List<UserInforDto>? UserInfors { get; set; }
    public int? ManagerId { get; set; }
  }
}