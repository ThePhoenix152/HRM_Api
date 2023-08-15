using System;
using System.Xml.Schema;
using API.Extensions;

using AutoMapper;
using HumanResourceapi.DTOs.LogLeaveDTO;
using HumanResourceapi.RequestHelpers;
using HumanResourceapi.Services;
using HumanResoureapi.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services
{
  public class LogLeaveService
  {
    private readonly SwpProjectContext _context;
    private readonly IMapper _mapper;
    private readonly TheCalendarService _theCalendarService;
    private readonly UserInfoService _userInfoService;
    private readonly ILogger _logger;
    private readonly PersonnelContractService _personnelContractService;

    public LogLeaveService(
        SwpProjectContext context,
        IMapper mapper,
        TheCalendarService theCalendarService,
        UserInfoService userInfoService,
        ILogger<LogLeaveService> logger,
        PersonnelContractService personnelContractService
        )
    {
      _context = context ?? throw new ArgumentNullException(nameof(context));
      _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
      _theCalendarService = theCalendarService ?? throw new ArgumentNullException(nameof(theCalendarService));
      _userInfoService = userInfoService ?? throw new ArgumentNullException(nameof(userInfoService));
      _logger = logger ?? throw new ArgumentNullException(nameof(logger));
      _personnelContractService = personnelContractService ?? throw new ArgumentNullException(nameof(personnelContractService));
    }
        public async Task<PagedList<LogLeaveDTO>> GetLogLeaveDTOs(LogLeaveParams logLeaveParams)
        {
            var logleaves =  _context.LeaveApplications
                                    .Include(c => c.LeaveType)
                                    .Include(c => c.Staff)
                                    .ThenInclude(c => c.Department)
                                    .OrderByDescending(c => c.LeaveLogId)
                                    .Search(logLeaveParams.SearchTerm)
                                    .Filter(logLeaveParams.Departments)
                                    .AsQueryable();


            var returnLogLeaves = await PagedList<LeaveApplication>.ToPagedList(
                logleaves,
                logLeaveParams.PageNumber,
                logLeaveParams.PageSize);


            var mappedLogLeaves = returnLogLeaves.Select(p => _mapper.Map<LogLeaveDTO>(p)).ToList();

            mappedLogLeaves = mappedLogLeaves.Select(returnLogLeave =>
            {
                returnLogLeave.ResponsdenceName = _context.UserInfors
                  .Where(c => c.StaffId == returnLogLeave.RespondencesId)
                  .Select(s => s.LastName + " " + s.FirstName)
                  .FirstOrDefault();
                return returnLogLeave;
            }).ToList();

            var finalLogLeaves = new PagedList<LogLeaveDTO>(
                mappedLogLeaves,
                returnLogLeaves.MetaData.TotalCount,
                logLeaveParams.PageNumber,
                logLeaveParams.PageSize
                );
            //var returnLogLeaves = _mapper.Map<List<LogLeaveDTO>>(logleaves);

           // returnLogLeaves = returnLogLeaves.Select(returnLogLeave =>
           //{
           //    returnLogLeave.ResponsdenceName = _context.UserInfors
           //      .Where(c => c.StaffId == returnLogLeave.RespondencesId)
           //      .Select(s => s.LastName + " " + s.FirstName)
           //      .FirstOrDefault();
           //    return returnLogLeave;
           //}).ToList();
            // return returnLogLeaves;
            return finalLogLeaves;
        }
        public async Task<List<LogLeaveDTO>> GetLogLeavesByStaffId(int staffId)
    {
      var logLeaves = await _context.LeaveApplications
                              .Include(c => c.LeaveType)
                              .Include(c => c.Staff)
                              .Where(c => c.StaffId == staffId)
                              .ToListAsync();

      var returnLogLeaves = _mapper.Map<List<LogLeaveDTO>>(logLeaves);
      returnLogLeaves = returnLogLeaves.Select(returnLogLeave =>
      {
        returnLogLeave.ResponsdenceName = _context.UserInfors
    .Where(c => c.StaffId == returnLogLeave.RespondencesId)
    .Select(s => s.LastName + " " + s.FirstName)
    .FirstOrDefault();
        return returnLogLeave;
      }).ToList();
      return returnLogLeaves;
    }

    public async Task<bool> IsLogLeaveExist(int logLeaveId)
    {
      return await _context.LeaveApplications.AnyAsync(c => c.LeaveLogId == logLeaveId);
    }

    public async Task<LeaveApplication> GetLogLeaveAsync(int staffId, int logLeaveId)
    {
      return await _context.LeaveApplications
                                 .Include(c => c.LeaveType)
                                 .Include(c => c.Staff)
                                 .Where(c => c.StaffId == staffId && c.LeaveLogId == logLeaveId)
                                 .FirstOrDefaultAsync();
    }

    public async Task<LogLeaveDTO> GetLogLeaveByStaffIdAndLogLeaveId(int staffId, int logLeaveId)
    {
      var logLeave = await _context.LeaveApplications
                                  .Include(c => c.LeaveType)
                                  .Include(c => c.Staff)
                                  .Where(c => c.StaffId == staffId && c.LeaveLogId == logLeaveId)
                                  .FirstOrDefaultAsync();
      var returnLogLeave = _mapper.Map<LogLeaveDTO>(logLeave);
      returnLogLeave.ResponsdenceName = _context.UserInfors
   .Where(c => c.StaffId == returnLogLeave.RespondencesId)
   .Select(s => s.LastName + " " + s.FirstName)
   .FirstOrDefault();
      return returnLogLeave;
    }
    public async Task UpdateLogLeave(int staffId, int logLeaveId, LogLeaveUpdateDTO logLeaveUpdateDTO)
    {
      var logLeave = await _context.LeaveApplications
                                  .Include(c => c.LeaveType)
                                  .Include(c => c.Staff)
                                  .Where(c => c.StaffId == staffId && c.LeaveLogId == logLeaveId)
                                  .FirstOrDefaultAsync();

      if (logLeave == null)
      {
        return;
      }

      _mapper.Map(logLeaveUpdateDTO, logLeave);

      await _context.SaveChangesAsync();

    }

    public async Task<UserInfor> GetUserLogLeave(int staffId)
    {
      var user = await _context.UserInfors
          .Include(c => c.LeaveApplications)
          .Where(c => c.StaffId == staffId)
          .FirstOrDefaultAsync();
      return user;
    }
    public async Task<int> LeaveDaysCalculation(DateTime start, DateTime end)
    {

      var leaveDays = await _context.TheCalendars.Where(c =>
              c.TheDate >= start &&
              c.TheDate <= end &&
              c.IsWorking == 1).ToListAsync();


      return leaveDays.Count;
    }

    public async Task<int> GetLeaveDays(int staffId, int month, int year)
    {
      var logLeaves = await _context.LeaveApplications
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

          var days = await _theCalendarService.GetWorkingDays(startDate, endDate);

          sum += days.Count;
        }
      }


      return (int)sum;
    }

    public async Task<int> GetLeavesHours(int staffId, int month, int year)
    {
      var logLeaves = await _context.LeaveApplications
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

    public DateTime GetStartDay(int month, DateTime start)
    {
      if (month < 1 || month > 12)
      {
        _logger.LogCritical("error at month");
        throw new ArgumentException("Invalid Month");
      }
      if (month < start.Month)
      {
        _logger.LogCritical("error at month");
        throw new ArgumentException("Invalid Month");

      }

      if (month == start.Month)
      {
        _logger.LogCritical(start.Day.ToString());
        return start;
      }
      else
      {
        _logger.LogCritical("1");
        DateTime firstDateOfMonth = new DateTime(start.Year, start.Month, 1);
        return firstDateOfMonth;
      }
    }

    public DateTime GetEndDay(int month, DateTime end)
    {
      if (month < 1 || month > 12)
      {
        _logger.LogCritical("error at month");
        throw new ArgumentException("Invalid Month");
      }

      if (month > end.Month)
      {
        _logger.LogCritical("error at month");
        throw new ArgumentException("Invalid Month");
      }

      if (month == end.Month)
      {
        _logger.LogCritical(end.Day.ToString());
        return end;
      }
      else
      {
        int daysInMonth = DateTime.DaysInMonth(end.Year, month);

        DateTime lastDayOfMonth = new DateTime(end.Year, month, daysInMonth);

        return lastDayOfMonth;
      }
    }


    public async Task<LogLeaveDTO> CreateLogLeave(int staffId, LogLeaveCreationDTO logLeaveCreationDTO)
    {

      var user = await GetUserLogLeave(staffId);

      var salaryPerDay = await _personnelContractService.BasicSalaryOneDayOfMonth(
          staffId,
          logLeaveCreationDTO.LeaveStart.Month,
          logLeaveCreationDTO.LeaveStart.Year);

      var days = await LeaveDaysCalculation(logLeaveCreationDTO.LeaveStart, logLeaveCreationDTO.LeaveEnd);
      logLeaveCreationDTO.LeaveDays = days;
      logLeaveCreationDTO.LeaveHours = days * 8;
      if (logLeaveCreationDTO.LeaveTypeId == 3)
      {

        logLeaveCreationDTO.Amount = salaryPerDay * days;
      }
      else
      {
        logLeaveCreationDTO.Amount = 0;

      }
      logLeaveCreationDTO.SalaryPerDay = salaryPerDay;
      logLeaveCreationDTO.Status = "pending";

      var logLeave = _mapper.Map<LeaveApplication>(logLeaveCreationDTO);

      user.LeaveApplications.Add(logLeave);

      await _context.SaveChangesAsync();


      var returnLogLeave = _mapper.Map<LogLeaveDTO>(logLeave);

      return returnLogLeave;
    }
    public async Task<int> GetDeductedSalary(int staffId, int paidByDate, int month, int year)
    {
      var logLeaves = await _context.LeaveApplications
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


        var workingDays = await _theCalendarService
             .GetWorkingDays(startDay, endDay);

        leaveDays = workingDays.Count;
      }

      int totalDeductedSalary = leaveDays * paidByDate;

      return totalDeductedSalary;
    }




  }
}
