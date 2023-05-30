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
using WebFront.Models.Role;

namespace WebFront.Controllers
{
    [Controller]
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IAcademicDegreeService _academicDegreeService;
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;

        public AccountController(IAccountService appDBContext1, IMapper mapper, IRoleService roleService, IAcademicDegreeService academicDegreeService)
        {
            _accountService = appDBContext1 ?? throw new ArgumentNullException(nameof(appDBContext1));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _academicDegreeService = academicDegreeService;
            _roleService = roleService;
        }

        [HttpGet("{controller}/" + nameof(List))]
        [Authorize(IncludeModels.PolicyNavigation.OnlyAdminPolicyName)]
        public async Task<IActionResult> List([FromQuery] UserSelectObject? selectObject, int page = 1, int pageSize = 10)
        {
            ViewBag.Roles = await _roleService.GetListViaSelectionObjectAsync(null);
            ViewBag.selectionObject = selectObject;
            ViewBag.page = page;
            ViewBag.pageSize = pageSize;

            return View("list", _mapper.Map<IEnumerable<UserViewModel>>(await _accountService.GetListViaSelectionObjectAsync(selectObject, page, pageSize)));
        }

        [HttpGet("{controller}/{id}")]
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

            ViewBag.RoleName = (await _roleService.FirstOrDefaultAsync(r => r.ID == user.RoleId))?.Name ?? "RoleNotFound";
            var mappedValue = _mapper.Map<UserViewModel>(user);
            return View("Get", mappedValue);
        }

        [HttpGet("{controller}/Edit/{id}")]
        public async Task<IActionResult> GetEditForm(int id)
        {
            id = id == 0 ? IncludeModels.UserIdentitiesTools.GetUserIDClaimValue(User) : id;
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

            ViewBag.Roles = await _roleService.GetListViaSelectionObjectAsync(null);
            var mappedValue = _mapper.Map<UserViewModel>(user);
            return View("Edit", mappedValue);
        }

        [HttpPost("{controller}/Edit")]
        public async Task<IActionResult> Put(UserEditModel editModel)
        {
            int currentUserRoleID = IncludeModels.UserIdentitiesTools.GetUserIDClaimValue(User);
            bool isAdmin = _roleService.IsAdminRoleName(IncludeModels.UserIdentitiesTools.GetUserRoleClaimValue(User));
            var currentUserLogin = User.Identity?.Name ?? throw new UnauthorizedAccessException();
            if (!isAdmin && currentUserLogin != editModel.Login)
            {
                return BadRequest(IncludeModels.BadRequestTextFactory.GetNoRightsExceptionText());
            }

            var updateData = _mapper.Map<User>(editModel);
            updateData.RoleId = isAdmin ? updateData.RoleId : -1;
            if (Database.Constants.IncludeModels.RolesNavigation.SuperAdminRoleID == updateData.RoleId && Database.Constants.IncludeModels.RolesNavigation.SuperAdminRoleID != currentUserRoleID)
            {
                return BadRequest(IncludeModels.BadRequestTextFactory.GetNoRightsExceptionText());
            }

            await _accountService.UpdateAsync(updateData);
            if (isAdmin)
            {
                return RedirectToAction(nameof(List));
            }

            return RedirectToAction("Get", new { id = 0 });
        }

        [HttpPost("{controller}/" + nameof(UpdatePassword))]
        public async Task<IActionResult> UpdatePassword(string userLoginToUpdatePassword, [MinLength(6)][RegularExpression("^[a-zA-Z0-9]+$")] string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                return BadRequest("Password is not equal to confirmPassword");
            }

            if (newPassword == "password")
            {
                return BadRequest("Password can not be 'password'");
            }

            bool isAdmin = _roleService.IsAdminRoleName(IncludeModels.UserIdentitiesTools.GetUserRoleClaimValue(User));
            var currentUserLogin = User.Identity?.Name ?? throw new UnauthorizedAccessException();
            if (!isAdmin && currentUserLogin != userLoginToUpdatePassword)
            {
                return BadRequest(IncludeModels.BadRequestTextFactory.GetNoRightsExceptionText());
            }

            await _accountService.UpdatePasswordAsync(userLoginToUpdatePassword, newPassword);
            return await Get(0);
        }

        [HttpGet("{controller}/" + nameof(GetRoles))]
        public async Task<IActionResult> GetRoles([FromQuery] RoleSelectObject? selectObject)
        {
            return Ok(_mapper.Map<IEnumerable<RoleViewModel>>(await _roleService.GetListViaSelectionObjectAsync(selectObject)));
        }

        #region DegreeAssignment
        [HttpGet("{controller}/Assignments")]
        [Authorize(IncludeModels.PolicyNavigation.OnlyAdminPolicyName)]
        public async Task<IActionResult> GetDegreeAssignment()
        {
            var ret = await _accountService.GetAllAssignmentsAsync();
            var atachedUsers = await _accountService.GetListViaSelectionObjectAsync(new() { IDs = ret.Select(r => r.ObjectIdentifier) });
            var atachedDegrees = await _academicDegreeService.GetListViaSelectionObjectAsync(new() { IDs = ret.Select(r => r.Value) });
            ViewBag.forID = null;
            ViewBag.degrees = atachedDegrees;
            ViewBag.Message = "Выборка для всех пользователей";
            return View("Assigments", ret);
        }

        [HttpGet("{controller}/Assignments/{id}")]
        public async Task<IActionResult> GetDegreeAssignments(int id)
        {
            bool isAdmin = _roleService.IsAdminRoleName(IncludeModels.UserIdentitiesTools.GetUserRoleClaimValue(User));
            var currentUserId = IncludeModels.UserIdentitiesTools.GetUserIDClaimValue(User);
            if (!isAdmin && currentUserId != id)
            {
                return BadRequest(IncludeModels.BadRequestTextFactory.GetNoRightsExceptionText());
            }


            var ret = await _accountService.GetAssignmentsForObject(id);
            var atachedUsers = await _accountService.GetListViaSelectionObjectAsync(new() { IDs = ret.Select(r => r.ObjectIdentifier) });
            ViewBag.users = atachedUsers;
            var atachedDegrees = await _academicDegreeService.GetListViaSelectionObjectAsync(new() { IDs = ret.Select(r => r.Value) });
            ViewBag.degrees = atachedDegrees;
            ViewBag.forID = id;
            ViewBag.Message = $"Выборка для {(await _accountService.FirstOrDefaultAsync(a => a.ID == id))?.NSP ?? "Unknown"}";
            return View("Assigments", ret);
        }

        //[HttpGet("{userId}/DegreeAssignments/{date}")]
        //public async Task<IActionResult> GetDegreeAssignment(int userId, DateTime date)
        //{
        //    bool isAdmin = _roleService.IsAdminRoleName(IncludeModels.UserIdentitiesTools.GetUserRoleClaimValue(User));
        //    var currentUserId = IncludeModels.UserIdentitiesTools.GetUserIDClaimValue(User);
        //    if (!isAdmin && currentUserId != userId)
        //    {
        //        return BadRequest(IncludeModels.BadRequestTextFactory.GetNoRightsExceptionText());
        //    }

        //    var ret = await _accountService.GetAssignmentOnDate(date, userId);
        //    return ret is null ? BadRequest("No data found") : Ok(ret);
        //}

        [HttpGet("{controller}/" + nameof(EditAssignment))]
        [Authorize(IncludeModels.PolicyNavigation.OnlyAdminPolicyName)]
        public async Task<IActionResult> GetEditAssignmentForm(int id, DateTime assignmentActiveDate)
        {
            var ret = await _accountService.GetAssignmentOnDate(assignmentActiveDate, id);
            if (ret == null)
            {
                return NotFound();
            }
            var atachedDegrees = await _academicDegreeService.GetListViaSelectionObjectAsync(null);
            ViewBag.degrees = atachedDegrees;
            return View("AssignmentEdit", ret);
        }

        [HttpPost("{controller}/" + nameof(EditAssignment))]
        [Authorize(IncludeModels.PolicyNavigation.OnlyAdminPolicyName)]
        public async Task<IActionResult> EditAssignment(int id, DateTime assignationActiveDate, int newValue, DateTime? newAssignationDate)
        {
            await _accountService.EditAssignmentAsync(id, assignationActiveDate, newValue, newAssignationDate, CancellationToken.None);
            return RedirectToAction("Assignments", new { id });
        }

        [HttpGet("{controller}/" + nameof(AddAssignment))]
        [Authorize(IncludeModels.PolicyNavigation.OnlyAdminPolicyName)]
        public async Task<IActionResult> GetAddAssignmentForm(int? forID)
        {
            ViewBag.forID = forID;
            var atachedUsers = await _accountService.GetListViaSelectionObjectAsync(null);
            ViewBag.users = atachedUsers;
            var atachedDegrees = await _academicDegreeService.GetListViaSelectionObjectAsync(null);
            ViewBag.degrees = atachedDegrees;
            return View("AssignmentAdd");
        }

        [HttpPost("{controller}/" + nameof(AddAssignment))]
        [Authorize(IncludeModels.PolicyNavigation.OnlyAdminPolicyName)]
        public async Task<IActionResult> AddAssignment(int id, DateTime assignmentDate, int Value, CancellationToken token = default)
        {
            var newAssignation = new UserAcademicDegreeAssignament { AssignmentDate = assignmentDate, Value = Value, ObjectIdentifier = id };
            await _accountService.AddAssignmentAsync(newAssignation, token);
            return RedirectToAction("Assignments", new { id });
        }

        [HttpPost("{controller}/" + nameof(DeleteAssignment))]
        [Authorize(IncludeModels.PolicyNavigation.OnlyAdminPolicyName)]
        public async Task<IActionResult> DeleteAssignment(int id, DateTime assignmentDate, CancellationToken token = default)
        {
            await _accountService.RemoveAssignmentAsync(id, assignmentDate, token);
            return RedirectToAction("Assignments");
        }
        #endregion
    }
}
