using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HumanResourceapi.Models;

namespace HumanResourceapi.Controllers.Leave
{
    [Route("api/[controller]")]
    [ApiController]
    public class LeaveDayDetailsController : ControllerBase
    {
        private readonly SwpProjectContext _context;

        public LeaveDayDetailsController(SwpProjectContext context)
        {
            _context = context;
        }

        // GET: api/LeaveDayDetails
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LeaveDayDetail>>> GetLeaveDayDetails()
        {
          if (_context.LeaveDayDetails == null)
          {
              return NotFound();
          }
            return await _context.LeaveDayDetails.ToListAsync();
        }

        // GET: api/LeaveDayDetails/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LeaveDayDetail>> GetLeaveDayDetail(int id)
        {
          if (_context.LeaveDayDetails == null)
          {
              return NotFound();
          }
            var leaveDayDetail = await _context.LeaveDayDetails.FindAsync(id);

            if (leaveDayDetail == null)
            {
                return NotFound();
            }

            return leaveDayDetail;
        }

        // PUT: api/LeaveDayDetails/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutLeaveDayDetail(int id, LeaveDayDetail leaveDayDetail)
        {
            if (id != leaveDayDetail.LeaveDayDetailId)
            {
                return BadRequest();
            }

            _context.Entry(leaveDayDetail).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!LeaveDayDetailExists(id))
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

        // POST: api/LeaveDayDetails
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<LeaveDayDetail>> PostLeaveDayDetail(LeaveDayDetail leaveDayDetail)
        {
          if (_context.LeaveDayDetails == null)
          {
              return Problem("Entity set 'SwpProjectContext.LeaveDayDetails'  is null.");
          }
            _context.LeaveDayDetails.Add(leaveDayDetail);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetLeaveDayDetail", new { id = leaveDayDetail.LeaveDayDetailId }, leaveDayDetail);
        }

        // DELETE: api/LeaveDayDetails/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteLeaveDayDetail(int id)
        {
            if (_context.LeaveDayDetails == null)
            {
                return NotFound();
            }
            var leaveDayDetail = await _context.LeaveDayDetails.FindAsync(id);
            if (leaveDayDetail == null)
            {
                return NotFound();
            }

            _context.LeaveDayDetails.Remove(leaveDayDetail);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool LeaveDayDetailExists(int id)
        {
            return (_context.LeaveDayDetails?.Any(e => e.LeaveDayDetailId == id)).GetValueOrDefault();
        }
    }
}
