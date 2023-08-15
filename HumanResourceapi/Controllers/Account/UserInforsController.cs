using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HumanResoureapi.Models;
using Microsoft.AspNetCore.Identity;
using HumanResourceapi.Extensions;

namespace HumanResourceapi.Controllers.Account
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserInforsController : ControllerBase
    {
        private readonly SwpProjectContext _context;
        

        public UserInforsController(SwpProjectContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            
        }     
        [HttpGet("{staffid}", Name = "GetUserInforById")]
        public async Task<ActionResult<UserInfor>> GetUserInforById(int staffid)
        {
            var userInfor = await _context.UserInfors.FirstOrDefaultAsync(u => u.StaffId == staffid);
            if (userInfor == null) return NotFound();
            return userInfor;
        }

        [HttpGet("filters")]
        public async Task<IActionResult> GetFilter()
        {
            var departments = await _context.UserInfors
                .Select(c => c.Department.DepartmentName)
                .Distinct()
                .ToListAsync();
            return Ok(departments);
        }
    }

}
