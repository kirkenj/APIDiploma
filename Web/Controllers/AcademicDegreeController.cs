using AutoMapper;
using Database.Entities;
using Logic.Interfaces;
using Logic.Models.AcademicDegree;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web.Models.AcademicDegrees;

namespace Diploma.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AcademicDegreeController : ControllerBase
    {
        private readonly IAcademicDegreeService _academicDegreeService;
        private readonly IMapper _mapper;

        public AcademicDegreeController(IAcademicDegreeService departmentService, IMapper mapper)
        {
            _academicDegreeService = departmentService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] AcademicDegreeSelectObject? selectObject, int? page = default, int? pageSize = default)
        {
            return Ok(_mapper.Map<List<AcademicDegreeViewModel>>(await _academicDegreeService.GetListViaSelectionObjectAsync(selectObject, page, pageSize)));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var ret = await _academicDegreeService.FirstOrDefaultAsync(d => d.ID == id);
            return ret is null? NotFound() : Ok(_mapper.Map<AcademicDegreeViewModel>(ret));
        }

        [HttpPost]
        //[Authorize(IncludeModels.PolicyNavigation.OnlyAdminPolicyName)]
        public async Task<IActionResult> Post([FromBody] string newDegreeName)
        {
            await _academicDegreeService.AddAsync(new AcademicDegree { Name = newDegreeName });
            return Created(nameof(Post), newDegreeName);
        }

        [HttpPut]
        //[Authorize(IncludeModels.PolicyNavigation.OnlyAdminPolicyName)]
        public async Task<IActionResult> Put(AcademicDegreeViewModel academicDegree)
        {
            await _academicDegreeService.UpdateAsync(_mapper.Map<AcademicDegree>(academicDegree), CancellationToken.None);
            return Ok();
        }

        [HttpDelete("{id}")]
        //[Authorize(IncludeModels.PolicyNavigation.OnlySuperAdminPolicyName)]
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
        [HttpGet("PriceAssignments")]
        //[Authorize(IncludeModels.PolicyNavigation.OnlySuperAdminPolicyName)]
        public async Task<IActionResult> GetPriceAssignations()
        {
            return Ok(await _academicDegreeService.GetAllAssignmentsAsync());
        }

        [HttpGet("{id}/PriceAssignments")]
        //[Authorize(IncludeModels.PolicyNavigation.OnlySuperAdminPolicyName)]
        public async Task<IActionResult> GetPriceAssignments(int id)
        {
            return Ok(await _academicDegreeService.GetAssignmentsForObject(id));
        }

        [HttpGet("{id}/PriceAssignment/{date}")]
        //[Authorize(IncludeModels.PolicyNavigation.OnlySuperAdminPolicyName)]
        public async Task<IActionResult> GetPriceAssignment(int id, DateTime date)
        {
            var ret = await _academicDegreeService.GetAssignmentOnDate(date, id);
            return ret is null ? BadRequest("No data found") : Ok(ret);
        }

        [HttpPut("{id}/PriceAssignment")]
        //[Authorize(IncludeModels.PolicyNavigation.OnlySuperAdminPolicyName)]
        public async Task<IActionResult> PutPriceAssignment(int id, DateTime assignationActiveDate, double newValue, DateTime? newAssignationDate )
        {
            await _academicDegreeService.EditAssignmentAsync(id, assignationActiveDate, newValue, newAssignationDate, CancellationToken.None);
            return Ok();
        }

        [HttpPost("{id}/PriceAssignment")]
        //[Authorize(IncludeModels.PolicyNavigation.OnlySuperAdminPolicyName)]
        public async Task<IActionResult> PostPriceAssignment(int id, DateTime assignationActiveDate, double Value, CancellationToken token = default)
        {
            var newAssignation = new AcademicDegreePriceAssignation { AssignmentDate = assignationActiveDate, Value = Value, ObjectIdentifier = id };
            await _academicDegreeService.AddAssignmentAsync(newAssignation, token);
            return Ok();
        }

        [HttpDelete("{id}/PriceAssignment")]
        public async Task<IActionResult> DeltePriceAssignment(int id, DateTime assignationActiveDate, CancellationToken token = default)
        {
            await _academicDegreeService.RemoveAssignmentAsync(id, assignationActiveDate, token);
            return Ok();
        }
        #endregion
    }
}
