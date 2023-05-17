using Database.Entities;
using Logic.Interfaces.Common;
using Logic.Models.MonthReports;

namespace Logic.Interfaces
{
    public interface IContractLinkingPartService : IDbAccessServise<ContractLinkingPart>
    {
        public Task<IEnumerable<Contract>> GetRelatedContractsAsync(int linkingPartID, CancellationToken token = default);
        public Task BlockReport(int linkingPartID, int month, int year, int userID, CancellationToken token = default);
        public Task<IEnumerable<RelatedContractsWithReportsObject>> GetReportsOnPeriodAsync(DateTime periodStart, DateTime periodEnd);
        public Task<MonthReportsUntakenTimeModel> GetUntakenTimeAsync(int v, DateTime date, IEnumerable<(int year, int month)> exceptValuesWithKeys);
        public Task OnContractRemovedAsync(Contract contract);
        public Task UpdateMonthReport(MonthReport monthReportToAply);
        Task<RelatedContractsWithReportsObject> GetFullData(int linkingPartID);
    }
}
