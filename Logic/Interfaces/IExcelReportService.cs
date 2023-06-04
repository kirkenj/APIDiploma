namespace Logic.Interfaces
{
    public interface IExcelReportService
    {
        public Task<string> GetReport(DateTime dateStart, DateTime dateEnd, IEnumerable<int>? reqDepartmentIDs = null);
    }
}
