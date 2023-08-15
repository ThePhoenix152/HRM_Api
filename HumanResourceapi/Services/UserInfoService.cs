using HumanResoureapi.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace HumanResourceapi.Services
{
    public class UserInfoService
    {
        private readonly SwpProjectContext _context;
        private readonly IMapper _mapper;

        public UserInfoService
            (SwpProjectContext context, IMapper mapper)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        
        public async Task<bool> IsUserExist(int StaffId)
        {
            return await _context.UserInfors.Where(c => c.StaffId == StaffId && c.AccountStatus == true).AnyAsync();
        }

        public async Task<bool> IsSaveChangeAsync()
        {
            return await _context.SaveChangesAsync() >= 0;
        }
        
        public async Task<List<int>> GetStaffIdsAsync()
        {
            var staffs = await _context.UserInfors
                .Include(c => c.PersonnelContracts)
                .Where(c => 
                c.PersonnelContracts.Any(c => c.ContractStatus == true))
                .ToListAsync();
            return staffs.Select(c => c.StaffId).ToList();
        }

        public async Task<List<int>> GetStaffsOfDepartment(int departmentId)
        {
            var staffs = await _context.UserInfors
                .Include(c => c.PersonnelContracts)
                .Where(c => c.DepartmentId == departmentId &&
                c.PersonnelContracts.Any(c => c.ContractStatus == true))
                .ToListAsync();

            return staffs.Select(c => c.StaffId).ToList();
        }

    }
}
