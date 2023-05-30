using AutoMapper;
using Database.Entities;
using Logic.Interfaces;
using Logic.Models.ContractType;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebFront.Constants;
using WebFront.Models.ContractType;

namespace WebFront.Controllers
{
    public class ContractTypeController : Controller
    {
        private readonly IContractTypeService _contractTypeService;
        private readonly IMapper _mapper;

        public ContractTypeController(IContractTypeService contractTypeService, IMapper mapper)
        {
            _contractTypeService = contractTypeService;
            _mapper = mapper;
        }

        [HttpGet("{controller}/" + nameof(List))]
        public async Task<IActionResult> List([FromQuery] ContractTypesSelectObject? selectObject, int page = 1, int pageSize = 10)
        {
            var res = await _contractTypeService.GetListViaSelectionObjectAsync(selectObject, page, pageSize);
            var toRet = _mapper.Map<List<ContractTypeViewModel>>(res);

            ViewBag.selectionObject = selectObject;
            ViewBag.page = page;
            ViewBag.pageSize = pageSize;
            return View("List", toRet);
        }

        [HttpGet("{controller}/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var res = await _contractTypeService.FirstOrDefaultAsync(d => d.ID == id);
            if (res == null)
            {
                return NotFound();
            }

            var toRet = _mapper.Map<ContractTypeViewModel>(res);
            return View("Get", toRet);
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
            await _contractTypeService.AddAsync(new ContractType { Name = newObjectName });
            return RedirectToAction(nameof(List));
        }

        [Authorize(IncludeModels.PolicyNavigation.OnlyAdminPolicyName)]
        [HttpGet("{controller}/Edit/{id}")]
        public async Task<IActionResult> GetEditForm(int id)
        {
            var dep = await _contractTypeService.FirstOrDefaultAsync(d => d.ID == id);
            if (dep == null)
            {
                return NotFound();
            }

            return View("Edit", _mapper.Map<ContractTypeViewModel>(dep));
        }

        [HttpPost("{controller}/Edit")]
        [Authorize(IncludeModels.PolicyNavigation.OnlyAdminPolicyName)]
        public async Task<IActionResult> Put(ContractTypeViewModel academicDegree)
        {
            await _contractTypeService.UpdateAsync(_mapper.Map<ContractType>(academicDegree), CancellationToken.None);
            return Redirect(nameof(List));
        }

        [HttpPost("{controller}/" + nameof(Delete))]
        [Authorize(IncludeModels.PolicyNavigation.OnlySuperAdminPolicyName)]
        public async Task<IActionResult> Delete(int id)
        {
            var valueToRemove = await _contractTypeService.FirstOrDefaultAsync(d => d.ID == id);
            if (valueToRemove == null)
            {
                return BadRequest();
            }

            await _contractTypeService.DeleteAsync(valueToRemove);
            return Redirect(nameof(List));
        }

        #region PriceAssignments
        [HttpGet("{controller}/Assignments")]
        public async Task<IActionResult> GetPriceAssignments()
        {
            var ret = await _contractTypeService.GetAllAssignmentsAsync();
            var atachedObjects = await _contractTypeService.GetListViaSelectionObjectAsync(new() { IDs = ret.Select(r => r.ObjectIdentifier) });
            ViewBag.AtachedObjects = atachedObjects;
            ViewBag.Message = "Выборка для всех видов";
            return View("Assigments", ret);
        }

        [HttpGet("{controller}/Assignments/{id}")]
        public async Task<IActionResult> GetPriceAssignments(int id)
        {
            var ret = await _contractTypeService.GetAssignmentsForObject(id);
            var atachedObjects = await _contractTypeService.GetListViaSelectionObjectAsync(new ContractTypesSelectObject { IDs = ret.Select(r => r.ObjectIdentifier) });
            ViewBag.AtachedObjects = atachedObjects;
            ViewBag.Message = $"Выборка для {(await _contractTypeService.FirstOrDefaultAsync(a => a.ID == id))?.Name ?? "Unknown"}";
            return View("Assigments", ret);
        }

        //[HttpGet("{controller}/Assignment/{id}")]
        //public async Task<IActionResult> GetPriceAssignment(int id, DateTime date)
        //{
        //    var ret = await _contractTypeService.GetAssignmentOnDate(date, id);
        //    return ret is null ? BadRequest("No data found") : Ok(ret);
        //}

        [HttpGet("{controller}/" + nameof(EditAssignment))]
        [Authorize(IncludeModels.PolicyNavigation.OnlyAdminPolicyName)]
        public async Task<IActionResult> GetEditAssignmentForm(int id, DateTime assignmentActiveDate)
        {
            var ret = await _contractTypeService.GetAssignmentOnDate(assignmentActiveDate, id);
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
            await _contractTypeService.EditAssignmentAsync(id, assignationActiveDate, newValue, newAssignationDate, CancellationToken.None);
            return RedirectToAction("Assignments", new { id });
        }

        [HttpGet("{controller}/" + nameof(AddAssignment))]
        [Authorize(IncludeModels.PolicyNavigation.OnlyAdminPolicyName)]
        public async Task<IActionResult> GetAddAssignmentForm(int? forID)
        {
            ViewBag.forID = forID;
            var atachedObjects = await _contractTypeService.GetListViaSelectionObjectAsync(null);
            ViewBag.AtachedObjects = atachedObjects;
            return View("AssignmentAdd");
        }

        [HttpPost("{controller}/" + nameof(AddAssignment))]
        [Authorize(IncludeModels.PolicyNavigation.OnlyAdminPolicyName)]
        public async Task<IActionResult> AddAssignment(int id, DateTime assignmentDate, double Value, CancellationToken token = default)
        {
            var newAssignation = new ContractTypePriceAssignment { AssignmentDate = assignmentDate, Value = Value, ObjectIdentifier = id };
            await _contractTypeService.AddAssignmentAsync(newAssignation, token);
            return RedirectToAction("Assignments", new { id });
        }

        [HttpPost("{controller}/" + nameof(DeleteAssignment))]
        [Authorize(IncludeModels.PolicyNavigation.OnlyAdminPolicyName)]
        public async Task<IActionResult> DeleteAssignment(int id, DateTime assignmentActiveDate, CancellationToken token = default)
        {
            await _contractTypeService.RemoveAssignmentAsync(id, assignmentActiveDate, token);
            return RedirectToAction("Assignments");
        }
        #endregion
    }
}
