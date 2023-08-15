
using AutoMapper;
using HumanResoureapi.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
    public class TheCalendarService
    {
        private readonly SwpProjectContext _context;
        private readonly IMapper _mapper;

        public TheCalendarService(SwpProjectContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ActionResult<List<TheCalendar>>> GetTypeDates(DateTime startDate, DateTime endDate)
        {
            DateTime now = DateTime.Now;

            List<TheCalendar> holidayDimensions = new List<TheCalendar>();
            if (startDate > endDate)
            {
                return holidayDimensions;
            }


            for (DateTime date = startDate; date <= endDate; date = date.AddDays(1))
            {
                var dateType = await _context.TheCalendars
                    .Where(c => date.Equals(c.TheDate)).FirstOrDefaultAsync();


                if (dateType != null)
                {
                    holidayDimensions.Add(dateType);
                }
            }

            return holidayDimensions.ToList();
        }
        public async Task<List<TheCalendar>> GetWorkingDays(DateTime start, DateTime end)
        {
            var workingdays = await _context.TheCalendars
                .Where(c =>
                        c.TheDate >= start &&
                        c.TheDate <= end &&
                        c.IsWorking == 1)
                .ToListAsync();
            return workingdays;
        }

        public async Task<List<TheCalendar>> GetHolidayDays(DateTime start, DateTime end)
        {
            var holidayDays = await _context.TheCalendars
                .Where(c =>
                    c.TheDate >= start &&
                    c.TheDate <= end &&
                    c.IsHoliday == 1)
                .ToListAsync();
            return holidayDays;
        }

        public async Task<List<TheCalendar>> GetWeekendDays(DateTime start, DateTime end)
        {
            var weekendDays = await _context.TheCalendars
                .Where(c =>
                    c.TheDate >= start &&
                    c.TheDate <= end &&
                    c.IsWeekend == 1)
                .ToListAsync();
            return weekendDays;
        }


    }
}
