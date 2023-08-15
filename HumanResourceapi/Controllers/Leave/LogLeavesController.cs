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
    public class LogLeavesController : ControllerBase
    {
        private readonly SwpProjectContext _context;

        public LogLeavesController(SwpProjectContext context)
        {
            _context = context;
        }

        // GET: api/LogLeaves
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LogLeave>>> GetLogLeaves()
        {
          if (_context.LogLeaves == null)
          {
              return NotFound();
          }
            return await _context.LogLeaves.ToListAsync();
        }

        // GET: api/LogLeaves/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LogLeave>> GetLogLeave(int id)
        {
          if (_context.LogLeaves == null)
          {
              return NotFound();
          }
            var logLeave = await _context.LogLeaves.FindAsync(id);

            if (logLeave == null)
            {
                return NotFound();
            }

            return logLeave;
        }

        // PUT: api/LogLeaves/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLogLeave(int id, LogLeave logLeave)
        {
            if (id != logLeave.LeaveLogId)
            {
                return BadRequest();
            }

            _context.Entry(logLeave).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LogLeaveExists(id))
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

        // POST: api/LogLeaves
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<LogLeave>> PostLogLeave(LogLeave logLeave)
        {
          if (_context.LogLeaves == null)
          {
              return Problem("Entity set 'SwpProjectContext.LogLeaves'  is null.");
          }
            _context.LogLeaves.Add(logLeave);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLogLeave", new { id = logLeave.LeaveLogId }, logLeave);
        }

        // DELETE: api/LogLeaves/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLogLeave(int id)
        {
            if (_context.LogLeaves == null)
            {
                return NotFound();
            }
            var logLeave = await _context.LogLeaves.FindAsync(id);
            if (logLeave == null)
            {
                return NotFound();
            }

            _context.LogLeaves.Remove(logLeave);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LogLeaveExists(int id)
        {
            return (_context.LogLeaves?.Any(e => e.LeaveLogId == id)).GetValueOrDefault();
        }
    }
}
