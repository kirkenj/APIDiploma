using Database.Entities;

namespace Logic.Interfaces
{
    public interface IMonthReportService
    {
        public Task<List<MonthReport>> GetMonthReportAsyncOnDate(DateTime date);
    }
}
