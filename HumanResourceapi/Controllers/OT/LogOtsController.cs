using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HumanResourceapi.Models;

namespace HumanResourceapi.Controllers.OT
{
    [Route("api/[controller]")]
    [ApiController]
    public class LogOtsController : ControllerBase
    {
        private readonly SwpProjectContext _context;

        public LogOtsController(SwpProjectContext context)
        {
            _context = context;
        }

        // GET: api/LogOts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LogOt>>> GetLogOts()
        {
          if (_context.LogOts == null)
          {
              return NotFound();
          }
            return await _context.LogOts.ToListAsync();
        }

        // GET: api/LogOts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LogOt>> GetLogOt(int id)
        {
          if (_context.LogOts == null)
          {
              return NotFound();
          }
            var logOt = await _context.LogOts.FindAsync(id);

            if (logOt == null)
            {
                return NotFound();
            }

            return logOt;
        }

        // PUT: api/LogOts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLogOt(int id, LogOt logOt)
        {
            if (id != logOt.OtLogId)
            {
                return BadRequest();
            }

            _context.Entry(logOt).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LogOtExists(id))
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

        // POST: api/LogOts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<LogOt>> PostLogOt(LogOt logOt)
        {
          if (_context.LogOts == null)
          {
              return Problem("Entity set 'SwpProjectContext.LogOts'  is null.");
          }
            _context.LogOts.Add(logOt);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLogOt", new { id = logOt.OtLogId }, logOt);
        }

        // DELETE: api/LogOts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLogOt(int id)
        {
            if (_context.LogOts == null)
            {
                return NotFound();
            }
            var logOt = await _context.LogOts.FindAsync(id);
            if (logOt == null)
            {
                return NotFound();
            }

            _context.LogOts.Remove(logOt);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LogOtExists(int id)
        {
            return (_context.LogOts?.Any(e => e.OtLogId == id)).GetValueOrDefault();
        }
    }
}
