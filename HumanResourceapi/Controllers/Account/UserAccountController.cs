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
            var user = await _context.UserAccounts.FindAsync(email);
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
    }
}
