using AutoMapper;
using Database.Entities;
using Logic.Interfaces;
using Logic.Models.Contracts;
using Logic.Models.MonthReports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebFront.Constants;
using WebFront.Models.Account;
using WebFront.Models.Contracts;
using WebFront.Models.ContractType;
using WebFront.Models.Departments;

namespace WebFront.Controllers
{
    [Authorize]
    public class ContractsController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IContractTypeService _contractTypeService;
        private readonly IDepartmentService _departmentService;
        private readonly IRoleService _roleService;
        private readonly IContractService _contractService;
        private readonly IMapper _mapper;

        public ContractsController(IMapper mapper, IAccountService accountService, IContractService contractService, IDepartmentService departmentService, IRoleService roleService, IContractTypeService contractTypeService)
        {
            _departmentService = departmentService;
            _contractTypeService = contractTypeService;
            _contractService = contractService;
            _mapper = mapper;
            _accountService = accountService;
            _roleService = roleService;
        }

        [HttpGet("{controller}/" + nameof(List))]
        public async Task<IActionResult> List([FromQuery] ContractsSelectObject? selectionObject, int page = 1, int? pageSize = 10)
        {
            selectionObject ??= new ContractsSelectObject();
            if (!_roleService.IsAdminRoleName(IncludeModels.UserIdentitiesTools.GetUserRoleClaimValue(User)))
            {
                selectionObject.UserIDs = new List<int> { IncludeModels.UserIdentitiesTools.GetUserIDClaimValue(User) };
            }
            var res = await _contractService.GetListViaSelectionObjectAsync(selectionObject, page, pageSize);
            var ret = _mapper.Map<IEnumerable<ContractViewModel>>(res);

            ViewBag.Users = (await _accountService.GetListViaSelectionObjectAsync(null)).Select(p => new KeyValuePair<int, string>(p.ID, p.NSP));
            ViewBag.Departments = (await _departmentService.GetListViaSelectionObjectAsync(null)).Select(p => new KeyValuePair<int, string>(p.ID, p.Name));
            ViewBag.Contracts = (await _contractService.GetListViaSelectionObjectAsync(null)).Select(p => new KeyValuePair<int, string>(p.ID, p.ContractIdentifier));
            ViewBag.ContractTypes = (await _contractTypeService.GetListViaSelectionObjectAsync(null)).Select(p => new KeyValuePair<int, string>(p.ID, p.Name));
            ViewBag.selectionObject = selectionObject;
            ViewBag.page = page;
            ViewBag.pageSize = pageSize;

            return View("List", ret);
        }

        [HttpGet("{controller}/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            bool isAdmin = _roleService.IsAdminRoleName(IncludeModels.UserIdentitiesTools.GetUserRoleClaimValue(User));
            var currentUserLogin = User.Identity?.Name ?? throw new UnauthorizedAccessException();
            if (!isAdmin && currentUserLogin != await _contractService.GetOwnersLoginAsync(id))
            {
                return BadRequest("You have no rights to do it");
            }

            var contractToReturn = await _contractService.GetFullData(id);
            var orderedContracts = contractToReturn.Contracts.OrderBy(c => c.AssignmentDate);
            var orderedReports = _mapper.Map<IEnumerable<MonthReportViewModel>>(contractToReturn.Reports).OrderBy(r => r.Year).ThenBy(r => r.Month).ToList();
            var reportsIndex = 0;
            foreach (var item in orderedContracts)
            {
                var next = orderedContracts.FirstOrDefault(n => n.PeriodStart > item.PeriodStart);
                while (reportsIndex < orderedReports.Count && (next == null || (orderedReports[reportsIndex].Year <= next.PeriodStart.Year && orderedReports[reportsIndex].Month < next.PeriodStart.Month)))
                {
                    var report = orderedReports[reportsIndex];
                    report.ContractID = item.ID;
                    reportsIndex++;
                }
            }


            var mappedContract = new RelatedContractsWithReportsViewModel
            {
                RelatedContracts = _mapper.Map<IEnumerable<ContractViewModel>>(orderedContracts),
                MonthReports = orderedReports,
                UntakenTimes = contractToReturn.UntakenTimeForContracts == null ? Array.Empty<MonthReportsUntakenTimeModel>() : contractToReturn.UntakenTimeForContracts
            };
            ViewBag.UserNSP = (await _accountService.FirstOrDefaultAsync(u => u.ID == contractToReturn.Contracts.First().UserID))?.NSP ?? "Undefined";
            return View("Get", mappedContract);
        }


        [HttpGet("{controller}/Add")]
        public async Task<IActionResult> GetAddForm(int? forContractId)
        {
            if (forContractId.HasValue)
            {
                bool isAdmin = _roleService.IsAdminRoleName(IncludeModels.UserIdentitiesTools.GetUserRoleClaimValue(User));
                var currentUserId = IncludeModels.UserIdentitiesTools.GetUserIDClaimValue(User);
                var contractToSend = await _contractService.FirstOrDefaultAsync(c => c.ID == forContractId);
                if (contractToSend == null)
                {
                    return NotFound(IncludeModels.BadRequestTextFactory.GetObjectNotFoundExceptionText($"Id = {forContractId}"));
                }

                if (!isAdmin && currentUserId != contractToSend.UserID)
                {
                    return BadRequest(IncludeModels.BadRequestTextFactory.GetNoRightsExceptionText());
                }

                if (!contractToSend.IsConfirmed)
                {
                    return BadRequest("This contract is not confirmed");
                }

                ViewBag.UserNSP = (await _accountService.FirstOrDefaultAsync(u => u.ID == contractToSend.UserID))?.NSP ?? "Undefined";
                return View("AddChild", _mapper.Map<ContractViewModel>(contractToSend));
            }

            var deps = await _departmentService.GetListViaSelectionObjectAsync(null, 1, int.MaxValue);
            var types = await _contractTypeService.GetListViaSelectionObjectAsync(null, 1, int.MaxValue);
            ViewBag.Departments = _mapper.Map<IEnumerable<DepartmentViewModel>>(deps);
            ViewBag.ContractTypes = _mapper.Map<IEnumerable<ContractTypeViewModel>>(types);
            if (IncludeModels.UserIdentitiesTools.GetUserIsAdminClaimValue(User))
            {
                ViewBag.Users = _mapper.Map<IEnumerable<UserViewModel>>(await _accountService.GetListViaSelectionObjectAsync(null, 1, int.MaxValue));
            }

            return View("Add");
        }

        [HttpPost("{controller}/Add")]
        public async Task<IActionResult> Post(ContractCreateModel createModel)
        {
            var contractToAdd = _mapper.Map<Contract>(createModel);
            if (!_roleService.IsAdminRoleName(IncludeModels.UserIdentitiesTools.GetUserRoleClaimValue(User)))
            {
                contractToAdd.UserID = IncludeModels.UserIdentitiesTools.GetUserIDClaimValue(User);
            }

            await _contractService.AddAsync(contractToAdd);
            return Redirect(nameof(List));
        }

        [Authorize(IncludeModels.PolicyNavigation.OnlyAdminPolicyName)]
        [HttpGet("{controller}/Edit/{id}")]
        public async Task<IActionResult> GetEditForm(int id)
        {
            var ret = await _contractService.FirstOrDefaultAsync(d => d.ID == id);
            if (ret == null)
            {
                return NotFound();
            }

            var deps = await _departmentService.GetListViaSelectionObjectAsync(null, 1, int.MaxValue);
            var types = await _contractTypeService.GetListViaSelectionObjectAsync(null, 1, int.MaxValue);
            ViewBag.Departments = _mapper.Map<IEnumerable<DepartmentViewModel>>(deps);
            ViewBag.ContractTypes = _mapper.Map<IEnumerable<ContractTypeViewModel>>(types);
            if (IncludeModels.UserIdentitiesTools.GetUserIsAdminClaimValue(User))
            {
                ViewBag.Users = _mapper.Map<IEnumerable<UserViewModel>>(await _accountService.GetListViaSelectionObjectAsync(null, 1, int.MaxValue));
            }

            return View("Edit", _mapper.Map<ContractViewModel>(ret));
        }

        [HttpPost("{controller}/Edit")]
        public async Task<ActionResult> Put(ContractEditModel editModel)
        {
            bool isAdmin = _roleService.IsAdminRoleName(IncludeModels.UserIdentitiesTools.GetUserRoleClaimValue(User));
            var currentUserID = IncludeModels.UserIdentitiesTools.GetUserIDClaimValue(User);
            editModel.UserId = currentUserID;
            var contractToEdit = await _contractService.FirstOrDefaultAsync(c => c.ID == editModel.Id);
            if (contractToEdit == null)
            {
                return BadRequest($"Contract with ID = {editModel.Id} not found");
            }

            if (!isAdmin && currentUserID != contractToEdit.UserID)
            {
                return BadRequest("You have no rights to do it");
            }

            var newContract = _mapper.Map<Contract>(editModel);
            if (!_roleService.IsAdminRoleName(IncludeModels.UserIdentitiesTools.GetUserRoleClaimValue(User)))
            {
                newContract.UserID = IncludeModels.UserIdentitiesTools.GetUserIDClaimValue(User);
            }

            await _contractService.UpdateAsync(newContract);
            return RedirectToAction(nameof(List));
        }

        [HttpGet("{controller}/" + nameof(Delete) + "/{id}")]
        public async Task<IActionResult> DeleteForm(int id)
        {
            bool isAdmin = _roleService.IsAdminRoleName(IncludeModels.UserIdentitiesTools.GetUserRoleClaimValue(User));
            var currentUserID = IncludeModels.UserIdentitiesTools.GetUserIDClaimValue(User);

            var valueToRemove = await _contractService.FirstOrDefaultAsync(c => c.ID == id);
            if (valueToRemove == null)
            {
                return BadRequest($"Contract with ID = {id} not found");
            }

            if (!isAdmin && currentUserID != valueToRemove.UserID)
            {
                return BadRequest("You have no rights to do it");
            }

            return View("Delete", _mapper.Map<ContractViewModel>(valueToRemove));
        }

        [HttpPost("{controller}/" + nameof(Delete) + "/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            bool isAdmin = _roleService.IsAdminRoleName(IncludeModels.UserIdentitiesTools.GetUserRoleClaimValue(User));
            var currentUserID = IncludeModels.UserIdentitiesTools.GetUserIDClaimValue(User);
            var valueToRemove = await _contractService.FirstOrDefaultAsync(c => c.ID == id);
            if (valueToRemove == null)
            {
                return BadRequest($"Contract with ID = {id} not found");
            }

            if (!isAdmin && currentUserID != valueToRemove.UserID)
            {
                return BadRequest("You have no rights to do it");
            }

            await _contractService.DeleteAsync(valueToRemove);
            return RedirectToAction(nameof(List));
        }

        [HttpPost("{controller}/" + nameof(Confirm) + "/{id}")]
        [Authorize(IncludeModels.PolicyNavigation.OnlyAdminPolicyName)]
        public async Task<IActionResult> Confirm(int id)
        {
            await _contractService.ConfirmAsync(id, User.Identity?.Name ?? throw new UnauthorizedAccessException());
            return RedirectToAction(nameof(List));
        }

        [HttpGet("{controller}/" + nameof(GetMonthReports) + "/{id}")]
        public async Task<IActionResult> GetMonthReports(int id)
        {
            bool isAdmin = _roleService.IsAdminRoleName(IncludeModels.UserIdentitiesTools.GetUserRoleClaimValue(User));
            var currentUserLogin = User.Identity?.Name ?? throw new UnauthorizedAccessException();
            if (!isAdmin && currentUserLogin != await _contractService.GetOwnersLoginAsync(id))
            {
                return BadRequest("Contract not found or you do not have rights to do it");
            }

            var ret = _mapper.Map<IEnumerable<MonthReportViewModel>>(await _contractService.GetMonthReportsAsync(id));
            foreach (var item in ret)
            {
                item.ContractID = id;
            }

            return Ok(ret);
        }

        [HttpGet("{controller}/" + nameof(GetUntakenTime))]
        public async Task<IActionResult> GetUntakenTime(int id)
        {
            bool isAdmin = _roleService.IsAdminRoleName(IncludeModels.UserIdentitiesTools.GetUserRoleClaimValue(User));
            var currentUserLogin = User.Identity?.Name ?? throw new UnauthorizedAccessException();
            if (!isAdmin && currentUserLogin != await _contractService.GetOwnersLoginAsync(id))
            {
                return BadRequest("You do not have rights to do it");
            }

            return Ok(await _contractService.GetUntakenTimeAsync(id, Enumerable.Empty<(int, int)>()));
        }

        [HttpGet("{controller}/" + nameof(EditMonthReport))]
        public async Task<IActionResult> GetEditMonthReportForm(int contractID, int month, int year)
        {
            bool isAdmin = _roleService.IsAdminRoleName(IncludeModels.UserIdentitiesTools.GetUserRoleClaimValue(User));
            var currentUserLogin = IncludeModels.UserIdentitiesTools.GetUserIDClaimValue(User);
            var contract = await _contractService.FirstOrDefaultAsync(e => e.ID == contractID);
            if (contract == null)
            {
                return BadRequest($"Contract with ID = {contractID} not found");
            }

            if (!contract.IsConfirmed)
            {
                return BadRequest("Contract is not confirmed");
            }

            var linkingPartID = contract.LinkingPartID ?? throw new ArgumentException($"{nameof(contract.LinkingPartID)} is null");
            if (!isAdmin && currentUserLogin != contract.UserID)
            {
                return BadRequest("You have no rights to do it");
            }

            var monthReport = _mapper.Map<MonthReportViewModel>(await _contractService.GetReport(linkingPartID, month, year));

            if (monthReport.IsBlocked)
            {
                return BadRequest("Month report is blocked");
            }

            monthReport.LinkingPartID = linkingPartID;
            monthReport.ContractID = contractID;

            ViewBag.MaxVals = await _contractService.GetMaxValuesForReport(linkingPartID, year, month);
            ViewBag.Identifier = contract.ContractIdentifier;

            return View("ReportEdit", monthReport);
        }

        [HttpPost("{controller}/" + nameof(EditMonthReport))]
        public async Task<IActionResult> EditMonthReport(MonthReportEditModel editModel)
        {
            bool isAdmin = _roleService.IsAdminRoleName(IncludeModels.UserIdentitiesTools.GetUserRoleClaimValue(User));
            var currentUserLogin = IncludeModels.UserIdentitiesTools.GetUserIDClaimValue(User);
            var contract = await _contractService.FirstOrDefaultAsync(e => e.ID == editModel.ContractID);
            if (contract == null)
            {
                return BadRequest($"Contract with ID = {editModel.ContractID} not found");
            }

            if (!contract.IsConfirmed)
            {
                return BadRequest("Contract is not confirmed");
            }

            if (!isAdmin && currentUserLogin != contract.UserID)
            {
                return BadRequest("You have no rights to do it");
            }


            var monthReportToApply = _mapper.Map<MonthReport>(editModel);
            monthReportToApply.LinkingPartID = contract?.LinkingPartID ?? throw new ArgumentException($"{nameof(contract.LinkingPartID)} is null");
            await _contractService.UpdateMonthReport(monthReportToApply);
            return RedirectToAction("Get", new { id = contract.ID });
        }

        [HttpGet("{controller}/" + nameof(GetMonthReportsReport))]
        [Authorize(IncludeModels.PolicyNavigation.OnlyAdminPolicyName)]
        public async Task<IActionResult> GetMonthReportsReport(DateTime periodStart, DateTime periodEnd)
        {
            if (periodEnd <= periodStart) return BadRequest("periodEnd <= periodStart");
            var res = await _contractService.GetReportsOnPeriodAsync(periodStart, periodEnd);
            var ret = res.Select(r => new RelatedContractsWithReportsViewModel { RelatedContracts = _mapper.Map<IEnumerable<ContractViewModel>>(r.Contracts), MonthReports = _mapper.Map<IEnumerable<MonthReportViewModel>>(r.Reports) });
            return Ok(ret);
        }

        [HttpPost("{controller}/" + nameof(BlockReport))]
        [Authorize(IncludeModels.PolicyNavigation.OnlyAdminPolicyName)]
        public async Task<IActionResult> BlockReport(int contractId, int linkingPartID, int year, int month)
        {
            await _contractService.BlockReport(linkingPartID, month, year, IncludeModels.UserIdentitiesTools.GetUserIDClaimValue(User));
            return RedirectToAction("Get", new { id = contractId });
        }

        [HttpPost("{controller}/" + nameof(UnBlockReport))]
        [Authorize(IncludeModels.PolicyNavigation.OnlyAdminPolicyName)]
        public async Task<IActionResult> UnBlockReport(int contractId, int linkingPartID, int year, int month)
        {
            await _contractService.UnBlockReport(linkingPartID, month, year, IncludeModels.UserIdentitiesTools.GetUserIDClaimValue(User));
            return RedirectToAction("Get", new { id = contractId });
        }
    }
}
