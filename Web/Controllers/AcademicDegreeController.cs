﻿using AutoMapper;
using Database.Entities;
using Logic.Interfaces;
using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> Get()
        {
            return Ok(_mapper.Map<List<AcademicDegreeViewModel>>(await _academicDegreeService.GetAllAsync()));
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

        [HttpGet("PriceAssignations")]
        //[Authorize(IncludeModels.PolicyNavigation.OnlySuperAdminPolicyName)]
        public async Task<IActionResult> GetPriceAssignations()
        {
            return Ok(await _academicDegreeService.GetAllAssignationsAsync());
        }

        [HttpGet("{id}/PriceAssignations")]
        //[Authorize(IncludeModels.PolicyNavigation.OnlySuperAdminPolicyName)]
        public async Task<IActionResult> GetPriceAssignations(int id)
        {
            return Ok(await _academicDegreeService.GetAssignationsForObject(id));
        }

        [HttpGet("{id}/PriceAssignation/{date}")]
        //[Authorize(IncludeModels.PolicyNavigation.OnlySuperAdminPolicyName)]
        public async Task<IActionResult> GetPriceAssignation(int id, DateTime date)
        {
            var ret = await _academicDegreeService.GetAssignationOnDate(date, id);
            return ret is null ? BadRequest("No data found") : Ok(ret);
        }

        [HttpPut("{id}/PriceAssignation")]
        //[Authorize(IncludeModels.PolicyNavigation.OnlySuperAdminPolicyName)]
        public async Task<IActionResult> PutPriceAssignation(int id, DateTime assignationActiveDate, double newValue, DateTime? newAssignationDate )
        {
            await _academicDegreeService.EditAssignationAsync(id, assignationActiveDate, newValue, newAssignationDate, CancellationToken.None);
            return Ok();
        }

        [HttpPost("{id}/PriceAssignation")]
        //[Authorize(IncludeModels.PolicyNavigation.OnlySuperAdminPolicyName)]
        public async Task<IActionResult> PostPriceAssignation(int id, DateTime assignationActiveDate, double Value, CancellationToken token = default)
        {
            var newAssignation = new AcademicDegreePriceAssignation { AssignationDate = assignationActiveDate, Value = Value, ObjectIdentifier = id };
            await _academicDegreeService.AddAssignationAsync(newAssignation, token);
            return Ok();
        }

        [HttpDelete("{id}/PriceAssignation")]
        public async Task<IActionResult> DeltePriceAssignation(int id, DateTime assignationActiveDate, CancellationToken token = default)
        {
            await _academicDegreeService.RemoveAssignationAsync(id, assignationActiveDate, token);
            return Ok();
        }
        //2023-05-12 02:10:59
    }
}
