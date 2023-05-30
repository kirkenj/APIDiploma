using AutoMapper;
using Database.Entities;
using Logic.Interfaces;
using Logic.Models.AcademicDegree;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebFront.Constants;
using WebFront.Models.AcademicDegrees;

namespace WebFront.Controllers
{
    public class AcademicDegreeController : Controller
    {
        private readonly IAcademicDegreeService _academicDegreeService;
        private readonly IMapper _mapper;

        public AcademicDegreeController(IAcademicDegreeService departmentService, IMapper mapper)
        {
            _academicDegreeService = departmentService;
            _mapper = mapper;
        }

        [HttpGet("{controller}/" + nameof(List))]
        public async Task<IActionResult> List([FromQuery] AcademicDegreeSelectObject? selectObject, int page = 1, int pageSize = 10)
        {
            ViewBag.selectionObject = selectObject;
            ViewBag.page = page;
            ViewBag.pageSize = pageSize;
            return View("List", _mapper.Map<List<AcademicDegreeViewModel>>(await _academicDegreeService.GetListViaSelectionObjectAsync(selectObject, page, pageSize)));
        }

        [HttpGet("{controller}/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var ret = await _academicDegreeService.FirstOrDefaultAsync(d => d.ID == id);
            return ret is null ? NotFound() : View("Get", _mapper.Map<AcademicDegreeViewModel>(ret));
        }

        [HttpGet("{controller}/Add")]
        [Authorize(IncludeModels.PolicyNavigation.OnlyAdminPolicyName)]
        public IActionResult GetAddForm()
        {
            return View("Add");
        }

        [HttpPost("{controller}/Add")]
        [Authorize(IncludeModels.PolicyNavigation.OnlyAdminPolicyName)]
        public async Task<IActionResult> Post(string newObjectName)
        {
            await _academicDegreeService.AddAsync(new AcademicDegree { Name = newObjectName });
            return Redirect(nameof(List));
        }

        [Authorize(IncludeModels.PolicyNavigation.OnlyAdminPolicyName)]
        [HttpGet("{controller}/Edit/{id}")]
        public async Task<IActionResult> GetEditForm(int id)
        {
            var dep = await _academicDegreeService.FirstOrDefaultAsync(d => d.ID == id);
            if (dep == null)
            {
                return NotFound();
            }

            return View("Edit", _mapper.Map<AcademicDegreeViewModel>(dep));
        }

        [HttpPost("{controller}/Edit")]
        [Authorize(IncludeModels.PolicyNavigation.OnlyAdminPolicyName)]
        public async Task<IActionResult> Put(AcademicDegreeViewModel academicDegree)
        {
            await _academicDegreeService.UpdateAsync(_mapper.Map<AcademicDegree>(academicDegree), CancellationToken.None);
            return Redirect(nameof(List));
        }

        [HttpPost("{controller}/" + nameof(Delete))]
        [Authorize(IncludeModels.PolicyNavigation.OnlySuperAdminPolicyName)]
        public async Task<IActionResult> Delete(int id)
        {
            var valueToRemove = await _academicDegreeService.FirstOrDefaultAsync(d => d.ID == id);
            if (valueToRemove == null)
            {
                return BadRequest(IncludeModels.BadRequestTextFactory.GetObjectNotFoundExceptionText($"Id = {id}"));
            }

            await _academicDegreeService.DeleteAsync(valueToRemove);
            return Redirect(nameof(List));
        }


        #region PriceAssignments
        [HttpGet("{controller}/Assignments")]
        public async Task<IActionResult> GetPriceAssignments()
        {
            var ret = await _academicDegreeService.GetAllAssignmentsAsync();
            var atachedObjects = await _academicDegreeService.GetListViaSelectionObjectAsync(new() { IDs = ret.Select(r => r.ObjectIdentifier) });
            ViewBag.AtachedObjects = atachedObjects;
            ViewBag.Message = "Выборка для всех видов";
            return View("Assigments", ret);
        }

        [HttpGet("{controller}/Assignments/{id}")]
        public async Task<IActionResult> GetPriceAssignments(int id)
        {
            var ret = await _academicDegreeService.GetAssignmentsForObject(id);
            var atachedObjects = await _academicDegreeService.GetListViaSelectionObjectAsync(new() { IDs = ret.Select(r => r.ObjectIdentifier) });
            ViewBag.AtachedObjects = atachedObjects;
            ViewBag.Message = $"Выборка для {(await _academicDegreeService.FirstOrDefaultAsync(a => a.ID == id))?.Name ?? "Unknown"}";
            return View("Assigments", ret);
        }

        //[HttpGet("{id}/PriceAssignment/{date}")]
        //public async Task<IActionResult> GetPriceAssignment(int id, DateTime date)
        //{
        //    var ret = await _academicDegreeService.GetAssignmentOnDate(date, id);
        //    return ret is null ? BadRequest("No data found") : Ok(ret);
        //}

        [HttpGet("{controller}/" + nameof(EditAssignment))]
        [Authorize(IncludeModels.PolicyNavigation.OnlyAdminPolicyName)]
        public async Task<IActionResult> GetEditAssignmentForm(int id, DateTime assignmentActiveDate)
        {
            var ret = await _academicDegreeService.GetAssignmentOnDate(assignmentActiveDate, id);
            if (ret == null)
            {
                return NotFound();
            }

            return View("AssignmentEdit", ret);
        }

        [HttpPost("{controller}/" + nameof(EditAssignment))]
        [Authorize(IncludeModels.PolicyNavigation.OnlyAdminPolicyName)]
        public async Task<IActionResult> EditAssignment(int id, DateTime assignationActiveDate, double newValue, DateTime? newAssignationDate)
        {
            await _academicDegreeService.EditAssignmentAsync(id, assignationActiveDate, newValue, newAssignationDate, CancellationToken.None);
            return RedirectToAction("Assignments", new { id });
        }

        [HttpGet("{controller}/" + nameof(AddAssignment))]
        [Authorize(IncludeModels.PolicyNavigation.OnlyAdminPolicyName)]
        public async Task<IActionResult> GetAddAssignmentForm(int? forID)
        {
            ViewBag.forID = forID;
            var atachedObjects = await _academicDegreeService.GetListViaSelectionObjectAsync(null);
            ViewBag.AtachedObjects = atachedObjects;
            return View("AssignmentAdd");
        }

        [HttpPost("{controller}/" + nameof(AddAssignment))]
        [Authorize(IncludeModels.PolicyNavigation.OnlyAdminPolicyName)]
        public async Task<IActionResult> AddAssignment(int id, DateTime assignmentDate, double Value, CancellationToken token = default)
        {
            var newAssignation = new AcademicDegreePriceAssignment { AssignmentDate = assignmentDate, Value = Value, ObjectIdentifier = id };
            await _academicDegreeService.AddAssignmentAsync(newAssignation, token);
            return RedirectToAction("Assignments");
        }

        [HttpPost("{controller}/" + nameof(DeleteAssignment))]
        [Authorize(IncludeModels.PolicyNavigation.OnlySuperAdminPolicyName)]
        public async Task<IActionResult> DeleteAssignment(int id, DateTime assignmentActiveDate, CancellationToken token = default)
        {
            await _academicDegreeService.RemoveAssignmentAsync(id, assignmentActiveDate, token);
            return RedirectToAction("Assignments");
        }
        #endregion
    }
}
