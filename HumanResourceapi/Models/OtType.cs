using System;
using System.Collections.Generic;

namespace HumanResoureapi.Models;

public partial class OtType
{
    public int OtTypeId { get; set; }

    public string? TypeName { get; set; }

    public double? TypePercentage { get; set; }

    public virtual ICollection<Otapplication> Otapplications { get; set; } = new List<Otapplication>();
}
