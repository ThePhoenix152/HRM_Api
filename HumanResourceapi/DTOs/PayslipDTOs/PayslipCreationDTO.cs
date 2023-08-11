﻿using HumanResourceapi.DTOs.StaffDtos;
using HumanResourceapi.DTOs.UserInforDTO;
using HumanResourceapi.Entities;

namespace HumanResourceapi.DTOs.PayslipDTOs
{
    public class PayslipCreationDTO
    {
        //public int PayslipId { get; set; }

        //public int StaffId { get; set; }

        public int? GrossStandardSalary { get; set; }

        public int? GrossActualSalary { get; set; }

        public double? StandardWorkDays { get; set; }

        public double? ActualWorkDays { get; set; }

        public double? LeaveHours { get; set; }

        public double? LeaveDays { get; set; }

        public int? OtTotal { get; set; }

        public int? Bhxhemp { get; set; }

        public int? Bhytemp { get; set; }

        public int? Bhtnemp { get; set; }

        public int? SalaryBeforeTax { get; set; }

        public int? SelfDeduction { get; set; }

        public int? FamilyDeduction { get; set; }

        public int? TaxableSalary { get; set; }

        public int? PersonalIncomeTax { get; set; }

        public int? TotalAllowance { get; set; }

        public int? SalaryRecieved { get; set; }

        public int? NetStandardSalary { get; set; }

        public int? NetActualSalary { get; set; }

        public int? Bhxhcomp { get; set; }

        public int? Bhytcomp { get; set; }

        public int? Bhtncomp { get; set; }

        public int? TotalCompInsured { get; set; }

        public int? TotalCompPaid { get; set; }

        public DateTime? CreateAt { get; set; }

        public DateTime? ChangeAt { get; set; }

        public int? CreatorId { get; set; }

        public int? ChangerId { get; set; }

        public DateTime? Payday { get; set; }

        public bool? Enable { get; set; }

        public string? Status { get; set; }

        public virtual StaffInfoDto Staff { get; set; } = null!;

        public virtual ICollection<TaxDetailDTO> TaxDetails { get; set; } = new List<TaxDetailDTO>();
    }
}
