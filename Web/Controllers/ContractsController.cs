﻿using AutoMapper;
using Database.Entities;
using Logic.Interfaces;
using Logic.Models.Contracts;
using Logic.Models.MonthReports;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebFront.Constants;
using WebFront.Models.Contracts;
using WebFront.RequestModels.Contracts;

namespace WebFront.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ContractsController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IRoleService _roleService;
        private readonly IContractService _contractService;
        private readonly IMapper _mapper;

        public ContractsController(IMapper mapper, IAccountService accountService, IContractService contractService, IRoleService roleService)
        {
            _contractService = contractService;
            _mapper = mapper;
            _accountService = accountService;
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] ContractsSelectObject? selectionObject, int? page = default, int? pageSize = default)
        {
            selectionObject ??= new ContractsSelectObject();
            if (!_roleService.IsAdminRoleName(IncludeModels.UserIdentitiesTools.GetUserRoleClaimValue(User)))
            {
                selectionObject.UserIDs = new List<int> { IncludeModels.UserIdentitiesTools.GetUserIDClaimValue(User) };
            }

            return Ok(_mapper.Map<List<ContractViewModel>>(await _contractService.GetPageContent(_contractService.GetContractHasChildKeyValuePair(selectionObject), page, pageSize).ToListAsync()));
        }

        [HttpGet("{contractID}")]
        public async Task<IActionResult> Get(int contractID)
        {
            bool isAdmin = _roleService.IsAdminRoleName(IncludeModels.UserIdentitiesTools.GetUserRoleClaimValue(User));
            var currentUserLogin = User.Identity?.Name ?? throw new UnauthorizedAccessException();
            if (!isAdmin && currentUserLogin != await _contractService.GetOwnersLoginAsync(contractID))
            {
                return BadRequest("You have no rights to do it");
            }

            var contractToReturn = await _contractService.GetFullData(contractID);
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

            return Ok(
                new RelatedContractsWithReportsViewModel
                {
                    RelatedContracts = _mapper.Map<IEnumerable<ContractViewModel>>(orderedContracts),
                    MonthReports = orderedReports,
                    UntakenTimes = contractToReturn.UntakenTimeForContracts ?? Array.Empty<MonthReportsUntakenTimeModel>()
                }
            );
        }

        [HttpPost]
        public async Task<IActionResult> Post(ContractCreateModel createModel)
        {
            var contractToAdd = _mapper.Map<Contract>(createModel);
            if (!_roleService.IsAdminRoleName(IncludeModels.UserIdentitiesTools.GetUserRoleClaimValue(User)))
            {
                contractToAdd.UserID = IncludeModels.UserIdentitiesTools.GetUserIDClaimValue(User);
            }

            await _contractService.AddAsync(contractToAdd);
            return Ok();
        }

        [HttpPut]
        public async Task<ActionResult> Put(ContractEditModel editModel)
        {
            bool isAdmin = _roleService.IsAdminRoleName(IncludeModels.UserIdentitiesTools.GetUserRoleClaimValue(User));
            var currentUserID = IncludeModels.UserIdentitiesTools.GetUserIDClaimValue(User);
            editModel.UserId = currentUserID;
            var contractToEdit = await _contractService.FirstOrDefaultAsync(c => c.ID == editModel.ContractID);
            if (contractToEdit == null)
            {
                return BadRequest($"Contract with ID = {editModel.ContractID} not found");
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
            return Ok();
        }

        [HttpDelete("{contractID}")]
        public async Task<IActionResult> Delete(int contractID)
        {
            bool isAdmin = _roleService.IsAdminRoleName(IncludeModels.UserIdentitiesTools.GetUserRoleClaimValue(User));
            var currentUserID = IncludeModels.UserIdentitiesTools.GetUserIDClaimValue(User);
            var valueToRemove = await _contractService.FirstOrDefaultAsync(c => c.ID == contractID);
            if (valueToRemove == null)
            {
                return BadRequest($"Contract with ID = {contractID} not found");
            }

            if (!isAdmin && currentUserID != valueToRemove.UserID)
            {
                return BadRequest("You have no rights to do it");
            }

            await _contractService.DeleteAsync(valueToRemove);
            return Ok();
        }

        [HttpPost(nameof(Confirm) + "/{contractID}")]
        [Authorize(IncludeModels.PolicyNavigation.OnlyAdminPolicyName)]
        public async Task<IActionResult> Confirm(int contractID)
        {
            await _contractService.ConfirmAsync(contractID, User.Identity?.Name ?? throw new UnauthorizedAccessException());
            return Ok();
        }

        [HttpGet(nameof(GetMonthReports) + "/{contractID}")]
        public async Task<IActionResult> GetMonthReports(int contractID)
        {
            bool isAdmin = _roleService.IsAdminRoleName(IncludeModels.UserIdentitiesTools.GetUserRoleClaimValue(User));
            var currentUserLogin = User.Identity?.Name ?? throw new UnauthorizedAccessException();
            if (!isAdmin && currentUserLogin != await _contractService.GetOwnersLoginAsync(contractID))
            {
                return BadRequest("Contract not found or you do not have rights to do it");
            }

            var ret = _mapper.Map<IEnumerable<MonthReportViewModel>>(await _contractService.GetMonthReportsAsync(contractID));
            foreach (var item in ret)
            {
                item.ContractID = contractID;
            }

            return Ok(ret);
        }

        [HttpGet(nameof(GetMonthReport) + "/{contractID}/{year}/{month}")]
        public async Task<IActionResult> GetMonthReport(int contractID, int year, int month)
        {
            bool isAdmin = IncludeModels.UserIdentitiesTools.GetUserIsAdminClaimValue(User);
            var contract = await _contractService.FirstOrDefaultAsync(c => c.ID == contractID);
            if (contract == null)
            {
                return NotFound();
            }

            if (!isAdmin && contract.ID != IncludeModels.UserIdentitiesTools.GetUserIDClaimValue(User))
            {
                return BadRequest("Contract not found or you do not have rights to do it");
            }

            if (!contract.IsConfirmed)
            {
                return BadRequest("Contract is not confirmed");
            }

            if (contract.LinkingPartID == null)
            {
                return BadRequest("Contract doesn't have linking part. Contact administration");
            }

            var linkingPartID = contract.LinkingPartID.Value;
            var monthReport = _mapper.Map<MonthReportViewModel>(await _contractService.GetReport(linkingPartID, month, year));
            var maxVals = await _contractService.GetUntakenTimeAsync(contractID, new List<(int, int)> { (monthReport.Year, monthReport.Month) }, true);
            monthReport.ContractID = contractID;
            maxVals.ContractID = contractID;
            return Ok(new { report = monthReport, maxValues = maxVals });
        }

        [HttpGet(nameof(GetUntakenTime))]
        public async Task<IActionResult> GetUntakenTime(int contractID)
        {
            bool isAdmin = _roleService.IsAdminRoleName(IncludeModels.UserIdentitiesTools.GetUserRoleClaimValue(User));
            var currentUserLogin = User.Identity?.Name ?? throw new UnauthorizedAccessException();
            if (!isAdmin && currentUserLogin != await _contractService.GetOwnersLoginAsync(contractID))
            {
                return BadRequest("You do not have rights to do it");
            }

            return Ok(await _contractService.GetUntakenTimeAsync(contractID, Enumerable.Empty<(int, int)>()));
        }

        [HttpPut(nameof(EditMonthReport))]
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
            return Ok();
        }

        [HttpGet(nameof(GetMonthReportsReport))]
        [Authorize(IncludeModels.PolicyNavigation.OnlyAdminPolicyName)]
        public async Task<IActionResult> GetMonthReportsReport(DateTime periodStart, DateTime periodEnd)
        {
            if (periodEnd <= periodStart) return BadRequest("periodEnd <= periodStart");
            var res = await _contractService.GetReportsOnPeriodAsync(periodStart, periodEnd);
            var ret = res.Select(r => new RelatedContractsWithReportsViewModel { RelatedContracts = _mapper.Map<List<ContractViewModel>>(r.Contracts), MonthReports = _mapper.Map<List<MonthReportViewModel>>(r.Reports) });
            return Ok(ret);
        }

        [HttpPost(nameof(BlockReport))]
        [Authorize(IncludeModels.PolicyNavigation.OnlyAdminPolicyName)]
        public async Task<IActionResult> BlockReport(int linkingPartID, int year, int month)
        {
            await _contractService.BlockReport(linkingPartID, month, year, IncludeModels.UserIdentitiesTools.GetUserIDClaimValue(User));
            return Ok();
        }

        [HttpPost(nameof(UnBlockReport))]
        [Authorize(IncludeModels.PolicyNavigation.OnlyAdminPolicyName)]
        public async Task<IActionResult> UnBlockReport(int linkingPartID, int year, int month)
        {
            await _contractService.UnBlockReport(linkingPartID, month, year, IncludeModels.UserIdentitiesTools.GetUserIDClaimValue(User));
            return Ok();
        }
    }
}
