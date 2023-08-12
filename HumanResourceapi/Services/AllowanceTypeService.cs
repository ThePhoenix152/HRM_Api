using HumanResourceapi.DTOs.PersonnelContractDTO;
using HumanResourceapi.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace HumanResourceapi.Services
{
    public class AllowanceTypeService
    {
        private readonly SwpProjectContext _context;
        private readonly IMapper _mapper;
        private readonly TheCalendarService _theCalendarService;
        private readonly UserInfoService _userInfoService;

        public AllowanceTypeService(
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

        public async Task<List<AllowanceTypeDTO>> GetAllowanceTypeDTOsAsync()
        {
            var allowanceTypes = await _context.AllowanceTypes.ToListAsync();

            var returnAllowanceTypes = _mapper.Map<List<AllowanceTypeDTO>>(allowanceTypes);

            return returnAllowanceTypes;
        }

        public async Task<List<AllowanceTypeDTO>> GetValidAllowanceTypes(int contractId)
        {
            //lấy những alllowance có trong hoppjw dồng
            var allowances = await _context.Allowances
                .Where(c => c.ContractId == contractId).ToListAsync();

            var allowancesInContract = allowances.Select(c => c.AllowanceTypeId);

            //muốn lấy những allowance Type chưa có trong hợp đồng 

            var validAllowances = await _context.AllowanceTypes.Where(c => !allowancesInContract.Contains(c.AllowanceTypeId)).ToListAsync();

            var returnValidAllowances = _mapper.Map<List<AllowanceTypeDTO>>(validAllowances);

            return returnValidAllowances;
        }

    }
}
