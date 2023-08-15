using HumanResourceapi.DTOs.StaffDtos;
using HumanResoureapi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HumanResourceapi.DTOs.LogOtDTOs
{
    public class LogOtDTO
    {
        public int OtLogId { get; set; }

        public int StaffId { get; set; }

        public int? OtTypeId { get; set; }

        public DateTime LogStart { get; set; }

        public DateTime LogEnd { get; set; }

        public double LogHours { get; set; }

        public int? Days { get; set; }

        public int? SalaryPerDay { get; set; }

        public int? Amount { get; set; }

        public string? Reason { get; set; }

        public string? Status { get; set; }

        public string? ProcessNote { get; set; }

        public int? RespondencesId { get; set; }

        public DateTime CreateAt { get; set; }

        public DateTime? ChangeStatusTime { get; set; }

        public bool? Enable { get; set; }

        public virtual OtTypeDTO? OtType { get; set; }
        public virtual StaffInfoDto Staff { get; set; } = null!;


    }
}