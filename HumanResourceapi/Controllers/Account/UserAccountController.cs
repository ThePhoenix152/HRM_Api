using HumanResourceapi.Controllers.Account.Login;
using HumanResourceapi.Controllers.Account.UserForm;
using HumanResoureapi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace HumanResourceapi.Controllers.Account
{
    [Route("api/[controller]")]
    [ApiController]

    public class UserAccountController : Controller
    {
        private readonly SwpProjectContext _context;
        public UserAccountController(SwpProjectContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromForm] UserLogin userLogin)
        {
            var user = await _context.UserInfors.FirstOrDefaultAsync(c => c.Email.Equals(userLogin.Email));
            if (user == null)
            {
                return Unauthorized();
            }
            else
            {
                if (user.Password.Equals(userLogin.Password))
                {
                    string directPage = "";
                    if (user.Roleid.Equals("HRM")) directPage = "HRM";
                    else if (user.Roleid.Equals("S")) directPage = "S";
                    else if (user.Roleid.Equals("HRS")) directPage = "HRS";
                    else return BadRequest("Invalid role");
                    return Ok(new {RedirectToPage = directPage});
                }
                else
                {
                    return Problem("Incorrect password");
                }
            }
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] UserRegister userRegister)
        {
            if (!_context.Departments.Any())
            {
                return Problem("No department");
            }
            var user = await _context.UserInfors.FirstOrDefaultAsync(c => c.Email.Equals(userRegister.Email));
            if (user != null)
            {
                return Problem("Account existed!");
            }
            else
            {
                var registerUser = new UserInfor
                {
                    Email = userRegister.Email,
                    Password = userRegister.Password,
                    Roleid = userRegister.RoleId,
                    LastName = userRegister.LastName,
                    FirstName = userRegister.FirstName,
                    Dob = userRegister.Dob,
                    Phone = userRegister.Phone,
                    Gender = userRegister.Gender,
                    Address = userRegister.Address,
                    Country = userRegister.Country,
                    CitizenId = userRegister.CitizenId,
                    DepartmentId = userRegister.DepartmentId,
                    IsManager = userRegister.IsManager,
                    HireDate = DateTime.Now,
                    BankAccount = userRegister.BankAccount,
                    BankAccountName = userRegister.BankAccountName.ToUpper(),
                    Bank = userRegister.Bank,
                    WorkTimeByYear = 0,
                    AccountStatus = true
                };
                await _context.UserInfors.AddAsync(registerUser);
            }

            
            await _context.SaveChangesAsync();
            return StatusCode(200);
        }
    }
}
