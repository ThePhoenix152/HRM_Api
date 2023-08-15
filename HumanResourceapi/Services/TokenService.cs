using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using HumanResoureapi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;

namespace HumanResourceapi.Services
{
    public class TokenService
    {
        private readonly UserManager<UserInfor> _userManager;
        private readonly IConfiguration _config;
        public TokenService(UserManager<UserInfor> userManager, IConfiguration config)
        {
            _config = config;
            _userManager = userManager;
        }

        //public async Task<string> GenerateToken(UserInfor user)
        //{
        //    var claims = new List<Claim>
        //    {
        //       new Claim(ClaimTypes.Email, user.Email),
        //       new Claim(ClaimTypes.Name, user.e)
        //    };

        //    var roles = await _userManager.GetRolesAsync(user);
        //    foreach(var role in roles)
        //    {
        //        claims.Add(new Claim(ClaimTypes.Role, role));
        //    }

        //    /* Ensure this key never leave the server, this key must be protected by server */
        //    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWTSettings:TokenKey"]));
        //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512);

        //    var tokenOptions = new JwtSecurityToken(
        //        issuer: null,
        //        audience: null,
        //        claims: claims,
        //        expires: DateTime.Now.AddDays(7),
        //        signingCredentials: creds
        //    );

        //    return new JwtSecurityTokenHandler().WriteToken(tokenOptions);
        //}
    }
}