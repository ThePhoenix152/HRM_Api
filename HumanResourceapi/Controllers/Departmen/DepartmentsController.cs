using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HumanResoureapi.Models;
using HumanResourceapi.Controllers.Departmen.Form;

namespace HumanResourceapi.Controllers.Departmen
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly SwpProjectContext _context;

        public DepartmentsController(SwpProjectContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        [HttpDelete("remove/{departmentId}")]
        public async Task<ActionResult> RemoveDepartment(int departmentId)
        {
            var department = await _context.Departments
                .FirstOrDefaultAsync(d => d.DepartmentId == departmentId);

            if (department == null) return NotFound();

            _context.Departments.Remove(department);

            var result = await _context.SaveChangesAsync() > 0;

            if (result) return Ok();

            return BadRequest(new ProblemDetails { Title = "Problem unexpected while removing" });
        }
        [HttpPut("add/{departmentName}")]
        public async Task<ActionResult> CreateDeparment(string departmentName)
        {
            Department departmentToAdd = new Department { DepartmentName = departmentName, Status = true };
            await _context.Departments.AddAsync(departmentToAdd);
            var result = await _context.SaveChangesAsync() > 0;
            if (result) return Ok();
            return BadRequest(new ProblemDetails { Title = "Problem unexpected while creating" });
        }
        [HttpPut("update/{departmentId}")]
        public async Task<ActionResult> UpdateDepartment(int departmentId,[FromForm]  DepartmentUpdateForm department)
        {
            var departmentToUpdate = await _context.Departments.FirstOrDefaultAsync(c => c.DepartmentId == departmentId);
            departmentToUpdate.DepartmentName = department.DepartmentName;
            departmentToUpdate.Status = department.Status;
            var result = await _context.SaveChangesAsync() > 0;
            if (result) return Ok();
            return BadRequest(new ProblemDetails { Title = "Problem unexpected while updating" });
        }
    }

}
