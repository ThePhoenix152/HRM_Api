using System;
using System.Collections.Generic;

namespace HumanResoureapi.Models;

public partial class TheCalendar
{
    public DateTime? TheDate { get; set; }

    public int? TheDay { get; set; }

    public string? TheDayName { get; set; }

    public int? TheDayOfWeek { get; set; }

    public byte? TheDayOfWeekInMonth { get; set; }

    public int IsWeekend { get; set; }

    public int? TheWeek { get; set; }

    public int? TheMonth { get; set; }

    public string? TheMonthName { get; set; }

    public DateTime? TheFirstOfMonth { get; set; }

    public int? TheQuarter { get; set; }

    public int? TheYear { get; set; }

    public DateTime? TheFirstOfYear { get; set; }

    public string? Style101 { get; set; }

    public int IsHoliday { get; set; }

    public string? HolidayText { get; set; }

    public int IsWorking { get; set; }

    public decimal Percent { get; set; }
}
