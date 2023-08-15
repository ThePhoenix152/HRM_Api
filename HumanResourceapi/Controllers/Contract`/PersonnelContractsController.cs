using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HumanResoureapi.Models;

namespace HumanResourceapi.Controllers.Contract_
{
    [Route("api/[controller]")]
    [ApiController]
    public class PersonnelContractsController : ControllerBase
    {
        private readonly SwpProjectContext _context;

        public PersonnelContractsController(SwpProjectContext context)
        {
            _context = context;
        }

        // GET: api/PersonnelContracts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PersonnelContract>>> GetPersonnelContracts()
        {
          if (_context.PersonnelContracts == null)
          {
              return NotFound();
          }
            return await _context.PersonnelContracts.ToListAsync();
        }

        // GET: api/PersonnelContracts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<PersonnelContract>> GetPersonnelContract(int id)
        {
          if (_context.PersonnelContracts == null)
          {
              return NotFound();
          }
            var personnelContract = await _context.PersonnelContracts.FindAsync(id);

            if (personnelContract == null)
            {
                return NotFound();
            }

            return personnelContract;
        }

        // PUT: api/PersonnelContracts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPersonnelContract(int id, PersonnelContract personnelContract)
        {
            if (id != personnelContract.ContractId)
            {
                return BadRequest();
            }

            _context.Entry(personnelContract).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PersonnelContractExists(id))
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

        // POST: api/PersonnelContracts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<PersonnelContract>> PostPersonnelContract(PersonnelContract personnelContract)
        {
          if (_context.PersonnelContracts == null)
          {
              return Problem("Entity set 'SwpProjectContext.PersonnelContracts'  is null.");
          }
            _context.PersonnelContracts.Add(personnelContract);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPersonnelContract", new { id = personnelContract.ContractId }, personnelContract);
        }

        // DELETE: api/PersonnelContracts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePersonnelContract(int id)
        {
            if (_context.PersonnelContracts == null)
            {
                return NotFound();
            }
            var personnelContract = await _context.PersonnelContracts.FindAsync(id);
            if (personnelContract == null)
            {
                return NotFound();
            }

            _context.PersonnelContracts.Remove(personnelContract);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PersonnelContractExists(int id)
        {
            return (_context.PersonnelContracts?.Any(e => e.ContractId == id)).GetValueOrDefault();
        }
    }
}
