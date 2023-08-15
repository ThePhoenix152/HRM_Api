using System;
using System.Collections.Generic;

namespace HumanResoureapi.Models;

public partial class UserTax
{
    public int? PayslipId { get; set; }

    public int? TaxLevel { get; set; }

    public virtual Payslip? Payslip { get; set; }

    public virtual TaxList? TaxLevelNavigation { get; set; }
}
