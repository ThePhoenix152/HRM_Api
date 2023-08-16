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
                    var newUsersIn30Days = await _context.UserInfors.Where(c => c.HireDate <= DateTime.Now && c.HireDate >= DateTime.Now.AddDays(-30) && c.IsManager == false).CountAsync();
                    var totalApplcationsIn30Days = await _context.LeaveApplications.Where(c => c.CreateAt <= DateTime.Now && c.CreateAt >= DateTime.Now.AddDays(-30)).CountAsync()
                        + await _context.Otapplications.Where(c => c.CreateAt <= DateTime.Now && c.CreateAt >= DateTime.Now.AddDays(-30)).CountAsync();
                    var totalAllowancesIn30Days = await _context.Payslips.Select(c => c.TotalAllowance).SumAsync();
                    if (user.Roleid == 1) return Ok(new {RedirectToPage = "HRManager", NewUsersIn30Days = newUsersIn30Days, ApplicationsIn30Days = totalApplcationsIn30Days, AllowancesIn30Days = totalAllowancesIn30Days});
                    else if (user.Roleid == 2) return Ok(new { RedirectToPage = "HRStaff", NewUsersIn30Days = newUsersIn30Days, ApplicationsIn30Days = totalApplcationsIn30Days, AllowancesIn30Days = totalAllowancesIn30Days });
                    else if (user.Roleid == 3) return Ok(new { RedirectToPage = "Staff"});
                    else return BadRequest("Invalid role");
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
