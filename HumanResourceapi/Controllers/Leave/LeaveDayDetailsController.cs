using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HumanResoureapi.Models;

namespace HumanResourceapi.Controllers.Leave
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveDayDetailsController : ControllerBase
    {
        private readonly SwpProjectContext _context;

        public LeaveDayDetailsController(SwpProjectContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        [HttpGet]
        public async Task<bool> IsUserExist(int staffId) => await _context.UserInfors.Where(c => c.StaffId == staffId && c.AccountStatus == true).AnyAsync();
        [HttpGet("{staffId}")]
        public async Task<ActionResult<List<LeaveDayDetail>>> GetDetailsOfStaffId(int staffId)
        {
            if (!await IsUserExist(staffId)) return NotFound();
            var list = await _context.LeaveDayDetails.Include(c => c.LeaveType).Include(c => c.Staff).Where(c => c.StaffId == staffId).ToListAsync();
            return list;
        }
        [HttpPut("add/{staffId}")]
        public async Task<ActionResult<LeaveDayDetail>> CreateLeaveDayForStaffId(int staffId)
        {
            if (!await IsUserExist(staffId)) return NotFound();
            if (await _context.LeaveDayDetails.AnyAsync(c => c.StaffId == staffId)) return BadRequest("This staff has already left");
            var user = await _context.UserInfors.Include(c => c.LeaveDayDetails).Where(c => c.StaffId == staffId).FirstOrDefaultAsync();
            bool? gender = (await _context.UserInfors.Where(c => c.StaffId == staffId).FirstOrDefaultAsync()).Gender;
            LeaveDayDetail leaveDayDetailToAdd = new LeaveDayDetail
            {
                StaffId = staffId, LeaveTypeId = 3, DayLeft = 1, ChangeAt = DateTime.Now, CreateAt = DateTime.Now, Year = DateTime.Now.Year
            };
            await _context.AddAsync(leaveDayDetailToAdd);
            if (await _context.SaveChangesAsync() > 0)
            {
                return Ok();
            }
            else
            {
                return BadRequest("Error occured while adding data");
            }
        }
    }
}
