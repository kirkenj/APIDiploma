using Database.Entities;
using Logic.Models.MonthReports;

namespace Logic.Interfaces
{
    public interface IContractService : IDbAccessServise<Contract>
    {
        public Task<IEnumerable<Contract>> GetAll();
        public Task ConfirmContractAsync(int contractID, string adminLogin, CancellationToken token = default);
        public Task<IEnumerable<MonthReport>> GetMonthReportsAsync(int contractID);
        public Task UpdateMonthReport(MonthReport monthReport);
        public Task<string?> GetOwnersLoginAsync(int contractID);
        public Task<MonthReportsUntakenTimeModel> GetUntakenTimeAsync(int contractID, IEnumerable<(int contractID,int year,int month)> exceptValuesWithKeys); 
        public Task<IEnumerable<(List<KeyValuePair<int, string>> relatedContractsIDs, List<MonthReport> monthReports)>> GetReportsForReportsOnPeriodAsync(DateTime periodStart, DateTime periodEnd);
        public string GetReportsForReportsOnPeriodInExcelAsync(DateTime periodStart, DateTime periodEnd);
    }
}
