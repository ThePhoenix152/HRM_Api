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
        private static int PersonalTaxDeduction = 11000000;
        private static int FamilyAllowances = 4400000;

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
        [HttpPost("staff/add/{staffId}")]
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
            var (payslip, result) = await AddPayslip(staffId, payslipCreationForm);
            if (payslip == null)
            {
                return BadRequest("Error when creating payslip");
            }
            
            var payslipInfo = await _context.Payslips.Include(c => c.TaxDetails).ThenInclude(c => c.TaxLevelNavigation).Where(c => c.StaffId == staffId && c.PayslipId == payslip.PayslipId).FirstOrDefaultAsync();
            foreach(var taxDetail in result)
            {
                payslipInfo.TaxDetails.Add(taxDetail);
            }
            return Ok();
        }
        [HttpPut("staff/update/{payslipId}")]
        public async Task<ActionResult<Payslip>> UpdatePayslip(int payslipId, [FromForm] PayslipUpdateForm payslipUpdateForm)
        {
            if(!await _context.Payslips.AnyAsync(c => c.PayslipId == payslipId))
            {
                return BadRequest("Staff doesn't have payslips");
            }
            var payslipToUpdate = await _context.Payslips.Where(c => c.PayslipId == payslipId).FirstOrDefaultAsync();
            payslipToUpdate.ChangeAt = DateTime.UtcNow.AddHours(7);
            await _context.SaveChangesAsync();
            return NoContent();
        }
        [HttpGet]
        public async Task<(Payslip, List<TaxDetail>)> AddPayslip(int staffId, [FromForm] PayslipCreationForm payslipCreationForm)
        {
            var userInfo = await _context.UserInfors.Include(c => c.Payslips).Where(c => c.StaffId == staffId && c.AccountStatus == true).FirstOrDefaultAsync();
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
            //total gross actual salary - family deduction
            int selfDeduction = PersonalTaxDeduction;
            int familyDeduction = await GetFamilyAllowance(staffId);
            int taxableIncome = await TaxableIncomeCalculation(actualGrossSalary, staffId);
            //thue tncn
            List<TaxDetail> resultPersonalIncomeTax = PersonalIncomeTaxCalculate(taxableIncome);
            int personalIncomeTax = (int)resultPersonalIncomeTax.Sum(c => c.Amount);
            //net salary
            int stdNetSalary = actualGrossSalary - personalIncomeTax;
            //thuc nhan
            int otSalary = await OtSalary(staffId, payslipCreationForm.Month, payslipCreationForm.Year);
            int actualNetSalary = stdNetSalary + otSalary;
            //ng su dung lao dong tra
            CompanyInsurance companyInsuranceDTO = CompanyInsuranceCalculate(actualGrossSalary, (int)actualTaxableSalary);
            int actualCompanyPaid = (int)(companyInsuranceDTO.NetSalary + otSalary);

            DateTime payDay = new DateTime(payslipCreationForm.Year, payslipCreationForm.Month, stdPayDay);
            Payslip payslip = new Payslip
            {
                GrossStandardSalary = stdGrossSalary,
                GrossActualSalary = actualGrossSalary,
                StandardWorkDays = stdWorkDays,
                ActualWorkDays = actualWorkDays,
                LeaveHours = leaveHours,
                LeaveDays = leaveDays,
                OtTotal = otSalary,
                SalaryBeforeTax = actualGrossSalary,
                SelfDeduction = selfDeduction,
                FamilyDeduction = familyDeduction,
                TaxableSalary = actualTaxableSalary,
                PersonalIncomeTax = personalIncomeTax,
                TotalAllowance = allowanceSalary,
                SalaryRecieved = actualNetSalary,
                NetStandardSalary = stdNetSalary,
                NetActualSalary = actualNetSalary,
                TotalCompPaid = actualCompanyPaid,
                CreateAt = DateTime.UtcNow.AddHours(7),
                ChangeAt = DateTime.UtcNow.AddHours(7),
                CreatorId = payslipCreationForm.CreatorId,
                ChangerId = payslipCreationForm.ChangerId,
                Status = "pending",
                Payday = payDay,
                Enable = true
            };
            await _context.Payslips.AddAsync(payslip);
            await _context.SaveChangesAsync();
            return (payslip, resultPersonalIncomeTax);
        }
        private static int COMPANY_MAX_SOCIAL_INSURANCE_FEE = 5215000;
        private static int COMPANY_MAX_HEALTH_INSURANCE_FEE = 894000;
        private static int COMPANY_MAX_UNEMPLOYEMENT_INSURANCE_FEE = 884000;

        private static double CompanySocialInsurance = 0.175;
        private static double CompanyHealthInsurance = 0.03;
        private static double CompanyUnemploymentInsurance = 0.01;
        [HttpGet]
        public CompanyInsurance CompanyInsuranceCalculate(int grossSalary, int taxableSalary)
        {
            int SocialInsuranceDeduction = (int)(taxableSalary * CompanySocialInsurance);
            int HealthInsuranceDeduction = (int)(taxableSalary * CompanyHealthInsurance);
            int UnemploymentInsuranceDeduction = (int)(taxableSalary * CompanyUnemploymentInsurance);

            if (SocialInsuranceDeduction > COMPANY_MAX_SOCIAL_INSURANCE_FEE)
            {
                SocialInsuranceDeduction = COMPANY_MAX_SOCIAL_INSURANCE_FEE;
            }

            if (HealthInsuranceDeduction > COMPANY_MAX_HEALTH_INSURANCE_FEE)
            {
                HealthInsuranceDeduction = COMPANY_MAX_HEALTH_INSURANCE_FEE;
            }

            if (UnemploymentInsuranceDeduction > COMPANY_MAX_UNEMPLOYEMENT_INSURANCE_FEE)
            {
                UnemploymentInsuranceDeduction = COMPANY_MAX_UNEMPLOYEMENT_INSURANCE_FEE;
            }

            int totalInsurance = (SocialInsuranceDeduction + HealthInsuranceDeduction + UnemploymentInsuranceDeduction);
            int netSalary = grossSalary + totalInsurance;

            CompanyInsurance companyInsuranceDto = new CompanyInsurance
            {
                GrossSalary = grossSalary,
                SocialInsurance = SocialInsuranceDeduction,
                HealthInsurance = HealthInsuranceDeduction,
                UnemploymentInsurance = UnemploymentInsuranceDeduction,
                TotalInsurance = totalInsurance,
                NetSalary = netSalary
            };

            return companyInsuranceDto;
        }

        (int, double)[] TaxableAmountAndTaxRate = {
                (5000000, 0.05),
                (5000000, 0.1),
                (8000000, 0.15),
                (14000000, 0.20),
                (20000000, 0.25),
                (28000000, 0.3),
                (0, 0.35),
            };
        [HttpGet]
        public async Task<int> OtSalary(int staffId, int month, int year)
        {
            var logOts = await _context.LogOts
                .Where(c =>
                    c.StaffId == staffId &&
                    c.LogStart.Month == month &&
                    c.LogStart.Year == year &&
                    c.Status.ToLower().Equals("approved"))
                .ToListAsync();

            var OtSalary = logOts.Sum(c => c.Amount);

            if (OtSalary != null)
            {
                return (int)OtSalary;
            }

            return 0;
        }
        [HttpGet]
        public List<TaxDetail> PersonalIncomeTaxCalculate(int ThuNhapChiuThue)
        {
            List<TaxDetail> result = new List<TaxDetail>();
            int TaxRate = 0;
            int i = 1;
            foreach (var number in TaxableAmountAndTaxRate)
            {
                if (number.Item1 == 0)
                {
                    TaxRate = (int)(ThuNhapChiuThue * number.Item2);
                }

                else if (ThuNhapChiuThue >= number.Item1)
                {
                    TaxRate = (int)(number.Item1 * number.Item2);
                }

                else
                {
                    TaxRate = (int)(ThuNhapChiuThue * number.Item2);
                }


                if (ThuNhapChiuThue <= 0) TaxRate = 0;

                result.Add(new TaxDetail
                {
                    TaxLevel = i,
                    Amount = TaxRate
                });


                ThuNhapChiuThue -= number.Item1;
                i++;
            }
            Console.WriteLine(result);


            return result;
        }
        [HttpGet]
        public int countWorkDays(int year, int month)
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
        [HttpGet]
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
        [HttpGet]
        public async Task<int> GetNoDependencies(int staffId)
        {
            var noOfDependencies = await _context.PersonnelContracts
                .Where(c => c.StaffId == staffId)
                .Select(c => c.NoOfDependences)
                .FirstOrDefaultAsync();
            return (int)noOfDependencies;
        }
        [HttpGet]
        public async Task<int> TaxableIncomeCalculation(int salaryBeforeTax, int staffId)
        {

            int noOfDependences = await GetNoDependencies(staffId);

            int FamilyTaxDeduction = FamilyAllowances * noOfDependences;

            int TotalTaxDeduction = (PersonalTaxDeduction + FamilyTaxDeduction);

            int taxableIncome = salaryBeforeTax - TotalTaxDeduction;

            if (taxableIncome < 0) taxableIncome = 0;

            return taxableIncome;
        }
        [HttpGet]
        public async Task<int> GetFamilyAllowance(int staffId)
        {
            int familyAllowance = 4400000;

            var noOfDependencies = await _context.PersonnelContracts
               .Where(c =>
                c.StaffId == staffId &&
                c.ContractStatus == true)
               .Select(c => c.NoOfDependences)
               .FirstOrDefaultAsync();

            return (int)noOfDependencies * familyAllowance;
        }

        [HttpGet]
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
        [HttpGet]
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
        [HttpGet]
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
        [HttpGet]
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
        [HttpGet]
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
        [HttpGet]
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
        [HttpGet]
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

        [HttpGet]
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
        [HttpGet]
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
        [HttpGet]
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
        [HttpGet("filters")]
        public async Task<IActionResult> GetFilter()
        {
            var departments = await _context.Payslips
                .Include(c => c.Staff)
                .ThenInclude(c => c.Department)
                .Select(c => c.Staff.Department.DepartmentName)
                .Distinct()
                .ToListAsync();
            return Ok(departments);
        }
    }
}
