using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HumanResoureapi.Models;

namespace HumanResourceapi.Controllers.OT
{
    [Route("api/[controller]")]
    [ApiController]
    public class OtTypesController : ControllerBase
    {
        private readonly SwpProjectContext _context;

        public OtTypesController(SwpProjectContext context)
        {
            _context = context;
        }

        // GET: api/OtTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OtType>>> GetOtTypes()
        {
          if (_context.OtTypes == null)
          {
              return NotFound();
          }
            return await _context.OtTypes.ToListAsync();
        }

        // GET: api/OtTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<OtType>> GetOtType(int id)
        {
          if (_context.OtTypes == null)
          {
              return NotFound();
          }
            var otType = await _context.OtTypes.FindAsync(id);

            if (otType == null)
            {
                return NotFound();
            }

            return otType;
        }

        // PUT: api/OtTypes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOtType(int id, OtType otType)
        {
            if (id != otType.OtTypeId)
            {
                return BadRequest();
            }

            _context.Entry(otType).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OtTypeExists(id))
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

        // POST: api/OtTypes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<OtType>> PostOtType(OtType otType)
        {
          if (_context.OtTypes == null)
          {
              return Problem("Entity set 'SwpProjectContext.OtTypes'  is null.");
          }
            _context.OtTypes.Add(otType);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOtType", new { id = otType.OtTypeId }, otType);
        }

        // DELETE: api/OtTypes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOtType(int id)
        {
            if (_context.OtTypes == null)
            {
                return NotFound();
            }
            var otType = await _context.OtTypes.FindAsync(id);
            if (otType == null)
            {
                return NotFound();
            }

            _context.OtTypes.Remove(otType);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool OtTypeExists(int id)
        {
            return (_context.OtTypes?.Any(e => e.OtTypeId == id)).GetValueOrDefault();
        }
    }
}
