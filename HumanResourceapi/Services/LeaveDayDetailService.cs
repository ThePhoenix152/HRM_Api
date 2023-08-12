using HumanResourceapi.DTOs.LeaveDayDetailDTO;
using HumanResourceapi.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HumanResourceapi.Services
{
    public class LeaveDayDetailService
    {
        private readonly SwpProjectContext _context;
        private readonly IMapper _mapper;
        private readonly TheCalendarService _theCalendarService;
        private readonly UserInfoService _userInfoService;

        public LeaveDayDetailService(
            SwpProjectContext context,
            IMapper mapper,
            TheCalendarService theCalendarService,
            UserInfoService userInfoService

            )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _theCalendarService = theCalendarService ?? throw new ArgumentNullException(nameof(theCalendarService));
            _userInfoService = userInfoService ?? throw new ArgumentNullException(nameof(userInfoService));
        }
        public async Task<List<LeaveDayDetailDTO>> GetLeaveDayDetailDTOs(int staffId)
        {
            var leaveDayDetails = await _context.LeaveDayDetails
                .Include(c => c.LeaveType)
                .Include(c => c.Staff)
                .Where(c => c.StaffId == staffId).ToListAsync();

            var returnLeaveDayDetails = _mapper.Map<List<LeaveDayDetailDTO>>(leaveDayDetails);

            return returnLeaveDayDetails;
        }

        public async Task<bool> IsLeaveDayDetailOfStaffEixst(int staffId)
        {
            return await _context.LeaveDayDetails.AnyAsync(c => c.StaffId == staffId);
        }

        public async Task<UserInfor> GetUserInforForLeaveDayDetail(int staffId)
        {
            return await _context.UserInfors
                    .Include(c => c.LeaveDayDetails)
                    .Where(c => c.StaffId == staffId)
                    .FirstOrDefaultAsync();
        }

        public async Task<bool> GetGender(int staffId)
        {
            var user = await _context.UserInfors.Where(c => c.StaffId == staffId).FirstOrDefaultAsync();

            return (bool)user.Gender;
        }

        public async Task CreateLeaveDayDetail(int staffId)
        {
            if (!await _userInfoService.IsUserExist(staffId))
            {
                return;
            }
            var user = await GetUserInforForLeaveDayDetail(staffId);

            bool gender = await GetGender(staffId);

            var leaveDayDetails = await GetLeaveDayDetailCreationDTOs(gender);

            foreach (var leaveDayDetail in leaveDayDetails)
            {
                var leaveDayEntity = _mapper.Map<LeaveDayDetail>(leaveDayDetail);

                user.LeaveDayDetails.Add(leaveDayEntity);

            }

            await _context.SaveChangesAsync();


        }

        public async Task<List<LeaveDayDetailCreationDTO>> GetLeaveDayDetailCreationDTOs(bool gender)
        {
            List<LeaveDayDetailCreationDTO> leaveDayDetailCreationDTOs = new List<LeaveDayDetailCreationDTO>();
            var leaveTypes = await _context.LeaveTypes.ToListAsync();

            if(gender)
            {
                foreach (var item in leaveTypes)
                {
                    if(item.LeaveTypeId != 1)
                    {
                        leaveDayDetailCreationDTOs.Add(
                        new LeaveDayDetailCreationDTO
                        {
                            LeaveTypeId = item.LeaveTypeId,
                            DayLeft = item.LeaveTypeDay
                        });
                    }
                }
            }
            else
            {
                foreach (var item in leaveTypes)
                {
                    leaveDayDetailCreationDTOs.Add(
                        new LeaveDayDetailCreationDTO
                        {
                            LeaveTypeId = item.LeaveTypeId,
                            DayLeft = item.LeaveTypeDay
                        });
                }
            }
            return leaveDayDetailCreationDTOs;
        }

        public async Task<bool> IsLeaveDayDetailExist(int staffId)
        {
            return await _context.LeaveDayDetails.AnyAsync(c => c.StaffId == staffId);
        }

        public async Task<bool> IsLeaveTypeDetailExist(int staffId, int leaveTypeId)
        {
            return await _context.LeaveDayDetails.AnyAsync(c => c.StaffId == staffId && c.LeaveTypeId == leaveTypeId);
        }


        public async Task<bool> IsLeaveDayDetailValid(int staffId, int leaveTypeId, int leaveDays)
        {
            var isLeaveDayDetailValid = await _context.LeaveDayDetails
                .AnyAsync(c => c.StaffId == staffId &&
                        c.LeaveTypeId == leaveTypeId &&    
                        c.DayLeft >=  leaveDays);

            return isLeaveDayDetailValid;
        }

        public async Task UpdateLeaveDayDetail(int staffId, int leaveDayDetailId, LeaveDayDetailUpdateDTO leaveDayDetailUpdateDTO)
        {
            var leaveDayDetailOfStaff = await _context.LeaveDayDetails
                .Where(c => 
                    c.StaffId == staffId && 
                    c.LeaveDayDetailId == leaveDayDetailId &&
                    c.LeaveTypeId == leaveDayDetailUpdateDTO.LeaveTypeId)
                .FirstOrDefaultAsync();
        }

        public async Task DecreaseDayLeft(int staffId, int leaveTypeId, int leaveDays)
        {
            var leaveDayDetail = _context.LeaveDayDetails
                .Where(c => c.StaffId == staffId && c.LeaveTypeId == leaveTypeId) 
                .FirstOrDefault();

            var remainingDay = leaveDayDetail.DayLeft - leaveDays;

            if(remainingDay > 0 && leaveDayDetail != null) 
            {
                leaveDayDetail.DayLeft = remainingDay;
            }

            await _context.SaveChangesAsync();
        }

        public bool IsApproved(string message)
        {
            return message.Equals("approved");
        }

    }
}
