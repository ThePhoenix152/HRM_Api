using HumanResourceapi.Models;
using Microsoft.AspNetCore.Identity;

namespace HumanResourceapi.Models
{
    public class User : IdentityUser
    {   
        public virtual UserInfor UserInfor { get; set; }
    }
}