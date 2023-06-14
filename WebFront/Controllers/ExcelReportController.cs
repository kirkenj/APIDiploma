using AutoMapper;
using Logic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebFront.Constants;
using WebFront.Models.Departments;

namespace WebFront.Controllers
{


    [Authorize(IncludeModels.PolicyNavigation.OnlyAdminPolicyName)]
    public class ExcelReportController : Controller
    {
        private readonly IExcelReportService _excelReportService;
        private readonly IDepartmentService _departmentService;
        private readonly IMapper _mapper;

        public ExcelReportController(IExcelReportService excelReportService, IDepartmentService departmentService, IMapper mapper)
        {
            _departmentService = departmentService;
            _excelReportService = excelReportService;
            _mapper = mapper;
        }

        [HttpGet("{controller}/MonthReports")]
        public async Task<IActionResult> GetMonthReportForm()
        {
            var ret = await _departmentService.GetListViaSelectionObjectAsync(null, 1, int.MaxValue);
            ViewBag.Departments = _mapper.Map<IEnumerable<DepartmentViewModel>>(ret);
            return View("MonthReports");
        }


        [HttpGet("{controller}/MonthReportsExcel")]
        public async Task<IActionResult> GetMonthReportsReportAsExcel(DateTime dateStart, DateTime dateEnd, IEnumerable<int>? departmentIDs = null)
        {
            if (dateStart == dateEnd)
            {
                return BadRequest("dateStart == dateEnd");
            }

            if (dateEnd < dateStart)
            {
                (dateEnd, dateStart) = (dateStart, dateEnd);
            }

            var path = await _excelReportService.GetReport(dateStart, dateEnd, departmentIDs);
            var bytes = System.IO.File.ReadAllBytes(path);
            System.IO.File.Delete(path);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "file.xlsx");
        }
    }
}
