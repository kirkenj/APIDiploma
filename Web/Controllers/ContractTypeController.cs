using AutoMapper;
using Database.Entities;
using Logic.Interfaces;
using Logic.Models.ContractType;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebFront.Constants;
using WebFront.Models.ContractType;

namespace Diploma.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContractTypeController : ControllerBase
    {
        private readonly IContractTypeService _academicDegreeService;
        private readonly IMapper _mapper;

        public ContractTypeController(IContractTypeService contractTypeService, IMapper mapper)
        {
            _academicDegreeService = contractTypeService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] ContractTypesSelectObject? selectObject, int? page = default, int? pageSize = default)
        {
            return Ok(_mapper.Map<List<ContractTypeViewModel>>(await _academicDegreeService.GetListViaSelectionObjectAsync(selectObject, page, pageSize)));
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var ret = await _academicDegreeService.FirstOrDefaultAsync(d => d.ID == id);
            return ret is null ? NotFound() : Ok(_mapper.Map<ContractTypeViewModel>(ret));
        }

        [HttpPost]
        [Authorize(IncludeModels.PolicyNavigation.OnlyAdminPolicyName)]
        public async Task<IActionResult> Post([FromBody] string newName)
        {
            await _academicDegreeService.AddAsync(new ContractType { Name = newName });
            return Created(nameof(Post), newName);
        }

        [HttpPut]
        [Authorize(IncludeModels.PolicyNavigation.OnlyAdminPolicyName)]
        public async Task<IActionResult> Put(ContractTypeViewModel academicDegree)
        {
            await _academicDegreeService.UpdateAsync(_mapper.Map<ContractType>(academicDegree), CancellationToken.None);
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(IncludeModels.PolicyNavigation.OnlySuperAdminPolicyName)]
        public async Task<IActionResult> Delete(int id)
        {
            var valueToRemove = await _academicDegreeService.FirstOrDefaultAsync(d => d.ID == id);
            if (valueToRemove == null)
            {
                return BadRequest();
            }

            await _academicDegreeService.DeleteAsync(valueToRemove);
            return Ok();
        }


        #region PriceAssignments
        [HttpGet("PriceAssignment")]
        public async Task<IActionResult> GetPriceAssignations()
        {
            return Ok(await _academicDegreeService.GetAllAssignmentsAsync());
        }

        [HttpGet("PriceAssignment/{id}")]
        public async Task<IActionResult> GetPriceAssignments(int id)
        {
            return Ok(await _academicDegreeService.GetAssignmentsForObject(id));
        }

        [HttpGet("PriceAssignment/{id}/{date}")]
        public async Task<IActionResult> GetPriceAssignment(int id, DateTime date)
        {
            var ret = await _academicDegreeService.GetAssignmentOnDate(date, id);
            return ret is null ? BadRequest("No data found") : Ok(ret);
        }

        [HttpPut("PriceAssignment/{id}")]
        [Authorize(IncludeModels.PolicyNavigation.OnlyAdminPolicyName)]
        public async Task<IActionResult> PutPriceAssignment(int id, DateTime assignationActiveDate, double newValue, DateTime? newAssignationDate)
        {
            await _academicDegreeService.EditAssignmentAsync(id, assignationActiveDate, newValue, newAssignationDate, CancellationToken.None);
            return Ok();
        }

        [HttpPost("PriceAssignment/{id}")]
        [Authorize(IncludeModels.PolicyNavigation.OnlyAdminPolicyName)]
        public async Task<IActionResult> PostPriceAssignment(int id, DateTime assignationActiveDate, double Value, CancellationToken token = default)
        {
            var newAssignation = new ContractTypePriceAssignment { AssignmentDate = assignationActiveDate, Value = Value, ObjectIdentifier = id };
            await _academicDegreeService.AddAssignmentAsync(newAssignation, token);
            return Ok();
        }

        [HttpDelete("PriceAssignment/{id}")]
        [Authorize(IncludeModels.PolicyNavigation.OnlyAdminPolicyName)]
        public async Task<IActionResult> DeletePriceAssignment(int id, DateTime assignationActiveDate, CancellationToken token = default)
        {
            await _academicDegreeService.RemoveAssignmentAsync(id, assignationActiveDate, token);
            return Ok();
        }
        #endregion
    }
}
