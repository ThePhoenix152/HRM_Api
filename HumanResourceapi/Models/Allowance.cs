using System;
using System.Collections.Generic;

namespace HumanResoureapi.Models;

public partial class Allowance
{
    public int AllowanceId { get; set; }

    public int? ContractId { get; set; }

    public int? AllowanceTypeId { get; set; }

    public int? AllowanceSalary { get; set; }

    public virtual AllowanceType? AllowanceType { get; set; }

    public virtual PersonnelContract? Contract { get; set; }
}
