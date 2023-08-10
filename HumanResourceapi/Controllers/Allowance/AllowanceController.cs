﻿using HumanResourceapi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Contracts;

namespace HumanResourceapi.Controllers.Allowances
{
    public class AllowanceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        private readonly SwpProjectContext _context;
        public AllowanceController(SwpProjectContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        [HttpGet]
        public async Task<ActionResult<List<Allowance>>> GetAllowancesAsync() => await _context.Allowances.Include(c => c.AllowanceType).ToListAsync();
        [HttpGet("contracts/{contractId}")]
        public async Task<ActionResult<List<Allowance>>> GetAllowanceAsync(int contractId)
        {
            if (!await _context.PersonnelContracts.AnyAsync(c => c.ContractId == contractId))
            {
                return NotFound();
            }
            return await _context.Allowances.Include(c => c.AllowanceType).Where(c => c.ContractId == contractId).ToListAsync();
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
            if(!await _context.AllowanceTypes.AnyAsync(c => c.AllowanceTypeId == allowance.AllowanceTypeId))
            {
                return BadRequest("Invalid allowance");
            }
            if(!await _context.Allowances.AnyAsync(c => c.ContractId == contractId && c.AllowanceTypeId == allowance.AllowanceTypeId))
            {
                return BadRequest("We already have this allowance");
            }
            var allowanceFromStore = await GetPersonnelContractAllowance(contractId);
            allowanceFromStore.Allowances.Add(allowance);
            await _context.SaveChangesAsync();
            return Ok();
        }
        public async Task<PersonnelContract?> GetPersonnelContractAllowance(int contractId) => await _context.PersonnelContracts.Include(c => c.Allowances).Where(c => c.ContractId == contractId).FirstOrDefaultAsync();
    }
}