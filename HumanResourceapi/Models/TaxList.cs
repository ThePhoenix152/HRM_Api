using System;
using System.Collections.Generic;

namespace HumanResoureapi.Models;

public partial class TaxList
{
    public int TaxLevel { get; set; }

    public string? Description { get; set; }

    public int? TaxRange { get; set; }

    public double? TaxPercentage { get; set; }
}
