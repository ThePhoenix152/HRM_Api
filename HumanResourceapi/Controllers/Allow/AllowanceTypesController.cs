using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HumanResourceapi.Models;

namespace HumanResourceapi.Controllers.Allow
{
    [Route("api/[controller]")]
    [ApiController]
    public class AllowanceTypesController : ControllerBase
    {
        private readonly SwpProjectContext _context;

        public AllowanceTypesController(SwpProjectContext context)
        {
            _context = context;
        }

        // GET: api/AllowanceTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AllowanceType>>> GetAllowanceTypes()
        {
            if (_context.AllowanceTypes == null)
            {
                return NotFound();
            }
            return await _context.AllowanceTypes.ToListAsync();
        }

        // GET: api/AllowanceTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<AllowanceType>> GetAllowanceType(int id)
        {
            if (_context.AllowanceTypes == null)
            {
                return NotFound();
            }
            var allowanceType = await _context.AllowanceTypes.FindAsync(id);

            if (allowanceType == null)
            {
                return NotFound();
            }

            return allowanceType;
        }

        // PUT: api/AllowanceTypes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAllowanceType(int id, AllowanceType allowanceType)
        {
            if (id != allowanceType.AllowanceTypeId)
            {
                return BadRequest();
            }

            _context.Entry(allowanceType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AllowanceTypeExists(id))
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

        // POST: api/AllowanceTypes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<AllowanceType>> PostAllowanceType(AllowanceType allowanceType)
        {
            if (_context.AllowanceTypes == null)
            {
                return Problem("Entity set 'SwpProjectContext.AllowanceTypes'  is null.");
            }
            _context.AllowanceTypes.Add(allowanceType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAllowanceType", new { id = allowanceType.AllowanceTypeId }, allowanceType);
        }

        // DELETE: api/AllowanceTypes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAllowanceType(int id)
        {
            if (_context.AllowanceTypes == null)
            {
                return NotFound();
            }
            var allowanceType = await _context.AllowanceTypes.FindAsync(id);
            if (allowanceType == null)
            {
                return NotFound();
            }

            _context.AllowanceTypes.Remove(allowanceType);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AllowanceTypeExists(int id)
        {
            return (_context.AllowanceTypes?.Any(e => e.AllowanceTypeId == id)).GetValueOrDefault();
        }
    }
}
