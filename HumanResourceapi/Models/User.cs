using Microsoft.AspNetCore.Identity;

namespace API.Entities
{
    public class User : IdentityUser
    {   
        public virtual UserInfor UserInfor { get; set; }
    }
}