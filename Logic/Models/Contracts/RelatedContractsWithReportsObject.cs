using Database.Entities;
using Logic.Models.MonthReports;

namespace Logic.Models.Contracts
{
    public class RelatedContractsWithReportsObject
    {
        public IEnumerable<Contract> Contracts { get; set; } = null!;
        public IEnumerable<MonthReport> Reports { get; set; } = null!;
        public IEnumerable<MonthReportsUntakenTimeModel> UntakenTimeForContracts { get; set; } = null!;
    }
}
