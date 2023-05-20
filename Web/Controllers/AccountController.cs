﻿using AutoMapper;
using Logic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.Constants;
using Web.RequestModels.Account;
using Database.Entities;
using System.ComponentModel.DataAnnotations;
using Logic.Services;
using Logic.Models.User;
using Logic.Models.Role;
using Web.Models.Account;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
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
        public async Task<IActionResult> GetAll([FromQuery] UserSelectObject? selectObject, int? page = default, int? pageSize = default)
        {
            return Ok( _mapper.Map<List<UserViewModel>>(await _accountService.GetListViaSelectionObjectAsync(selectObject, page, pageSize)));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            var login = User.Identity?.Name ?? throw new UnauthorizedAccessException();
            var mappedValue = _mapper.Map<UserViewModel>(await _accountService.FirstOrDefaultAsync(u => u.Login == login));

            return Ok(new AccountFromToken
            {
                ID = IncludeModels.UserIdentitiesTools.GetUserIDClaimValue(User),
                Login = IncludeModels.UserIdentitiesTools.GetUserNameClaimValue(User),
                Role = IncludeModels.UserIdentitiesTools.GetUserRoleClaimValue(User),
                UserAccountInDB = mappedValue
            });
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

        [Authorize]
        [HttpGet(nameof(GetRoles))]
        public async Task<IActionResult> GetRoles([FromQuery]RoleSelectObject? selectObject)
        {
            return Ok(await _roleService.GetListViaSelectionObjectAsync(selectObject));
        }

        #region DegreeAssignment
        [HttpGet("DegreeAssignment")]
        //[Authorize(IncludeModels.PolicyNavigation.OnlySuperAdminPolicyName)]
        public async Task<IActionResult> GetDegreeAssignment()
        {
            return Ok(await _accountService.GetAllAssignmentsAsync());
        }

        [HttpGet("{id}/DegreeAssignment")]
        //[Authorize(IncludeModels.PolicyNavigation.OnlySuperAdminPolicyName)]
        public async Task<IActionResult> GetDegreeAssignment(int id)
        {
            return Ok(await _accountService.GetAssignmentsForObject(id));
        }

        [HttpGet("{id}/DegreeAssignment/{date}")]
        //[Authorize(IncludeModels.PolicyNavigation.OnlySuperAdminPolicyName)]
        public async Task<IActionResult> GetDegreeAssignment(int id, DateTime date)
        {
            var ret = await _accountService.GetAssignmentOnDate(date, id);
            return ret is null ? BadRequest("No data found") : Ok(ret);
        }

        [HttpPut("{id}/DegreeAssignment")]
        //[Authorize(IncludeModels.PolicyNavigation.OnlySuperAdminPolicyName)]
        public async Task<IActionResult> PutDegreeAssignment(int id, DateTime assignationActiveDate, int newValue, DateTime? newAssignationDate)
        {
            await _accountService.EditAssignmentAsync(id, assignationActiveDate, newValue, newAssignationDate, CancellationToken.None);
            return Ok();
        }

        [HttpPost("{id}/DegreeAssignment")]
        //[Authorize(IncludeModels.PolicyNavigation.OnlySuperAdminPolicyName)]
        public async Task<IActionResult> PostDegreeAssignment(int id, DateTime assignationActiveDate, int Value, CancellationToken token = default)
        {
            var newAssignation = new UserAcademicDegreeAssignament { AssignmentDate = assignationActiveDate, Value = Value, ObjectIdentifier = id };
            await _accountService.AddAssignmentAsync(newAssignation, token);
            return Ok();
        }

        [HttpDelete("{id}/DegreeAssignment")]
        public async Task<IActionResult> DeletePriceAssignment(int id, DateTime assignationActiveDate, CancellationToken token = default)
        {
            await _accountService.RemoveAssignmentAsync(id, assignationActiveDate, token);
            return Ok();
        }
        #endregion
    }
}
