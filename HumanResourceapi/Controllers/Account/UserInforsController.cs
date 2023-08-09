using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HumanResourceapi.Models;

namespace HumanResourceapi.Controllers.Account
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserInforsController : ControllerBase
    {
        private readonly SwpProjectContext _context;

        public UserInforsController(SwpProjectContext context)
        {
            _context = context;
        }

        // GET: api/UserInfors
        [HttpGet]
        public async Task<ActionResult<IEnumerable<UserInfor>>> GetUserInfors()
        {
          if (_context.UserInfors == null)
          {
              return NotFound();
          }
            return await _context.UserInfors.ToListAsync();
        }

        // GET: api/UserInfors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<UserInfor>> GetUserInfor(int id)
        {
          if (_context.UserInfors == null)
          {
              return NotFound();
          }
            var userInfor = await _context.UserInfors.FindAsync(id);

            if (userInfor == null)
            {
                return NotFound();
            }

            return userInfor;
        }

        // PUT: api/UserInfors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUserInfor(int id, UserInfor userInfor)
        {
            if (id != userInfor.StaffId)
            {
                return BadRequest();
            }

            _context.Entry(userInfor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserInforExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/UserInfors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<UserInfor>> PostUserInfor(UserInfor userInfor)
        {
          if (_context.UserInfors == null)
          {
              return Problem("Entity set 'SwpProjectContext.UserInfors'  is null.");
          }
            _context.UserInfors.Add(userInfor);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetUserInfor", new { id = userInfor.StaffId }, userInfor);
        }

        // DELETE: api/UserInfors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUserInfor(int id)
        {
            if (_context.UserInfors == null)
            {
                return NotFound();
            }
            var userInfor = await _context.UserInfors.FindAsync(id);
            if (userInfor == null)
            {
                return NotFound();
            }

            _context.UserInfors.Remove(userInfor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserInforExists(int id)
        {
            return (_context.UserInfors?.Any(e => e.StaffId == id)).GetValueOrDefault();
        }
    }
}
