using AutoMapper;
using Database.Entities;
using Logic.Interfaces;
using Logic.Models.Department;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebFront.Constants;
using WebFront.Models.Departments;

namespace WebFront.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IDepartmentService _departmentService;

        public DepartmentsController(IDepartmentService departmentService, IMapper mapper)
        {
            _departmentService = departmentService;
            _mapper = mapper;
        }


        [HttpGet("{controller}/" + nameof(List))]
        public async Task<IActionResult> List([FromQuery] DepartmentSelectObject? selectObject, int page = 1, int pageSize = 10)
        {
            var res = (await _departmentService.GetListViaSelectionObjectAsync(selectObject, page, pageSize)).ToList();
            var toRet = _mapper.Map<List<DepartmentViewModel>>(res);

            ViewBag.selectionObject = selectObject;
            ViewBag.page = page;
            ViewBag.pageSize = pageSize;
            return View("List", toRet);
        }

        [HttpGet("{controller}/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var res = await _departmentService.FirstOrDefaultAsync(d => d.ID == id);
            if (res == null)
            {
                return NotFound();
            }

            var toRet = _mapper.Map<DepartmentViewModel>(res);
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
            await _departmentService.AddAsync(new Department { Name = newObjectName });
            return RedirectToAction(nameof(List));
        }

        [Authorize(IncludeModels.PolicyNavigation.OnlyAdminPolicyName)]
        [HttpGet("{controller}/Edit/{id}")]
        public async Task<IActionResult> GetEditForm(int id)
        {
            var dep = await _departmentService.FirstOrDefaultAsync(d => d.ID == id);
            if (dep == null)
            {
                return NotFound();
            }

            return View("Edit", _mapper.Map<DepartmentViewModel>(dep));
        }

        [HttpPost("{controller}/Edit")]
        [Authorize(IncludeModels.PolicyNavigation.OnlyAdminPolicyName)]
        public async Task<IActionResult> Put(DepartmentViewModel department)
        {
            await _departmentService.UpdateAsync(_mapper.Map<Department>(department), CancellationToken.None);
            return Redirect(nameof(List));
        }

        [HttpPost("{controller}/" + nameof(Delete))]
        [Authorize(IncludeModels.PolicyNavigation.OnlySuperAdminPolicyName)]
        public async Task<IActionResult> Delete(int id)
        {
            var valueToRemove = await _departmentService.FirstOrDefaultAsync(d => d.ID == id);
            if (valueToRemove == null)
            {
                return BadRequest(IncludeModels.BadRequestTextFactory.GetObjectNotFoundExceptionText($"Id = {id}"));
            }

            await _departmentService.DeleteAsync(valueToRemove);
            return Redirect(nameof(List));
        }
    }
}
