using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HumanResoureapi.Models;
using HumanResourceapi.Controllers.Allow.Form;

namespace HumanResourceapi.Controllers.Allow
{
    [Route("api/[controller]")]
    [ApiController]
    public class AllowancesController : ControllerBase
    {
        private readonly SwpProjectContext _context;
        public AllowancesController(SwpProjectContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        [HttpGet("getAll")]
        public async Task<ActionResult<List<Allowance>>> GetAllowanceListAsync()
        {
            var list = await _context.Allowances.ToListAsync();
            if (list.Count == 0 ) return BadRequest("No allowanace data");
            return list;
        }
        
        [HttpGet("contracts/{contractId}")]
        public async Task<ActionResult<List<Allowance>>> GetAllowanceAsync(int contractId)
        {
            if (!await _context.PersonnelContracts.AnyAsync(c => c.ContractId == contractId))
            {
                return NotFound();
            }
            var result = await _context.Allowances.Include(c => c.AllowanceType).Where(c => c.ContractId == contractId).ToListAsync();
            return result;
        }
        [HttpGet("{allowanceId}/contracts/{contractId}", Name = "GetAllowance")]
        public async Task<ActionResult<List<Allowance>>> GetAllowanceAsync(int contractId, int allowanceId)
        {
            if (!await _context.PersonnelContracts.AnyAsync(c => c.ContractId == contractId))
            {
                return NotFound();
            }
            return await _context.Allowances.Include(c => c.AllowanceType).Where(c => c.ContractId == contractId && c.AllowanceId == allowanceId).ToListAsync();
        }
        [HttpPost("contracts/{contractId}")]
        public async Task<ActionResult<Allowance>> CreateAllowanceAsync(int contractId, Allowance allowance)
        {
            if (!await _context.PersonnelContracts.AnyAsync(c => c.ContractId == contractId))
            {
                return NotFound();
            }
            if (!await _context.AllowanceTypes.AnyAsync(c => c.AllowanceTypeId == allowance.AllowanceTypeId))
            {
                return BadRequest("Invalid allowance");
            }
            if (!await _context.Allowances.AnyAsync(c => c.ContractId == contractId && c.AllowanceTypeId == allowance.AllowanceTypeId))
            {
                return BadRequest("We already have this allowance");
            }
            var allowanceFromStore = await _context.PersonnelContracts.Include(c => c.Allowances).Where(c => c.ContractId == contractId).FirstOrDefaultAsync();
            allowanceFromStore.Allowances.Add(allowance);
            await _context.SaveChangesAsync();
            return Ok();
        }
        
        [HttpPut("contracts/{allowanceId}/{contractId}")]
        public async Task<ActionResult<Allowance>> UpdateAllowanceAsync(int contractId, int allowanceId, [FromForm] AllowanceUpdateForm allowance)
        {
            if (!await _context.PersonnelContracts.AnyAsync(c => c.ContractId == contractId))
            {
                return NotFound();
            }
            if (!await _context.AllowanceTypes.AnyAsync(c => c.AllowanceTypeId == allowance.AllowanceTypeId))
            {
                return BadRequest("Invalid allowance");
            }
            var allowanceToUpdate = await _context.Allowances.Where(c => c.ContractId == contractId && c.AllowanceId == allowanceId).FirstOrDefaultAsync();
            allowanceToUpdate.AllowanceTypeId = allowance.AllowanceTypeId;
            allowanceToUpdate.AllowanceSalary = allowance.AllowanceSalary;
            _context.Allowances.Update(allowanceToUpdate);
            await _context.SaveChangesAsync();
            return Ok();
        }
        [HttpDelete("{allowanceId}")]
        public async Task<ActionResult<Allowance>> DeleteAllowanceAsync(int allowanceId)
        {
            var allowanceToDelete = await _context.Allowances.Where(c => c.AllowanceId == allowanceId).FirstOrDefaultAsync();
            if (allowanceToDelete == null) return BadRequest("Khong ton tai phuc loi");
            _context.Allowances.Remove(allowanceToDelete);
            await _context.SaveChangesAsync();
            return Ok(allowanceToDelete.AllowanceId);
        }
    }
}
