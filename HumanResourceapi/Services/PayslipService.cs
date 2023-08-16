using System.Runtime.CompilerServices;
using HumanResourceapi.DTOs.PayslipDTOs;
using HumanResourceapi.DTOs.PersonnelContractDTO;
using HumanResoureapi.Models;
using HumanResourceapi.Extensions;
using HumanResourceapi.RequestHelpers;
using AutoMapper;
using Azure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Logging;
using API.Services;

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
        private readonly AllowanceService _allowanceService;
        private readonly LogLeaveService _logLeaveService;


        public PayslipService(
            SwpProjectContext context,
            IMapper mapper,
            ILogger<PayslipService> logger,
            PersonnelContractService personnelContractService,
            LogOtService logOtService,
            AllowanceService allowanceService
            )
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _personnelContractService = personnelContractService ?? throw new ArgumentNullException(nameof(personnelContractService));
            _logOtService = logOtService ?? throw new ArgumentNullException(nameof(logOtService));
            _allowanceService = allowanceService ?? throw new ArgumentNullException(nameof(allowanceService));
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

            var payslipDTO = await GetPayslipCreationDtoOfStaff(
                staffId, 
                payslipInputCreationDto);


            var payslipInfor = await _context.Payslips.Where(c => c.StaffId == staffId && c.PayslipId == payslipDTO.PayslipId)
                .FirstOrDefaultAsync();

            await _context.SaveChangesAsync();


            return payslipDTO;
        }
        public async Task<PayslipDTO> GetPayslipCreationDtoOfStaff(
                int staffId,
                PayslipInputCreationDto payslipInputCreationDto
            )
        {
            int standardPayDay = 22;

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
            // Giảm trừ gia cảnh
            int selfDeduction = PersonalTaxDeduction;
            int familyDeduction = await _personnelContractService.GetFamilyAllowance(staffId);


            // Lương thực nhận
            int otSalary = await _logOtService.OtSalary(
                staffId,
                payslipInputCreationDto.Month,
                payslipInputCreationDto.Year);
            int actualNetSalary =  otSalary;


            //Nguoi su dung lao dong tra
            int actualCompPaid = (int)( otSalary);


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
                SelfDeduction = selfDeduction,
                FamilyDeduction = familyDeduction,
                TotalAllowance = allowancesSalary,
                SalaryRecieved = actualNetSalary,
                NetActualSalary = actualNetSalary,
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

            return (returnPayslip);
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
            var logOtHours = await _context.Otapplications
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
            var logOtStaff = await _context.Otapplications
                .Where(c =>
                    c.StaffId == StaffId &&
                    c.Status.ToLower().Equals("approved"))
                .ToListAsync();

            int logOtDays = logOtStaff.Count();
            return logOtDays;
        }

        public async Task<int> GetLogLeaveHours(int StaffId)
        {
            var LeaveDays = await _context.LeaveApplications.Where(c => c.StaffId == StaffId).ToListAsync();

            var LogLeavesHours = LeaveDays.Sum(c => c.LeaveDays) * 8;

            return (int)LogLeavesHours;
        }

        public async Task<int> GetLogLeaveDays(int StaffId)
        {
            var LeaveDays = await _context.LeaveApplications.Where(c => c.StaffId == StaffId).ToListAsync();


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

