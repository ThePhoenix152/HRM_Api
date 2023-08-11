using HumanResourceapi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HumanResourceapi.Controllers.Demention
{
    [ApiController]
    [Route("api/date-dimension")]
    public class DateDemen : Controller
    {
        private readonly SwpProjectContext _context;
        public DateDemen(SwpProjectContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));

        }

        [HttpGet]
        public async Task<ActionResult<List<DateDimension>>> GetDateDimensions()
        {
            var DateDimensions = await _context.DateDimensions.ToListAsync();

            return DateDimensions;
        }
    }
}
