using AutoMapper;
using Logic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Constants;
using Web.RequestModels.Account;
using Database.Entities;
using System.ComponentModel.DataAnnotations;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService; 
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;

        public AccountController(IAccountService appDBContext1, IMapper mapper, IRoleService roleService)
        {
            _accountService = appDBContext1 ?? throw new ArgumentNullException(nameof(appDBContext1));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _roleService = roleService;
        }

        [HttpGet(nameof(GetAll))]
        [Authorize(IncludeModels.PolicyNavigation.OnlyAdminPolicyName)]
        public async Task<IActionResult> GetAll()
        {
            return Ok( _mapper.Map<List<UserViewModel>>(await _accountService.GetUsersAsync()));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            var login = User.Identity?.Name ?? throw new UnauthorizedAccessException();
            var mappedValue = _mapper.Map<UserViewModel>(await _accountService.FirstOrDefaultAsync(u => u.Login == login));
            return Ok(mappedValue);
        }

        [HttpPut]
        public async Task<IActionResult> Put(UserEditModel editModel)
        {
            bool isAdmin = _roleService.IsAdminRoleName(IncludeModels.UserIdentitiesTools.GetUserRoleClaimValue(User));
            var currentUserLogin = User.Identity?.Name ?? throw new UnauthorizedAccessException();
            if (!isAdmin && currentUserLogin != editModel.Login)
            {
                return BadRequest();
            }

            var updateData = _mapper.Map<User>(editModel);
            updateData.RoleId = isAdmin ? updateData.RoleId : -1;
            await _accountService.UpdateAsync(updateData);
            return Ok();
        }

        [HttpPut(nameof(UpdatePassword))]
        public async Task<IActionResult> UpdatePassword(string userLoginToUpdatePassword, [MinLength(6)][RegularExpression("^[a-zA-Z0-9]+$")] string newPassword)
        {
            bool isAdmin = _roleService.IsAdminRoleName(IncludeModels.UserIdentitiesTools.GetUserRoleClaimValue(User));
            var currentUserLogin = User.Identity?.Name ?? throw new UnauthorizedAccessException();
            if (!isAdmin && currentUserLogin != userLoginToUpdatePassword )
            {
                return BadRequest();
            }

            await _accountService.UpdatePasswordAsync(userLoginToUpdatePassword, newPassword);
            return Ok();
        }

        [HttpGet(nameof(GetRoles))]
        public List<Role> GetRoles() 
        {
            return _roleService.GetRange(u => u.ID >= 0).ToList();
        }
    }
}
