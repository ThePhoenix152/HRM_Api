using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using HumanResourceapi.Controllers;
using HumanResourceapi.DTOs.UserInforDTO;
using HumanResoureapi.Models;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using HumanResourceapi.Models;

namespace HumanResourceapi.Services
{
  public class UserService
  {
    private readonly UserManager<User> _userManager;
    private readonly SwpProjectContext _context;
    private readonly IMapper _mapper;

    public UserService(UserManager<User> userManager, SwpProjectContext context, IMapper mapper)
    {
      _userManager = userManager;
      _context = context;
      _mapper = mapper;
    }

    // public async Task<UserInforDto> GetCurrentUserInfor(ClaimsPrincipal User)
    // {
    //   var user = await _userManager.FindByNameAsync(User.Identity.Name);

    //   var userInfor = _context.UserInfors.FirstOrDefault(c => c.Id == user.Id);

    //   var userInforDto = _mapper.Map<UserInforDto>(userInfor);

    //   return userInforDto;
    // }

    public async Task<UserInforDto> GetCurrentUserInfor(ClaimsPrincipal User)
    {
      using (var userManager = _userManager)
      {
        var user = await userManager.FindByNameAsync(User.Identity.Name);

        var userInforDto = _mapper.Map<UserInforDto>(await _context.UserInfors
          .SingleOrDefaultAsync(c => c.Id == user.Id));

        return userInforDto;
      }
    }

  }

}