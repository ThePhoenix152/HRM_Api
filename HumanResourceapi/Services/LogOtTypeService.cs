using HumanResourceapi.DTOs.LogOtDTOs;
using HumanResoureapi.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace HumanResourceapi.Services
{
    public class LogOtTypeService
    {
        private readonly SwpProjectContext _context;
        private readonly IMapper _mapper;
        private readonly UserInfoService _userInfoService;

        public LogOtTypeService(
            SwpProjectContext context,
            IMapper mapper,
            UserInfoService userInfoService
            )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _userInfoService = userInfoService ?? throw new ArgumentNullException(nameof(userInfoService));
        }

        public async Task<List<OtTypeDTO>> GetLogOtType()
        {
            var logOtType = await _context.OtTypes.ToListAsync();

            var returnLogOtType = _mapper.Map<List<OtTypeDTO>>(logOtType);

            return returnLogOtType;
        }

    
    }
}
