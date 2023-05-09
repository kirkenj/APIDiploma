using Database.Entities;
using Logic.Interfaces;
using Logic.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using Web.Constants;

namespace Diploma.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;
        private readonly IContractService _contractService;

        public DepartmentsController(IDepartmentService departmentService, IContractService contractService)
        {
            _departmentService = departmentService;
            _contractService = contractService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Ok(await _departmentService.GetAllAsync());
        }

        [HttpPost]
        [Authorize(IncludeModels.PolicyNavigation.OnlyAdminPolicyName)]
        public async Task<IActionResult> Post([FromBody] string newDepartmentsName)
        {
            await _departmentService.AddAsync(new Department { Name = newDepartmentsName });
            return Created(nameof(Post),newDepartmentsName);
        }

        [HttpPut("{id}")]
        [Authorize(IncludeModels.PolicyNavigation.OnlyAdminPolicyName)]
        public async Task<IActionResult> Put(Department department)
        {
            await _departmentService.UpdateAsync(department, CancellationToken.None);
            return Ok();
        }

        [HttpDelete("{id}")]
        [Authorize(IncludeModels.PolicyNavigation.OnlySuperAdminPolicyName)]
        public async Task<IActionResult> Delete(int id)
        {
            var valueToRemove = await _departmentService.FirstOrDefaultAsync(d => d.ID ==  id);
            if (valueToRemove == null)
            {
                return BadRequest();
            }

            await _departmentService.DeleteAsync(valueToRemove);
            return Ok();
        }

        [HttpGet(nameof(GetMonthReportsReportAsExcel))]
        public FileResult GetMonthReportsReportAsExcel()
        {
            var result = _contractService.GetReportsForReportsOnPeriodInExcelAsync(DateTime.MinValue, DateTime.MaxValue);
            var bytes = System.IO.File.ReadAllBytes(result);
            System.IO.File.Delete(result);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "file.xlsx");
        }
    }
}
