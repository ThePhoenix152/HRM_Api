using HumanResourceapi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HumanResourceapi.Controllers.Account
{
    [Route("api/[controller]")]
    [ApiController]

    public class UserAccountController : Controller
    {
        private readonly SwpProjectContext _context;
        public UserAccountController(SwpProjectContext context)
        {
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _context.UserAccounts.FirstOrDefaultAsync(c => c.Email.Equals(email));
            if (user == null)
            {
                return Problem("Incorrect account email");
            }
            else
            {
                if (user.Password.Equals(password))
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
        public async Task<IActionResult> Register(string email, string password)
        {
            if (!_context.Departments.Any())
            {
                return Problem("No department");
            }
            var user = await _context.UserAccounts.FirstOrDefaultAsync(c => c.Email.Equals(email));
            if(user! == null) 
            {
                return Problem("Account existed!");
            }
            var registerUser = new UserAccount { Email = email, Password = password };
            var result = _context.UserAccounts.AddAsync(registerUser);
            await _context.SaveChangesAsync();
            return StatusCode(200);
        }
    }
}
