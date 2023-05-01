using Logic.Interfaces;
using Database.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Web.RequestModels.Contracts;
using AutoMapper;
using Web.Constants;

namespace Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ContractsController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IContractService _contractService;
        private readonly IMapper _mapper;

        public ContractsController(IMapper mapper, IAccountService accountService, IContractService contractService)
        {
            _contractService = contractService;
            _mapper = mapper;
            _accountService = accountService;
        }

        [HttpGet]
        public async Task<IActionResult> Get() 
        {
            string? userName = User.Identity?.Name ?? throw new UnauthorizedAccessException();
            return Ok(_mapper.Map<List<ContractViewModel>>(await _contractService.GetUserContracts(userName)));
        }
       
        [HttpGet("{contractID}")]
        public async Task<IActionResult> Get(int contractID) 
        {
            bool isAdmin = _accountService.IsAdmin(IncludeModels.UserIdentitiesTools.GetUserRoleClaimValue(User));
            var currentUserLogin = User.Identity?.Name ?? throw new UnauthorizedAccessException();
            if (!isAdmin && currentUserLogin != await _contractService.GetOwnersLoginAsync(contractID))
            {
                return BadRequest();
            }

            var mappedContract = _mapper.Map<ContractFullViewModel>(await _contractService.GetContractAsync(contractID));
            var mappedReports = _mapper.Map<List<MonthReportViewModel>>(await _contractService.GetMonthReportsAsync(contractID));
            mappedContract.MonthReports = mappedReports;
            return Ok(mappedContract);
        }

        [Authorize(IncludeModels.PolicyNavigation.OnlyAdminPolicyName)]
        [HttpGet(nameof(GetAll))]
        public async Task<IActionResult> GetAll()
        {
            return Ok(_mapper.Map<List<ContractViewModel>>(await _contractService.GetAll()));
        }

        [HttpPost]
        public async Task<IActionResult> Post(ContractCreateModel createModel)
        {
            var contractToAdd = _mapper.Map<Contract>(createModel);
            if (!_accountService.IsAdmin(IncludeModels.UserIdentitiesTools.GetUserRoleClaimValue(User)))
            {
                contractToAdd.UserID = IncludeModels.UserIdentitiesTools.GetUserIDClaimValue(User);
            }

            await _contractService.Add(contractToAdd);
            return Ok();
        }

        [HttpPut]
        public async Task<ActionResult> Put(ContractEditModel editModel)
        {
            var newContract = _mapper.Map<Contract>(editModel);
            if (!_accountService.IsAdmin(IncludeModels.UserIdentitiesTools.GetUserRoleClaimValue(User)))
            {
                newContract.UserID = IncludeModels.UserIdentitiesTools.GetUserIDClaimValue(User);
            }

            await _contractService.Edit(newContract);
            return Ok();
        }

        [HttpDelete("{contractID}")]
        public async Task<IActionResult> Delete(int contractID)
        {
            await _contractService.Delete(contractID);
            return Ok(); 
        }

        [HttpPost(nameof(Confirm) + "/{contractID}")]
        [Authorize(IncludeModels.PolicyNavigation.OnlyAdminPolicyName)]
        public async Task<IActionResult> Confirm(int contractID)
        {
            await _contractService.ConfirmContractAsync(contractID, User.Identity?.Name ?? throw new UnauthorizedAccessException());
            return Ok();
        }

        [HttpGet(nameof(GetMonthReports) + "/{contractID}")]
        public async Task<IActionResult> GetMonthReports(int contractID)
        {
            bool isAdmin = _accountService.IsAdmin(IncludeModels.UserIdentitiesTools.GetUserRoleClaimValue(User));
            var currentUserLogin = User.Identity?.Name ?? throw new UnauthorizedAccessException();
            if (!isAdmin && currentUserLogin != await _contractService.GetOwnersLoginAsync(contractID))
            {
                return BadRequest();
            }

            return Ok(_mapper.Map<List<MonthReportViewModel>>(await _contractService.GetMonthReportsAsync(contractID)));
        }

        [HttpGet(nameof(GetMonthReportsUntakenTime) + "/{contractID}")]
        public async Task<IActionResult> GetMonthReportsUntakenTime(int contractID)
        {
            bool isAdmin = _accountService.IsAdmin(IncludeModels.UserIdentitiesTools.GetUserRoleClaimValue(User));
            var currentUserLogin = User.Identity?.Name ?? throw new UnauthorizedAccessException();
            if (!isAdmin && currentUserLogin != await _contractService.GetOwnersLoginAsync(contractID))
            {
                return BadRequest();
            }
        
            return Ok(await _contractService.GetMonthReportsUntakenTimeAsync(contractID, Enumerable.Empty<(int,int,int)>()));
        }

        [HttpPut(nameof(EditMonthReport))]
        public async Task<IActionResult> EditMonthReport(EditMonthReportModel editModel)
        {
            bool isAdmin = _accountService.IsAdmin(IncludeModels.UserIdentitiesTools.GetUserRoleClaimValue(User));
            var currentUserLogin = User.Identity?.Name ?? throw new UnauthorizedAccessException();
            if (!isAdmin && currentUserLogin != await _contractService.GetOwnersLoginAsync(editModel.ContractID))
            {
                return BadRequest();
            }
            
            var monthReportToApply = _mapper.Map<MonthReport>(editModel);
            await _contractService.UpdateMonthReport(monthReportToApply);
            return Ok();
        }

        [HttpGet(nameof(GetMonthReportsReport))]
        public async Task<IActionResult> GetMonthReportsReport(DateTime periodStart, DateTime periodEnd)
        {
            if (periodEnd <= periodStart) return BadRequest("periodEnd <= periodStart");
            var result = await _contractService.GetReportsForReportsOnPeriodAsync(periodStart, periodEnd);
            return Ok(result.Select(r => new { groupIDs = r.relatedContractsIDs, reports = _mapper.Map<List<MonthReportViewModel>>(r.monthReports) }));
        }
    }
}
