using Database.Entities;
using Logic.Interfaces.Common;
using Logic.Models.Contracts;
using Logic.Models.MonthReports;

namespace Logic.Interfaces
{
    public interface IContractService : IDbAccessServise<Contract>, IConfirmService<Contract>, IGetViaSelectObjectService<Contract, ContractsSelectObject>
    {
        public Task<IEnumerable<MonthReport>> GetMonthReportsAsync(int contractID);
        public Task UpdateMonthReport(MonthReport monthReport);
        public Task<string?> GetOwnersLoginAsync(int contractID);
        public Task<IEnumerable<Contract>> GetRelatedContracts(Contract contract, CancellationToken token = default);
        public Task<MonthReportsUntakenTimeModel> GetUntakenTimeAsync(int contractID, IEnumerable<(int year, int month)> exceptValuesWithKeys, bool replaceNegativesWithZero = false);
        public Task<IEnumerable<RelatedContractsWithReportsObject>> GetReportsOnPeriodAsync(DateTime periodStart, DateTime periodEnd);
        public Task<RelatedContractsWithReportsObject> GetFullData(int contractID);
        public Task<MonthReport?> GetReport(int linkingPartID, int month, int year);
        public Task BlockReport(int linkingPartID, int month, int year, int userID);
        public Task UnBlockReport(int linkingPartID, int month, int year, int userID);
        public IQueryable<KeyValuePair<Contract, bool>> GetContractHasChildKeyValuePair(ContractsSelectObject? selectionObject);
        public Task<MonthReportsUntakenTimeModel> GetMaxValuesForReport(int linkingPartId, int repYear, int repMonth);
    }
}
