using ClosedXML.Excel;
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
        [HttpGet(nameof(GetMonthReportsReportAsExcel))]
        public FileResult GetMonthReportsReportAsExcel(DateTime dateStart, DateTime dateEnd, int? departmentID)
        {
            var path = $"D:\\{Guid.NewGuid()}.xlsx";
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sample Sheet");
                worksheet.Cell("A1").Value = 1;
                worksheet.Cell("A2").Value = 2;
                worksheet.Cell("A3").Value = 3;
                worksheet.Cell("A4").FormulaA1 = "=SUM(A1:A3)";
                workbook.SaveAs(path);
            }

            var bytes = System.IO.File.ReadAllBytes(path);
            System.IO.File.Delete(path);
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "file.xlsx");
        }
    }
}
