using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HumanResourceapi.DTOs.UserInforDTO;

namespace HumanResourceapi.DTOs.DepartmentDTO
{
  public class DepartmentCreateDto
  {
    public string DepartmentName { get; set; }
    public int? ManagerId { get; set; }
    public int[] UserInfors { get; set; }
  }
}