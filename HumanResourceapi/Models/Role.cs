using System;
using System.Collections.Generic;

namespace HumanResourceapi.Models;

public partial class Role
{
    public string RoleId { get; set; } = null!;

    public string? RoleName { get; set; }

    public virtual ICollection<UserAccount> UserAccounts { get; set; } = new List<UserAccount>();
}
