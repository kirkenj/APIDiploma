using Logic.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebFront.Constants;

namespace WebFront.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    [Authorize(IncludeModels.PolicyNavigation.OnlyAdminPolicyName)]
    public class ExcelReportController : ControllerBase
    {
        private readonly IExcelReportService _excelReportService;

        public ExcelReportController(IExcelReportService excelReportService)
        {
            _excelReportService = excelReportService;
        }


        [HttpGet(nameof(GetMonthReportsReportAsExcel))]
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
