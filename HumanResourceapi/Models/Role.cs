using System;
using System.Collections.Generic;

namespace HumanResoureapi.Models;

public partial class Role
{
    public int RoleId { get; set; }

    public string? RoleName { get; set; }

    public virtual ICollection<UserInfor> UserInfors { get; set; } = new List<UserInfor>();
}
