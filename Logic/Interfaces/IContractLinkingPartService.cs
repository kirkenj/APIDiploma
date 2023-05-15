using Database.Entities;
using Logic.Interfaces.Common;

namespace Logic.Interfaces
{
    public interface IContractLinkingPartService : IDbAccessServise<ContractLinkingPart>
    {
        public Task<IEnumerable<Contract>> GetRelatedContractsAsync(int linkingPartID, CancellationToken token = default);
        public Task BlockReport(int linkingPartID, int year, int month, int userID, CancellationToken token = default);
        public Task<IEnumerable<(List<KeyValuePair<int, string>> relatedContractsIDs, List<MonthReport> monthReports)>> GetReportsOnPeriodAsync(DateTime periodStart, DateTime periodEnd);
    }
}
