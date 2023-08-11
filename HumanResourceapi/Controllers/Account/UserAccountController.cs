using HumanResourceapi.Controllers.Account.Login;
using HumanResourceapi.Controllers.Account.UserForm;
using HumanResourceapi.Models;
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
            var user = await _context.UserAccounts.FirstOrDefaultAsync(c => c.Email.Equals(userLogin.Email));
            if (user == null)
            {
                return Unauthorized();
            }
            else
            {
                if (user.Password.Equals(userLogin.Password))
                {
                    return Ok();
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
            var user = await _context.UserAccounts.FirstOrDefaultAsync(c => c.Email.Equals(userRegister.Email));
            if(user != null) 
            {
                return Problem("Account existed!");
            }
            else
            {
                user = new UserAccount { Email = userRegister.Email, Password = userRegister.Password, RoleId = userRegister.RoleId };
                await _context.UserAccounts.AddAsync(user);
            }
            var registerUser = new UserInfor {
                Id = userRegister.Id,
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
            var result = _context.UserInfors.AddAsync(registerUser);
            await _context.SaveChangesAsync();
            return StatusCode(200);
        }
    }
}
