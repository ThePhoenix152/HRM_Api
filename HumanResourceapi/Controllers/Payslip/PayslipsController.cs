using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HumanResourceapi.Models;
using HumanResourceapi.Controllers.Form;

namespace HumanResourceapi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayslipsController : ControllerBase
    {
        private readonly SwpProjectContext _context;

        public PayslipsController(SwpProjectContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        [HttpGet("{staffId}")]
        public async Task<ActionResult<List<Payslip>>> GetPayslipWithStaffId(int staffId)
            => await _context.Payslips.Include(c => c.Staff).ThenInclude(c => c.Department).Include(c => c.TaxDetails).ThenInclude(c => c.TaxLevelNavigation).Where(c => c.StaffId == staffId).OrderByDescending(c => c.PayslipId).ToListAsync();
        [HttpGet("{payslipId}/staff/{staffId}")]
        public async Task<ActionResult<Payslip>> GetPayslipWithStaffIdAndPayslipId(int payslipId, int staffId)
        {
            if (!await _context.Payslips.AnyAsync(c => c.PayslipId == payslipId && c.StaffId == staffId)) return BadRequest("Khong ton tai");
            return await _context.Payslips.Include(c => c.Staff).ThenInclude(c => c.Department).Include(c => c.TaxDetails).ThenInclude(c => c.TaxLevelNavigation).Where(c => c.StaffId == staffId && c.PayslipId == payslipId).FirstOrDefaultAsync();
        }
        [HttpPut("staff/add/{staffId}")]
        public async Task<ActionResult<Payslip>> CreatePayslipByStaffId(int staffId, [FromForm] PayslipCreationForm payslipCreationForm)
        {
            if (!await _context.UserInfors.Where(c => c.StaffId == staffId && c.AccountStatus == true).AnyAsync())
            {
                return BadRequest("User is not exist");
            }
            if (await _context.PersonnelContracts.AnyAsync(c => c.ContractStatus == true && c.StaffId == staffId) == false)
            {
                return BadRequest("User doesn't have any contracts");
            }
            return Ok();
        }
        public async Task<Payslip> AddPayslip(int staffId, [FromForm] PayslipCreationForm payslipCreationForm)
        {
            var userInfo = await _context.UserInfors.Include(c => c.Payslips).Where(c => c.StaffId == staffId && c.AccountStatus == true).FirstOrDefaultAsync();
            if (userInfo == null) return null;
            int stdPayDay = 25;
            //gross to net
            var personnelContract = await _context.PersonnelContracts.Where(c => c.StaffId == staffId && c.ContractStatus == true).FirstOrDefaultAsync();
            int paidByDate = await BasicSalaryOneDayOfMonth(staffId, payslipCreationForm.Month, payslipCreationForm.Year);
            //gross std salary
            var stdGrossSalary = personnelContract.Salary;
            var allowanceSalary = await GetAllowancesOfStaff(staffId);
            var leavesDeductedSalary = await GetDeductedSalary(staffId, paidByDate, payslipCreationForm.Month, payslipCreationForm.Year);
            //gross actual salary
            var actualGrossSalary = stdGrossSalary + allowanceSalary - leavesDeductedSalary;
            //taxable salary
            var stdTaxableSalary = personnelContract.TaxableSalary;
            var actualTaxableSalary = stdTaxableSalary + allowanceSalary - leavesDeductedSalary;
            //calculation days
            int stdWorkDays = await GetStandardWorkDays(payslipCreationForm.Month, payslipCreationForm.Year);
            int actualWorkDays = await GetActualWorkDaysOfStaff(staffId, payslipCreationForm.Month, payslipCreationForm.Year);
            int leaveDays = await GetLeaveDays(staffId, payslipCreationForm.Month, payslipCreationForm.Year);
            int leaveHours = await GetLeavesHours(staffId, payslipCreationForm.Month, payslipCreationForm.Year);
            //total gross actual salary
            return null;
        }
        public static int countWorkDays(int year, int month)
        {
            int totalDays = DateTime.DaysInMonth(year, month);
            int workDays = 0;
            for (int day = 1; day <= totalDays; day++)
            {
                DateTime date = new DateTime(year, month, day);
                if (date.DayOfWeek != DayOfWeek.Sunday && date.DayOfWeek != DayOfWeek.Saturday)
                {
                    workDays++;
                }
            }
            return workDays;
        }
        public async Task<int> GetStandardWorkDays(int month, int year)
        {
            var StandardWorkDays = await _context.TheCalendars
                .Where(c =>
                    c.IsWorking == 1 &&
                    c.TheMonth == month &&
                    c.TheYear == year)
                .CountAsync();

            return StandardWorkDays;
        }

        public async Task<int> BasicSalaryOneDayOfMonth(int staffId, int month, int year)
        {
            var personnelContract = await _context.PersonnelContracts
                .Where(c => c.StaffId == staffId && c.ContractStatus == true)

                .FirstOrDefaultAsync();

            var standardWorkDays = await _context.TheCalendars
                                            .Where(c =>
                                                c.IsWeekend == 0 &&
                                                c.TheMonth == month &&
                                                c.TheYear == year)
                                            .ToListAsync();

            int totalDays = standardWorkDays.Count;
            int salaryOneDay = 0;

            if (personnelContract != null && personnelContract.SalaryType.Contains("Gross To Net"))
            {
                var basicSalary = personnelContract.Salary;
                salaryOneDay = basicSalary / totalDays;
            }
            Console.WriteLine("Here");
            return salaryOneDay;
        }
        public async Task<int> GetAllowancesOfStaff(int staffId)
        {
            var personnelContractId = await _context.PersonnelContracts
                .Where(c =>
                    c.StaffId == staffId &&
                    c.ContractStatus == true
                    )
                .Select(c => c.ContractId)
                .FirstOrDefaultAsync();

            var allowance = await _context.Allowances
                                        .Where(c => c.ContractId == personnelContractId)
                                        .ToListAsync();


            int? allowanceSalary = 0;

            foreach (var item in allowance)
            {
                allowanceSalary += item.AllowanceSalary;
            }

            return (int)allowanceSalary;


        }
        public async Task<int> GetActualWorkDaysOfStaff(
            int staffId, int month, int year)
        {
            var basicActualWorkDays = await GetStandardWorkDays(month, year);

            var otDays = await GetOtDays(staffId, month, year);

            var leaveDays = await GetLeaveDays(staffId, month, year);




            int totalWorkingDays = basicActualWorkDays + otDays - leaveDays;

            if (totalWorkingDays < 0)
            {
                return 0;
            }
            else
            {
                return totalWorkingDays;
            }
        }
        public async Task<int> GetOtDays(int staffId, int month, int year)
        {
            var logOts = await _context.LogOts
                .Where(c =>
                    c.StaffId == staffId &&
                    c.LogStart.Month == month &&
                    c.LogStart.Year == year &&
                    c.Status == "approved")
                .ToListAsync();

            var logOtDays = logOts.Sum(c => c.LogHours) / 8;

            return (int)logOtDays;
        }
        public async Task<int> GetLeaveDays(int staffId, int month, int year)
        {
            var logLeaves = await _context.LogLeaves
                .Where(c =>
                c.StaffId == staffId &&
                c.Status.ToLower().Contains("approved") &&
                c.LeaveStart.Month >= month &&
                c.LeaveEnd.Month <= month)
                .ToListAsync();

            int sum = 0;

            foreach (var logLeave in logLeaves)
            {
                if (logLeave.LeaveStart.Month == month && logLeave.LeaveEnd.Month == month && logLeave.LeaveStart.Year == year)
                {
                    sum += (int)logLeave.LeaveDays;
                }
                else
                {
                    var startDate = GetStartDay(month, logLeave.LeaveStart);
                    var endDate = GetEndDay(month, logLeave.LeaveEnd);

                    var days = await GetWorkingDays(startDate, endDate);

                    sum += days.Count;
                }
            }


            return (int)sum;
        }
        public async Task<int> GetLeavesHours(int staffId, int month, int year)
        {
            var logLeaves = await _context.LogLeaves
                    .Where(c =>
                        c.StaffId == staffId &&
                        c.Status == "approved" &&
                        month >= c.LeaveStart.Month &&
                        month <= c.LeaveEnd.Month
                        )
                    .ToListAsync();

            int leaveHours = 0;

            foreach (var item in logLeaves)
            {
                DateTime startDay = GetStartDay(month, item.LeaveStart);
                DateTime endDay = GetEndDay(month, item.LeaveEnd);

                leaveHours += (endDay.Day - startDay.Day) + 1;
            }

            return leaveHours * 8;
        }
        public async Task<List<TheCalendar>> GetWorkingDays(DateTime start, DateTime end)
        {
            var workingdays = await _context.TheCalendars
                .Where(c =>
                        c.TheDate >= start &&
                        c.TheDate <= end &&
                        c.IsWorking == 1)
                .ToListAsync();
            return workingdays;
        }


        public async Task<int> GetDeductedSalary(int staffId, int paidByDate, int month, int year)
        {
            var logLeaves = await _context.LogLeaves
                .Where(c =>
                c.StaffId == staffId &&
                c.Status.Contains("approved") &&
                c.LeaveTypeId == 3 &&
                c.LeaveStart.Month <= month &&
                c.LeaveEnd.Month >= month &&
                c.LeaveStart.Year == year)
                .ToListAsync();

            var leaveDays = 0;

            foreach (var item in logLeaves)
            {
                DateTime startDay = GetStartDay(month, item.LeaveStart);
                DateTime endDay = GetEndDay(month, item.LeaveEnd);


                var workingDays = await _context.TheCalendars.Where(c => c.TheDate >= startDay && c.TheDate <= endDay && c.IsWorking == 1).ToListAsync();

                leaveDays = workingDays.Count;
            }

            int totalDeductedSalary = leaveDays * paidByDate;

            return totalDeductedSalary;
        }
        public DateTime GetStartDay(int month, DateTime start)
        {
            if (month < 1 || month > 12)
            {

                throw new ArgumentException("Invalid Month");
            }
            if (month < start.Month)
            {

                throw new ArgumentException("Invalid Month");

            }

            if (month == start.Month)
            {
                return start;
            }
            else
            {

                DateTime firstDateOfMonth = new DateTime(start.Year, start.Month, 1);
                return firstDateOfMonth;
            }
        }

        public DateTime GetEndDay(int month, DateTime end)
        {
            if (month < 1 || month > 12)
            {

                throw new ArgumentException("Invalid Month");
            }

            if (month > end.Month)
            {

                throw new ArgumentException("Invalid Month");
            }

            if (month == end.Month)
            {

                return end;
            }
            else
            {
                int daysInMonth = DateTime.DaysInMonth(end.Year, month);

                DateTime lastDayOfMonth = new DateTime(end.Year, month, daysInMonth);

                return lastDayOfMonth;
            }
        }
    }
}
