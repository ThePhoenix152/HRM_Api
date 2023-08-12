using System.Runtime.CompilerServices;
using HumanResourceapi.DTOs.PayslipDTOs;
using HumanResourceapi.DTOs.PersonnelContractDTO;
using HumanResourceapi.Models;
using HumanResourceapi.Extensions;
using HumanResourceapi.RequestHelpers;
using AutoMapper;
using Azure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;

namespace HumanResourceapi.Services
{
    public class PayslipService
    {
        private static int PersonalTaxDeduction = 11000000;
        private static int FamilyAllowances = 4400000;

        private readonly SwpProjectContext _context;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        private readonly PersonnelContractService _personnelContractService;
        private readonly LogOtService _logOtService;
        private readonly LogLeaveService _logLeaveService;
        private readonly AllowanceService _allowanceService;
        private readonly TheCalendarService _theCalendarService;

        public PayslipService(
            SwpProjectContext context,
            IMapper mapper,
            ILogger<PayslipService> logger,
            PersonnelContractService personnelContractService,
            LogOtService logOtService,
            LogLeaveService logLeaveService,
            AllowanceService allowanceService,
            TheCalendarService theCalendarService
            )
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _personnelContractService = personnelContractService ?? throw new ArgumentNullException(nameof(personnelContractService));
            _logOtService = logOtService ?? throw new ArgumentNullException(nameof(logOtService));
            _logLeaveService = logLeaveService ?? throw new ArgumentNullException(nameof(logLeaveService));
            _allowanceService = allowanceService ?? throw new ArgumentNullException(nameof(allowanceService));
            _theCalendarService = theCalendarService ?? throw new ArgumentNullException(nameof(theCalendarService));
            _context = context ?? throw new ArgumentNullException(nameof(context));

        }

        public async Task<PayslipDTO> AddPayslipToDatabase(
            int staffId, 
            PayslipInputCreationDto payslipInputCreationDto)
        {
            var userInfo = await _context.UserInfors
                .Include(c => c.Payslips)
                .Where(c => c.StaffId == staffId && c.AccountStatus == true)
                .FirstOrDefaultAsync();

            if (userInfo == null)
            {
                return null;
            }

            var (payslipDTO, result) = await GetPayslipCreationDtoOfStaff(
                staffId, 
                payslipInputCreationDto);


            var payslipInfor = await _context.Payslips
                .Include(c => c.TaxDetails)
                .ThenInclude(c => c.TaxLevelNavigation)
                .Where(c => c.StaffId == staffId && c.PayslipId == payslipDTO.PayslipId)
                .FirstOrDefaultAsync();

            foreach (var taxDetail in result)
            {
                var taxDetailEntity = _mapper.Map<TaxDetail>(taxDetail);


                payslipInfor.TaxDetails.Add(taxDetailEntity);
            }

            await _context.SaveChangesAsync();


            return payslipDTO;
        }
        public async Task<(PayslipDTO, List<TaxDetailCreationDTO> taxDetailDTOs)> GetPayslipCreationDtoOfStaff(
                int staffId,
                PayslipInputCreationDto payslipInputCreationDto
            )
        {
            int standardPayDay = 25;

            //Gross To Net
            var personnelContract = await _personnelContractService
                                        .GetValidPersonnelContractEntityByStaffId(staffId);
            int paidByDate = await _personnelContractService.BasicSalaryOneDayOfMonth(
                staffId, 
                payslipInputCreationDto.Month, 
                payslipInputCreationDto.Year);

            //grossStandardSalary
            var standardGrossSalary = personnelContract.Salary;
            var allowancesSalary = await _allowanceService.GetAllowancesOfStaff(staffId);
            var leavesDeductedSalary = await _logLeaveService.GetDeductedSalary(
                staffId, 
                paidByDate,
                payslipInputCreationDto.Month,
                payslipInputCreationDto.Year);
            //grossActualSalary
            var actualGrossSalary = (standardGrossSalary + allowancesSalary) - leavesDeductedSalary;

            //taxable Salary
            var standardTaxableSalary = personnelContract.TaxableSalary;
            var actualTaxableSalary = standardTaxableSalary + allowancesSalary - leavesDeductedSalary;



            //days Calculation: stardard works days, overtime days, leaves days (Include total type leaves days)
            int standardWorkDays = await GetStandardWorkDays(
                payslipInputCreationDto.Month,
                payslipInputCreationDto.Year);
            int actualWorkDays = await GetActualWorkDaysOfStaff(
                staffId,
                payslipInputCreationDto.Month,
                payslipInputCreationDto.Year);

            int leaveDays = await _logLeaveService.GetLeaveDays(
                staffId,
                payslipInputCreationDto.Month,
                payslipInputCreationDto.Year);


            int leaveHours = await _logLeaveService.GetLeavesHours(
                staffId,
                payslipInputCreationDto.Month,
                payslipInputCreationDto.Year);

            //Tính bảo hiểm từng phần
            var personalInsurance = PersonalInsuranceCalculate((int)actualTaxableSalary);

            //Thu nhập trước thuế (Tổng bảo hiểm cá nhân)
            //Gross Actual Salary - total 
            int salaryBeforeTax = CalculatePretaxEarning(personalInsurance, actualGrossSalary);


            // Giảm trừ gia cảnh
            int selfDeduction = PersonalTaxDeduction;
            int familyDeduction = await _personnelContractService.GetFamilyAllowance(staffId);
            int taxableIncome = await TaxableIncomeCalculation(salaryBeforeTax, staffId);



            //Thue Thu Nhap Ca Nhan
            var result = PersonalIncomeTaxCalculate(taxableIncome);


            foreach (var item in result)
            {
                _logger.LogCritical(item.Amount.ToString());
            }

            int personalIncomeTax = (int)result.Sum(c => c.Amount);
            //Net Salary
            int standardNetSalary = (salaryBeforeTax - personalIncomeTax);


            // Lương thực nhận
            int otSalary = await _logOtService.OtSalary(
                staffId,
                payslipInputCreationDto.Month,
                payslipInputCreationDto.Year);
            int actualNetSalary = standardNetSalary + otSalary;


            //Nguoi su dung lao dong tra
            CompanyInsuranceDTO companyInsuranceDto = CompanyInsuranceCalculate(actualGrossSalary, (int)actualTaxableSalary);
            int actualCompPaid = (int)(companyInsuranceDto.NetSalary + otSalary);


            DateTime payDay = new DateTime(payslipInputCreationDto.Year, payslipInputCreationDto.Month, standardPayDay);

            PayslipCreationDTO payslipDto = new PayslipCreationDTO
            {
                GrossStandardSalary = standardGrossSalary,
                GrossActualSalary = actualGrossSalary,
                StandardWorkDays = standardWorkDays,
                ActualWorkDays = actualWorkDays,
                LeaveHours = leaveHours,
                LeaveDays = leaveDays,
                OtTotal = otSalary,
                Bhxhemp = (int?)personalInsurance.SocialInsurance,
                Bhytemp = (int?)personalInsurance.HealthInsurance,
                Bhtnemp = (int?)personalInsurance.UnemploymentInsurance,
                SalaryBeforeTax = salaryBeforeTax,
                SelfDeduction = selfDeduction,
                FamilyDeduction = familyDeduction,
                TaxableSalary = actualTaxableSalary,
                PersonalIncomeTax = personalIncomeTax,
                TotalAllowance = allowancesSalary,
                SalaryRecieved = actualNetSalary,
                NetStandardSalary = standardNetSalary,
                NetActualSalary = actualNetSalary,
                Bhxhcomp = (int?)companyInsuranceDto.SocialInsurance,
                Bhytcomp = (int?)companyInsuranceDto.HealthInsurance,
                Bhtncomp = (int?)companyInsuranceDto.UnemploymentInsurance,
                TotalCompInsured = (int?)companyInsuranceDto.TotalInsurance,
                TotalCompPaid = actualCompPaid,
                CreateAt = DateTime.UtcNow.AddHours(7),
                ChangeAt = DateTime.UtcNow.AddHours(7),
                CreatorId = payslipInputCreationDto.CreatorId,
                ChangerId = payslipInputCreationDto.ChangerId,
                Status = "pending",
                Payday = payDay,
                Enable = true
            };

            var userInfo = await _context.UserInfors
                .Include(c => c.Payslips)
                .Where(c => c.StaffId == staffId && c.AccountStatus == true)
                .FirstOrDefaultAsync();

            var payslipEntity = _mapper.Map<Payslip>(payslipDto);
            userInfo.Payslips.Add(payslipEntity);

            await _context.SaveChangesAsync();
            var returnPayslip = _mapper.Map<PayslipDTO>(payslipEntity);

            return (returnPayslip, result);
        }

        public async Task<int> TaxableIncomeCalculation(int salaryBeforeTax, int staffId)
        {

            int noOfDependences = await _personnelContractService
                                            .GetNoDependencies(staffId);

            int FamilyTaxDeduction = FamilyAllowances * noOfDependences;

            int TotalTaxDeduction = (PersonalTaxDeduction + FamilyTaxDeduction);

            int taxableIncome = salaryBeforeTax - TotalTaxDeduction;

            if (taxableIncome < 0) taxableIncome = 0;

            return taxableIncome;
        }

        public static int CalculatePretaxEarning(InsuranceDTO Insurance, int actualGrossSalary)
        {
            var totalInsurance =
                Insurance.SocialInsurance +
                Insurance.HealthInsurance +
                Insurance.UnemploymentInsurance;
            return (int)(actualGrossSalary - totalInsurance);
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

        public List<TaxDetailCreationDTO> PersonalIncomeTaxCalculate(int ThuNhapChiuThue)
        {
            List<TaxDetailCreationDTO> result = new List<TaxDetailCreationDTO>();
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

                result.Add(new TaxDetailCreationDTO
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
        private static int PERSONAL_MAX_SOCIAL_INSURANCE_FEE = 2384000;
        private static int PERSONAL_MAX_HEALTH_INSURANCE_FEE = 447000;
        private static int PERSONAL_MAX_UNEMPLOYEMENT_INSURANCE_FEE = 884000;

        private static double PersonalSocialInsurance = 0.08;
        private static double PersonalHealthInsurance = 0.015;
        private static double PersonalUnemploymentInsurance = 0.01;
        public InsuranceDTO PersonalInsuranceCalculate(int InsuranceSalary)
        {
            int SocialInsuranceDeduction = (int)(InsuranceSalary * PersonalSocialInsurance);
            int HealthInsuranceDeduction = (int)(InsuranceSalary * PersonalHealthInsurance);
            int UnemploymentInsuranceDeduction = (int)(InsuranceSalary * PersonalUnemploymentInsurance);

            if (SocialInsuranceDeduction > PERSONAL_MAX_SOCIAL_INSURANCE_FEE)
            {
                SocialInsuranceDeduction = PERSONAL_MAX_SOCIAL_INSURANCE_FEE;
            }

            if (HealthInsuranceDeduction > PERSONAL_MAX_HEALTH_INSURANCE_FEE)
            {
                HealthInsuranceDeduction = PERSONAL_MAX_HEALTH_INSURANCE_FEE;
            }

            if (UnemploymentInsuranceDeduction > PERSONAL_MAX_UNEMPLOYEMENT_INSURANCE_FEE)
            {
                UnemploymentInsuranceDeduction = PERSONAL_MAX_UNEMPLOYEMENT_INSURANCE_FEE;
            }

            var InsuranceDeduction = new InsuranceDTO
            {
                SocialInsurance = SocialInsuranceDeduction,
                UnemploymentInsurance = UnemploymentInsuranceDeduction,
                HealthInsurance = HealthInsuranceDeduction,
            };
            return InsuranceDeduction;
        }


        private static int COMPANY_MAX_SOCIAL_INSURANCE_FEE = 5215000;
        private static int COMPANY_MAX_HEALTH_INSURANCE_FEE = 894000;
        private static int COMPANY_MAX_UNEMPLOYEMENT_INSURANCE_FEE = 884000;

        private static double CompanySocialInsurance = 0.175;
        private static double CompanyHealthInsurance = 0.03;
        private static double CompanyUnemploymentInsurance = 0.01;
        public CompanyInsuranceDTO CompanyInsuranceCalculate(int grossSalary, int taxableSalary)
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

            CompanyInsuranceDTO companyInsuranceDto = new CompanyInsuranceDTO
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

        public async Task<int> GetNumberWeekDaysInMonth(int month, int year)
        {
            var weekDays = await _context.DateDimensions
                .Where(c =>
                    c.IsWeekend == 0 &&
                    c.TheMonth == month &&
                    c.TheYear == year)
                .CountAsync();

            return weekDays;
        }

        public async Task<int> GetNumberHolidaysInMonth(int month, int year)
        {
            var holidays = await _context.HolidayDimensions
                .Where(c =>
                c.TheDate.Month == month &&
                c.TheDate.Year == year)
                .CountAsync();

            return holidays;
        }


        public async Task<int> GetPaidByDate(int month, int year, int salary)
        {
            var StandardWorkDays = await _context.TheCalendars
               .Where(c =>
                    c.IsWeekend == 0 &&
                    c.TheMonth == month &&
                    c.TheYear == year)
               .CountAsync();

            int paidByDate = (salary / StandardWorkDays);

            return paidByDate;
        }


        public async Task<double> GetLogOtHours(int StaffId)
        {
            var logOtHours = await _context.LogOts
                .Where(c =>
                    c.StaffId == StaffId &&
                    c.Status.ToLower().Equals("approved"))
                .ToListAsync();

            double total = 0;
            total = logOtHours.Sum(c => c.LogHours);

            return total;
        }


        public async Task<int> GetLogOtDays(int StaffId)
        {
            var logOtStaff = await _context.LogOts
                .Where(c =>
                    c.StaffId == StaffId &&
                    c.Status.ToLower().Equals("approved"))
                .ToListAsync();

            int logOtDays = logOtStaff.Count();
            return logOtDays;
        }

        public async Task<int> GetLogLeaveHours(int StaffId)
        {
            var LeaveDays = await _context.LogLeaves.ToListAsync();

            var LogLeavesHours = LeaveDays.Sum(c => c.LeaveDays) * 8;

            return (int)LogLeavesHours;
        }

        public async Task<int> GetLogLeaveDays(int StaffId)
        {
            var LeaveDays = await _context.LogLeaves.ToListAsync();


            return (int)LeaveDays.Sum(c => c.LeaveDays);
        }

        public async Task<int> GetTotalAllowancesByStaffId(int StaffId)
        {
            var staffAllowances = await _context.PersonnelContracts
                .Include(c => c.Allowances).Where(c => c.StaffId == StaffId)
                .ToListAsync();

            var allowances = staffAllowances
                .SelectMany(c => c.Allowances)
                .Sum(c => c.AllowanceSalary);

            return (int)allowances;
        }

        public async Task<bool> IsGrossToNet(int staffId, int contractId)
        {
            return await _context.PersonnelContracts
                .AnyAsync(c => c.StaffId == staffId && c.ContractId == contractId && c.ContractStatus == true);
        }

        public async Task<PagedList<PayslipDTO>> GetPayslipAsync(
            PayslipParams payslipParams
            )
        {
            var payslips = _context.Payslips
                .Include(c => c.Staff)
                .ThenInclude(c => c.Department)
                .Include(c => c.TaxDetails)
                .ThenInclude(c => c.TaxLevelNavigation)
                .Sort(payslipParams.OrderBy)
                .Search(payslipParams.SearchTerm)
                .Filter(payslipParams.Departments)
                .AsQueryable();

            var returnPayslips = await PagedList<Payslip>.ToPagedList(
                payslips,
                payslipParams.PageNumber, payslipParams.PageSize);

            var mappedPayslips = returnPayslips.Select(p => _mapper.Map<PayslipDTO>(p)).ToList();

            var finalPayslips = new PagedList<PayslipDTO>(
                mappedPayslips, 
                returnPayslips.MetaData.TotalCount, 
                payslipParams.PageNumber, 
                payslipParams.PageSize);

            return finalPayslips;
        }

        public async Task<List<PayslipDTO>> GetPayslipOfStaff(int staffId)
        {
            var payslips = await _context.Payslips
                .Include(c => c.Staff)
                .ThenInclude(c => c.Department)
                .Include(c => c.TaxDetails)
                    .ThenInclude(c => c.TaxLevelNavigation)
                .Where(c => c.StaffId == staffId)
                .OrderByDescending(c => c.PayslipId)
                .ToListAsync();

            var finalPayslips = _mapper.Map<List<PayslipDTO>>(payslips);
            return finalPayslips;
        }

        public async Task<bool> IsPayslipExist(int staffId, int payslipId)
        {
            return await _context.Payslips
                .AnyAsync(c => c.StaffId == staffId && c.PayslipId == payslipId);
        }

        public async Task<bool> IsPayslipExist( int payslipId)
        {
            return await _context.Payslips
                .AnyAsync(c =>  c.PayslipId == payslipId);
        }

        public async Task<PayslipDTO> GetPayslipOfStaffByPayslipId(int staffId, int payslipId)
        {
            var payslip = await _context.Payslips
                .Include(c => c.Staff)
                .Include(c => c.TaxDetails)
                .ThenInclude(c => c.TaxLevelNavigation)
                .Where(c => c.StaffId == staffId && c.PayslipId == payslipId)
                .FirstOrDefaultAsync();

            var returnPayslip = _mapper.Map<PayslipDTO>(payslip);

            return returnPayslip;
        }

        //Số ngày đi làm cơ bản của 1 người trong 1 tháng
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

        //Số ngày đi làm trong tuần
        public async Task<int> GetStandardWorkDaysWithoutHoliday(int month, int year)
        {
            var basicActualWorkDays = await _context.TheCalendars
                .Where(c =>
                    c.TheMonth == month &&
                    c.TheYear == year &&
                    c.IsWeekend == 0)
                .ToListAsync();

            return basicActualWorkDays.Count;
        }

        public async Task<int> GetActualWorkDaysOfStaff(
            int staffId, int month, int year)
        {
            var basicActualWorkDays = await GetStandardWorkDays(month, year);

            var otDays = await _logOtService.GetOtDays(staffId, month, year);

            var leaveDays = await _logLeaveService.GetLeaveDays(staffId, month, year);




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




    }
}

