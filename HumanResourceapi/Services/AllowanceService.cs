using HumanResourceapi.DTOs.AllowanceDTO;
using HumanResourceapi.DTOs.PersonnelContractDTO;
using HumanResoureapi.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics.Contracts;
using System.Runtime.CompilerServices;

namespace HumanResourceapi.Services
{
    public class AllowanceService
    {
        private readonly SwpProjectContext _context;
        private readonly IMapper _mapper;
        private readonly UserInfoService _userInfoService;

        public AllowanceService(
            SwpProjectContext context,
            IMapper mapper,
            UserInfoService userInfoService

            )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _userInfoService = userInfoService ?? throw new ArgumentNullException(nameof(userInfoService));
        }
        public async Task<List<AllowanceDTO>> GetAllowanceDTOs()
        {
            var allowances = await _context.Allowances
                                .Include(c => c.AllowanceType)
                                .ToListAsync();

            var finalAllowances = _mapper.Map<List<AllowanceDTO>>(allowances);
            return finalAllowances;
        }

        public async Task<bool> SaveChangeAsync()
        {
            return await _context.SaveChangesAsync() >= 0;
        }

        public async Task<bool> IsContractExist(int contractId)
        {
            return await _context.PersonnelContracts.AnyAsync(c => c.ContractId == contractId);
        }
        public async Task<AllowanceDTO> GetAllowanceByAllowanceId(int contractId, int allowanceId)
        {
            var allowance = await _context.Allowances
                                            .Include(c => c.AllowanceType)
                                            .Where(c => c.ContractId == contractId && c.AllowanceId == allowanceId)
                                            .FirstOrDefaultAsync();

            var returnAllowance = _mapper.Map<AllowanceDTO>(allowance);

            return returnAllowance;
        }

        public async Task<List<AllowanceDTO>> GetAllowanceByContractId(int contractId)
        {
            var allowances = await _context.Allowances
                                .Include(c => c.AllowanceType)
                                .Where(c => c.ContractId == contractId).ToListAsync();

            var finalAllowances = _mapper.Map<List<AllowanceDTO>>(allowances);
            return finalAllowances;
        }

        public async Task<PersonnelContract?> GetPersonnelContractAllowance(int contractId)
        {
            return await _context.PersonnelContracts
                                .Include(c => c.Allowances)
                                .Where(c => c.ContractId == contractId)
                                .FirstOrDefaultAsync();
        }

        public async Task<bool> IsAllowanceTypeValid(int allowanceTypeId)
        {
            return await _context.AllowanceTypes.AnyAsync(c => c.AllowanceTypeId == allowanceTypeId);
        }
        public async Task<AllowanceDTO> CreateAllowance(int contractId, AllowanceCreationDTO allowanceCreationDTO)
        {
            var allowance = _mapper.Map<Allowance>(allowanceCreationDTO);

            var allowanceFromStore = await GetPersonnelContractAllowance(contractId);

            allowanceFromStore.Allowances.Add(allowance);

            await _context.SaveChangesAsync();

            var returnAllowance = _mapper.Map<AllowanceDTO>(allowance);

            return returnAllowance;
        }

        public async Task UpdateAllowance(int contractId, int allowanceId,AllowanceUpdateDTO allowanceUpdateDTO)
        {

            var allowance = await _context.Allowances
                .Where(c => c.ContractId == contractId && c.AllowanceId == allowanceId)
                .FirstOrDefaultAsync();

            var returnAllowance = _mapper.Map(allowanceUpdateDTO, allowance);

            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsProjectAlreadyHaveAllowanceType(int contractId, int allowanceTypeId)
        {
            return await _context.Allowances
                .AnyAsync(c => c.ContractId == contractId && c.AllowanceTypeId == allowanceTypeId);
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
    }
}
