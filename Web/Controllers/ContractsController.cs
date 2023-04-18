using Logic.Interfaces;
using Database.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Logic.RequestModels.Contracts;
using AutoMapper;

namespace DiplomaAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ContractsController : ControllerBase
    {
        private readonly IContractService _contractService;
        private readonly IMapper _mapper;

        public ContractsController(IDepartmentService departmentService, IMapper mapper, IAccountService accountService, IContractService contractService, IMonthReportService monthReportService)
        {
            _contractService = contractService;
            _mapper = mapper;
        }

        [HttpGet]
        public IActionResult Get() 
        {
            string? userName = User.Identity?.Name;
            if (userName == null)
            {
                return BadRequest();
            }

            return Ok(_contractService.GetUserContracts(userName));
        }

        [HttpPost]
        public IActionResult Post(ContractCreateModel createModel)
        {
            var contract = _mapper.Map<Contract>(createModel);


            return NotFound();
        }
    }
}
