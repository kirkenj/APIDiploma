using AutoMapper;
using Database.Entities;
using Logic.Interfaces;
using Logic.Models.Department;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebFront.Constants;
using WebFront.Models.Departments;

namespace Diploma.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IDepartmentService _departmentService;

        public DepartmentsController(IDepartmentService departmentService, IMapper mapper)
        {
            _departmentService = departmentService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetOne([FromQuery] DepartmentSelectObject? selectObject, int? page = default, int? pageSize = default)
        {
            return Ok(_mapper.Map<List<DepartmentViewModel>>(await _departmentService.GetListViaSelectionObjectAsync(selectObject, page, pageSize)));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var res = await _departmentService.FirstOrDefaultAsync(d => d.ID == id);
            if (res == null)
            {
                return NotFound();
            }

            return Ok(_mapper.Map<DepartmentViewModel>(res));
        }

        [HttpPost]
        [Authorize(IncludeModels.PolicyNavigation.OnlyAdminPolicyName)]
        public async Task<IActionResult> Post([FromBody] string newDepartmentsName)
        {
            await _departmentService.AddAsync(new Department { Name = newDepartmentsName });
            return Created(nameof(Post), newDepartmentsName);
        }

        [HttpPut]
        [Authorize(IncludeModels.PolicyNavigation.OnlyAdminPolicyName)]
        public async Task<IActionResult> Put(DepartmentViewModel department)
        {
            await _departmentService.UpdateAsync(_mapper.Map<Department>(department), CancellationToken.None);
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(IncludeModels.PolicyNavigation.OnlySuperAdminPolicyName)]
        public async Task<IActionResult> Delete(int id)
        {
            var valueToRemove = await _departmentService.FirstOrDefaultAsync(d => d.ID == id);
            if (valueToRemove == null)
            {
                return BadRequest(IncludeModels.BadRequestTextFactory.GetObjectNotFoundExceptionText($"Id = {id}"));
            }

            await _departmentService.DeleteAsync(valueToRemove);
            return Ok();
        }
    }
}
