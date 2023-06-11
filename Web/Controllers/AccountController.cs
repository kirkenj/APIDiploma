using AutoMapper;
using Database.Entities;
using Logic.Interfaces;
using Logic.Models.Role;
using Logic.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using WebFront.Constants;
using WebFront.Models.Account;
using WebFront.RequestModels.Account;

namespace WebFront.Controllers
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
            return Ok(_mapper.Map<List<UserViewModel>>(await _accountService.GetListViaSelectionObjectAsync(selectObject, page, pageSize)));
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

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            if (id == 0)
            {
                id = IncludeModels.UserIdentitiesTools.GetUserIDClaimValue(User);
            }

            var isAdmin = IncludeModels.UserIdentitiesTools.GetUserIsAdminClaimValue(User);
            var userID = IncludeModels.UserIdentitiesTools.GetUserIDClaimValue(User);
            if (!isAdmin && userID != id)
            {
                return BadRequest();
            }

            var user = await _accountService.FirstOrDefaultAsync(u => u.ID == id);
            if (user == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<UserViewModel>(user));
        }

        [HttpPut]
        public async Task<IActionResult> Put(UserEditModel editModel)
        {
            bool isAdmin = _roleService.IsAdminRoleName(IncludeModels.UserIdentitiesTools.GetUserRoleClaimValue(User));
            var currentUserLogin = User.Identity?.Name ?? throw new UnauthorizedAccessException();
            if (!isAdmin && currentUserLogin != editModel.Login)
            {
                return BadRequest(IncludeModels.BadRequestTextFactory.GetNoRightsExceptionText());
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
            if (!isAdmin && currentUserLogin != userLoginToUpdatePassword)
            {
                return BadRequest(IncludeModels.BadRequestTextFactory.GetNoRightsExceptionText());
            }

            await _accountService.UpdatePasswordAsync(userLoginToUpdatePassword, newPassword);
            return Ok();
        }

        [Authorize]
        [HttpGet(nameof(GetRoles))]
        public async Task<IActionResult> GetRoles([FromQuery] RoleSelectObject? selectObject)
        {
            return Ok(await _roleService.GetListViaSelectionObjectAsync(selectObject));
        }

        #region DegreeAssignment
        [HttpGet("DegreeAssignment")]
        [Authorize(IncludeModels.PolicyNavigation.OnlyAdminPolicyName)]
        public async Task<IActionResult> GetDegreeAssignment()
        {
            return Ok(await _accountService.GetAllAssignmentsAsync());
        }

        [HttpGet("DegreeAssignment/{userId}")]
        public async Task<IActionResult> GetDegreeAssignments(int userId)
        {
            bool isAdmin = _roleService.IsAdminRoleName(IncludeModels.UserIdentitiesTools.GetUserRoleClaimValue(User));
            var currentUserId = IncludeModels.UserIdentitiesTools.GetUserIDClaimValue(User);
            if (!isAdmin && currentUserId != userId)
            {
                return BadRequest(IncludeModels.BadRequestTextFactory.GetNoRightsExceptionText());
            }


            return Ok(await _accountService.GetAssignmentsForObject(userId));
        }

        [HttpGet("DegreeAssignment/{userId}/{date}")]
        public async Task<IActionResult> GetDegreeAssignment(int userId, DateTime date)
        {
            bool isAdmin = _roleService.IsAdminRoleName(IncludeModels.UserIdentitiesTools.GetUserRoleClaimValue(User));
            var currentUserId = IncludeModels.UserIdentitiesTools.GetUserIDClaimValue(User);
            if (!isAdmin && currentUserId != userId)
            {
                return BadRequest(IncludeModels.BadRequestTextFactory.GetNoRightsExceptionText());
            }

            var ret = await _accountService.GetAssignmentOnDate(date, userId);
            return ret is null ? BadRequest("No data found") : Ok(ret);
        }

        [HttpPut("DegreeAssignment/{userId}")]
        [Authorize(IncludeModels.PolicyNavigation.OnlyAdminPolicyName)]
        public async Task<IActionResult> PutDegreeAssignment(int userId, DateTime assignationActiveDate, int newValue, DateTime? newAssignationDate)
        {
            await _accountService.EditAssignmentAsync(userId, assignationActiveDate, newValue, newAssignationDate, CancellationToken.None);
            return Ok();
        }

        [HttpPost("DegreeAssignment/{userId}")]
        [Authorize(IncludeModels.PolicyNavigation.OnlyAdminPolicyName)]
        public async Task<IActionResult> PostDegreeAssignment(int userId, DateTime assignationActiveDate, int Value, CancellationToken token = default)
        {
            var newAssignation = new UserAcademicDegreeAssignment { AssignmentDate = assignationActiveDate, Value = Value, ObjectIdentifier = userId };
            await _accountService.AddAssignmentAsync(newAssignation, token);
            return Ok();
        }

        [HttpDelete("DegreeAssignment/{userId}")]
        [Authorize(IncludeModels.PolicyNavigation.OnlyAdminPolicyName)]
        public async Task<IActionResult> DeletePriceAssignment(int userId, DateTime assignationActiveDate, CancellationToken token = default)
        {
            await _accountService.RemoveAssignmentAsync(userId, assignationActiveDate, token);
            return Ok();
        }
        #endregion
    }
}
