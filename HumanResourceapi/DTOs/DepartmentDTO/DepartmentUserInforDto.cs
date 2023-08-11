using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.DTOs.DepartmentDTO
{
    public class DepartmentUserInforDto
    {
        public int StaffId { get; set; }

        public string Id { get; set; }

        public string LastName { get; set; } = null!;

        public string FirstName { get; set; } = null!;

        public string FullName { get; set; }
        
        public DateTime? Dob { get; set; }

        public string? Phone { get; set; }

        public bool Gender { get; set; }

        public string? Address { get; set; }

        public string? Country { get; set; }

        public string CitizenId { get; set; } = null!;

        public int? DepartmentId { get; set; }

        public bool? IsManager { get; set; }

        public DateTime HireDate { get; set; }

        public string BankAccount { get; set; } = null!;

        public string BankAccountName { get; set; } = null!;

        public string Bank { get; set; } = null!;

        public int? LeaveDayLeft { get; set; }

        public int? WorkTimeByYear { get; set; }

        public bool? AccountStatus { get; set; }
        //public  DepartmentDto2 Department { get; set; }

    }
}