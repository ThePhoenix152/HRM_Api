using System;
using System.Collections.Generic;

namespace HumanResourceapi.Models;

public partial class TaxList
{
    public int TaxLevel { get; set; }

    public string? Description { get; set; }

    public int? TaxRange { get; set; }

    public double? TaxPercentage { get; set; }

    public virtual ICollection<TaxDetail> TaxDetails { get; set; } = new List<TaxDetail>();
}
