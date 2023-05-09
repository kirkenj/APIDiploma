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
        public IActionResult Get() 
        {
            int userID = IncludeModels.UserIdentitiesTools.GetUserIDClaimValue(User);
            return Ok(_mapper.Map<List<ContractViewModel>>(_contractService.GetRange(c => c.UserID == userID)));
        }
       
        [HttpGet("{contractID}")]
        public async Task<IActionResult> Get(int contractID) 
        {
            bool isAdmin = _roleService.IsAdminRoleName(IncludeModels.UserIdentitiesTools.GetUserRoleClaimValue(User));
            var currentUserLogin = User.Identity?.Name ?? throw new UnauthorizedAccessException();
            if (!isAdmin && currentUserLogin != await _contractService.GetOwnersLoginAsync(contractID))
            {
                return BadRequest();
            }

            var contractToReturn = await _contractService.FirstOrDefaultAsync(c => c.ID == contractID);
            if (contractToReturn is null)
            {
                return BadRequest();
            }

            var mappedContract = _mapper.Map<ContractFullViewModel>(contractToReturn);
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
            var valueToRemove = await _contractService.FirstOrDefaultAsync(c => c.ID ==  contractID);
            if (valueToRemove is null)
            {
                return BadRequest();
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
                return BadRequest();
            }

            return Ok(_mapper.Map<List<MonthReportViewModel>>(await _contractService.GetMonthReportsAsync(contractID)));
        }

        [HttpGet(nameof(GetMonthReportsUntakenTime) + "/{contractID}")]
        public async Task<IActionResult> GetMonthReportsUntakenTime(int contractID)
        {
            bool isAdmin = _roleService.IsAdminRoleName(IncludeModels.UserIdentitiesTools.GetUserRoleClaimValue(User));
            var currentUserLogin = User.Identity?.Name ?? throw new UnauthorizedAccessException();
            if (!isAdmin && currentUserLogin != await _contractService.GetOwnersLoginAsync(contractID))
            {
                return BadRequest();
            }
        
            return Ok(await _contractService.GetUntakenTimeAsync(contractID, Enumerable.Empty<(int,int,int)>()));
        }

        [HttpPut(nameof(EditMonthReport))]
        public async Task<IActionResult> EditMonthReport(EditMonthReportModel editModel)
        {
            bool isAdmin = _roleService.IsAdminRoleName(IncludeModels.UserIdentitiesTools.GetUserRoleClaimValue(User));
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
        [Authorize(IncludeModels.PolicyNavigation.OnlyAdminPolicyName)]
        public async Task<IActionResult> GetMonthReportsReport(DateTime periodStart, DateTime periodEnd)
        {
            if (periodEnd <= periodStart) return BadRequest("periodEnd <= periodStart");
            var result = await _contractService.GetReportsForReportsOnPeriodAsync(periodStart, periodEnd);
            return Ok(result.Select(r => new { groupIDs = r.relatedContractsIDs, reports = _mapper.Map<List<MonthReportViewModel>>(r.monthReports) }));
        }

        
    }
}
