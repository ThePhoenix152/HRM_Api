using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HumanResourceapi.Models;

namespace Allow
{
    [Route("api/[controller]")]
    [ApiController]
    public class AllowancesController : ControllerBase
    {
        private readonly SwpProjectContext _context;

        public AllowancesController(SwpProjectContext context)
        {
            _context = context;
        }

        // GET: api/Allowances
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Allowance>>> GetAllowances()
        {
          if (_context.Allowances == null)
          {
              return NotFound();
          }
            return await _context.Allowances.ToListAsync();
        }

        // GET: api/Allowances/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Allowance>> GetAllowance(int id)
        {
          if (_context.Allowances == null)
          {
              return NotFound();
          }
            var allowance = await _context.Allowances.FindAsync(id);

            if (allowance == null)
            {
                return NotFound();
            }

            return allowance;
        }

        // PUT: api/Allowances/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAllowance(int id, Allowance allowance)
        {
            if (id != allowance.AllowanceId)
            {
                return BadRequest();
            }

            _context.Entry(allowance).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AllowanceExists(id))
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

        // POST: api/Allowances
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Allowance>> PostAllowance(Allowance allowance)
        {
          if (_context.Allowances == null)
          {
              return Problem("Entity set 'SwpProjectContext.Allowances'  is null.");
          }
            _context.Allowances.Add(allowance);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAllowance", new { id = allowance.AllowanceId }, allowance);
        }

        // DELETE: api/Allowances/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAllowance(int id)
        {
            if (_context.Allowances == null)
            {
                return NotFound();
            }
            var allowance = await _context.Allowances.FindAsync(id);
            if (allowance == null)
            {
                return NotFound();
            }

            _context.Allowances.Remove(allowance);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AllowanceExists(int id)
        {
            return (_context.Allowances?.Any(e => e.AllowanceId == id)).GetValueOrDefault();
        }
    }
}
