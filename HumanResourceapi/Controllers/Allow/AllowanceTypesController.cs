using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HumanResoureapi.Models;


namespace HumanResourceapi.Controllers.Allow
{
    [Route("api/[controller]")]
    [ApiController]
    public class AllowanceTypesController : ControllerBase
    {
        private readonly SwpProjectContext _context;

        public AllowanceTypesController(SwpProjectContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        [HttpGet("valid/contracts/{contractId}")]
        public async Task<ActionResult<List<AllowanceType>>> GetValidAllowanceTypesOfContract(int contractId)
        {
            var allowances = await _context.Allowances
                 .Where(c => c.ContractId == contractId).ToListAsync();

            var allowancesInContract = allowances.Select(c => c.AllowanceTypeId);
            var validAllowances = await _context.AllowanceTypes.Where(c => !allowancesInContract.Contains(c.AllowanceTypeId)).ToListAsync();
            return validAllowances;
        }

    }
}
