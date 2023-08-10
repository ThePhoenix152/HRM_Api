using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HumanResourceapi.Models;

namespace HumanResourceapi.Controllers.Payslit
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayslipsController : ControllerBase
    {
        private readonly SwpProjectContext _context;

        public PayslipsController(SwpProjectContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // GET: api/Payslips
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Payslip>>> GetPayslips()
        {
          if (_context.Payslips == null)
          {
              return NotFound();
          }
            return await _context.Payslips.ToListAsync();
        }

        // GET: api/Payslips/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Payslip>> GetPayslip(int id)
        {
          if (_context.Payslips == null)
          {
              return NotFound();
          }
            var payslip = await _context.Payslips.FindAsync(id);

            if (payslip == null)
            {
                return NotFound();
            }

            return payslip;
        }

        // PUT: api/Payslips/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPayslip(int id, Payslip payslip)
        {
            if (id != payslip.PayslipId)
            {
                return BadRequest();
            }

            _context.Entry(payslip).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PayslipExists(id))
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

        // POST: api/Payslips
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Payslip>> PostPayslip(Payslip payslip)
        {
          if (_context.Payslips == null)
          {
              return Problem("Entity set 'SwpProjectContext.Payslips'  is null.");
          }
            _context.Payslips.Add(payslip);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPayslip", new { id = payslip.PayslipId }, payslip);
        }

        // DELETE: api/Payslips/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePayslip(int id)
        {
            if (_context.Payslips == null)
            {
                return NotFound();
            }
            var payslip = await _context.Payslips.FindAsync(id);
            if (payslip == null)
            {
                return NotFound();
            }

            _context.Payslips.Remove(payslip);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PayslipExists(int id)
        {
            return (_context.Payslips?.Any(e => e.PayslipId == id)).GetValueOrDefault();
        }
    }
}
