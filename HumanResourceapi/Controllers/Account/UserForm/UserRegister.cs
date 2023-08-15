using HumanResoureapi.Models;

namespace HumanResourceapi.Controllers.Account.UserForm
{
    public class UserRegister
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
        public string? Id { get; set; }

        public string? ImageFile { get; set; }

        public string? LastName { get; set; }

        public string? FirstName { get; set; }

        public DateTime? Dob { get; set; }

        public string? Phone { get; set; }

        public bool? Gender { get; set; }

        public string? Address { get; set; }

        public string? Country { get; set; }

        public string? CitizenId { get; set; }

        public int? DepartmentId { get; set; }

        public string? BankAccount { get; set; }

        public string? BankAccountName { get; set; }

        public string? Bank { get; set; }

        public bool? IsManager { get; set; }
    }
}
