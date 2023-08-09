using System;
using System.Collections.Generic;

namespace BussinessObjects.Models;

public partial class TaxDetail
{
    public int TaxDetailId { get; set; }

    public int? PayslipId { get; set; }

    public int? TaxLevel { get; set; }

    public int? Amount { get; set; }

    public virtual Payslip? Payslip { get; set; }

    public virtual TaxList? TaxLevelNavigation { get; set; }
}
