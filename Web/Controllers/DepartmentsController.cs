using Database.Entities;
using Logic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Diploma.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentsController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        // GET: api/<ValuesController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Department>>> Get()
        {
            return await _departmentService.GetAllAsync();
        }

        // POST api/<ValuesController>
        [HttpPost]
        [Authorize("OnlyAdmin")]
        public async Task<IActionResult> Post([FromBody] string value)
        {
            if (await _departmentService.FindByNameAsync(value) != null)
            {
                return BadRequest();
            }

            await _departmentService.Create(new Department { Name = value });
            return Created(nameof(Post),value);
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        [Authorize("OnlyAdmin")]
        public async Task<IActionResult> Put(int id, [FromBody] string value)
        {
            if (!(await _departmentService.TryEditAsync(id, value)))
            {
                return BadRequest();
            }

            return Ok();
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        [Authorize("OnlySuperAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _departmentService.DeleteAsync(id);
            return Ok();
        }
    }
}
