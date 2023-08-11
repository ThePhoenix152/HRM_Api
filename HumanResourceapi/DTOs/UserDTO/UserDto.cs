using HumanResourceapi.DTOs.UserInforDTO;

namespace HumanResourceapi.DTOs.UserDTO
{
    public class UserDto
    {
        public string Email { get; set; }
        public string Token { get; set; }

        public UserInforDto UserInfor { get; set;}
    }
}