using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs.UserInforDTO;

namespace API.DTOs.DepartmentDTO
{
  public class DepartmentCreateDto
  {
    public string DepartmentName { get; set; }
    public int? ManagerId { get; set; }
    public int[] UserInfors { get; set; }
  }
}