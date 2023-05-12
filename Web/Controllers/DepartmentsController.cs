using AutoMapper;
using Database.Entities;
using Logic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Web.Constants;
using Web.Models.Departments;
using Web.RequestModels.Account;

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
        public async Task<IActionResult> Get()
        {
            return Ok(_mapper.Map<List<DepartmentViewModel>>(await _departmentService.GetAllAsync()));
        }

        [HttpPost]
        [Authorize(IncludeModels.PolicyNavigation.OnlyAdminPolicyName)]
        public async Task<IActionResult> Post([FromBody] string newDepartmentsName)
        {
            await _departmentService.AddAsync(new Department { Name = newDepartmentsName });
            return Created(nameof(Post),newDepartmentsName);
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
            var valueToRemove = await _departmentService.FirstOrDefaultAsync(d => d.ID ==  id);
            if (valueToRemove == null)
            {
                return BadRequest();
            }

            await _departmentService.DeleteAsync(valueToRemove);
            return Ok();
        }

        //[HttpGet(nameof(GetMonthReportsReportAsExcel))]
        //public FileResult GetMonthReportsReportAsExcel()
        //{
        //    var result = _contractService.GetReportsForReportsOnPeriodInExcelAsync(DateTime.MinValue, DateTime.MaxValue);
        //    var bytes = System.IO.File.ReadAllBytes(result);
        //    System.IO.File.Delete(result);
        //    return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "file.xlsx");
        //}
    }
}
