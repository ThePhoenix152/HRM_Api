using API.DTOs.UserInforDTO;

namespace API.DTOs.UserDTO
{
    public class UserDto
    {
        public string Email { get; set; }
        public string Token { get; set; }

        public UserInforDto UserInfor { get; set;}
    }
}