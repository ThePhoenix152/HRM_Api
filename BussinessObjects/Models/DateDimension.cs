using System;
using System.Collections.Generic;

namespace BussinessObjects.Models;

public partial class DateDimension
{
    public DateTime? TheDate { get; set; }

    public int? TheDay { get; set; }

    public string? TheDayName { get; set; }

    public byte? TheDayOfWeekInMonth { get; set; }

    public int IsWeekend { get; set; }

    public int? TheWeek { get; set; }

    public int? TheDayOfWeek { get; set; }

    public int? TheMonth { get; set; }

    public string? TheMonthName { get; set; }

    public int? TheQuarter { get; set; }

    public int? TheYear { get; set; }

    public DateTime? TheFirstOfYear { get; set; }

    public DateTime? TheFirstOfMonth { get; set; }

    public string? Style101 { get; set; }

    public int UniqueId { get; set; }
}
