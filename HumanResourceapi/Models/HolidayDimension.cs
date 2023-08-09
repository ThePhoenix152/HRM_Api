using System;
using System.Collections.Generic;

namespace HumanResourceapi.Models;

public partial class HolidayDimension
{
    public DateTime TheDate { get; set; }

    public string HolidayText { get; set; } = null!;

    public virtual DateDimension TheDateNavigation { get; set; } = null!;
}
