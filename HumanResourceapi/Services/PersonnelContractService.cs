using HumanResourceapi.DTOs.PersonnelContractDTO;
using HumanResoureapi.Models;
using HumanResourceapi.Extensions;
using HumanResourceapi.RequestHelpers;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace HumanResourceapi.Services
{
  public class PersonnelContractService
  {
    private readonly SwpProjectContext _context;
    private readonly IMapper _mapper;
    private readonly UserInfoService _userInfoService;

    public PersonnelContractService(
        SwpProjectContext context,
        IMapper mapper,
        UserInfoService userInfoService
        )
    {
      _context = context ?? throw new ArgumentNullException(nameof(context));
      _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
      _userInfoService = userInfoService ?? throw new ArgumentNullException(nameof(userInfoService));
    }

    public async Task<PagedList<PersonnelContractDTO>> GetPersonnelContractsAsync(
        ContractParams contractParams)
    {
      var personnelContracts =  _context.PersonnelContracts
                                      .Include(c => c.ContractType)
                                      .Include(c => c.Allowances)
                                      .ThenInclude(c => c.AllowanceType)
                                      .Include(c => c.Staff)
                                      .Search(contractParams.SearchTerm)
                                      .Filter(contractParams.Departments)
                                      .OrderByDescending(c => c.ContractId)
                                      .AsQueryable();

            var returnContracts = await PagedList<PersonnelContract>.ToPagedList(
                personnelContracts,
                contractParams.PageNumber,
                contractParams.PageSize);

            var mappedContracts = returnContracts.Select(p => _mapper.Map<PersonnelContractDTO>(p)).ToList();

            var finalContracts = new PagedList<PersonnelContractDTO>(
                mappedContracts,
                returnContracts.MetaData.TotalCount,
                contractParams.PageNumber,
                contractParams.PageSize);

            return finalContracts;
    }
    public async Task<PersonnelContractDTO> CreatePersonnelContract(int staffId, PersonnelContractCreationDTO personnelContractCreationDTO)
    {

      var PersonnelContract = _mapper.Map<PersonnelContract>(personnelContractCreationDTO);
      var userInfor = await GetUserInfoContract(staffId);

      userInfor.PersonnelContracts.Add(PersonnelContract);
      await _context.SaveChangesAsync();

      var returnPersonnelContract = _mapper.Map<PersonnelContractDTO>(PersonnelContract);

      return returnPersonnelContract;
    }

    private async Task<UserInfor?> GetUserInfoContract(int staffId)
    {
      return await _context.UserInfors
          .Include(c => c.PersonnelContracts)
          .Where(c => c.StaffId == staffId)
          .FirstOrDefaultAsync();
    }

    public async Task<List<PersonnelContractDTO>> GetPersonnelContractById(int staffId)
    {
      var personnelContract = await _context.PersonnelContracts
                                  .Include(c => c.ContractType)
                                  .Include(c => c.Allowances)
                                  .ThenInclude(c => c.AllowanceType)
                                  .Include(c => c.Staff)
                                  .Where(c => c.StaffId == staffId)
                                  .ToListAsync();

      var finalPersonnelContracts = _mapper.Map<List<PersonnelContractDTO>>(personnelContract);

      return finalPersonnelContracts;
    }

    public async Task<bool> IsContractTypeValid(int contractTypeId)
    {
      var IsContractTypeValid = await _context.ContractTypes.AnyAsync(c => c.ContractTypeId == contractTypeId);

      return IsContractTypeValid;
    }

    public async Task<PersonnelContract> GetValidPersonnelContractEntityByStaffId(int staffId)
    {
      return await _context.PersonnelContracts
                      .Where(c => c.StaffId == staffId && c.ContractStatus == true)
                      .FirstOrDefaultAsync();
    }

    public async Task<PersonnelContractDTO> GetValidPersonnelContractByStaffId(int staffId)
    {
      var validPersonnelContract = await _context.PersonnelContracts
          .Include(c => c.ContractType)
          .Include(c => c.Staff)
          .Include(c => c.Allowances)
          .ThenInclude(c => c.AllowanceType)
          .Where(c => c.StaffId == staffId && c.ContractStatus == true)
          .FirstOrDefaultAsync();

      var returnValidPersonnelContract = _mapper.Map<PersonnelContractDTO>(validPersonnelContract);

      return returnValidPersonnelContract;
    }
    public async Task<PersonnelContractDTO> GetContractByIdAndStaffId(int contractId, int staffId)
    {
      var validPersonnelContract = await _context.PersonnelContracts
          .Include(c => c.ContractType)
          .Include(c => c.Staff)
          .Include(c => c.Allowances)
          .ThenInclude(c => c.AllowanceType)
          .Where(c =>
              c.StaffId == staffId &&
              c.ContractId == contractId &&
              c.ContractStatus == true)
          .FirstOrDefaultAsync();

      var returnValidPersonnelContract = _mapper.Map<PersonnelContractDTO>(validPersonnelContract);

      return returnValidPersonnelContract;
    }

    public async Task<bool> IsContractTimeValid(DateTime start, DateTime end)
    {
      if (start >= end)
      {
        return false;   
      }

      if ((end - start).TotalDays < 30)
      {
        return false;
      }

      //if (start < DateTime.Today)
      //{
      //  return false;
      //}
      return true;
    }

    public async Task<bool> IsValidContractExist(int staffId)
    {
      var IsValidContractExist = await _context.PersonnelContracts
          .AnyAsync(c => c.ContractStatus == true && c.StaffId == staffId);

      return IsValidContractExist;
    }

    public async Task<int> GetNoDependencies(int staffId)
    {
      var noOfDependencies = await _context.PersonnelContracts
          .Where(c => c.StaffId == staffId)
          .Select(c => c.NoOfDependences)
          .FirstOrDefaultAsync();
      return (int)noOfDependencies;
    }

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

    public async Task<int> BasicGrossSalary(int staffId)
    {
      var personnelContract = await _context.PersonnelContracts
          .Where(c => c.StaffId == staffId && c.ContractStatus == true)
          .FirstOrDefaultAsync();

      if (personnelContract != null && personnelContract.SalaryType == "GrossToNet")
      {
        return personnelContract.Salary;
      }
      return 0;
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
  }
}
