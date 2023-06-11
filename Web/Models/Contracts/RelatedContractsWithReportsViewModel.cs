using Logic.Models.MonthReports;
using WebFront.RequestModels.Contracts;

namespace WebFront.Models.Contracts
{
    public class RelatedContractsWithReportsViewModel
    {
        public IEnumerable<ContractViewModel> RelatedContracts { get; set; } = null!;
        public IEnumerable<MonthReportViewModel> MonthReports { get; set; } = null!;
        public IEnumerable<MonthReportsUntakenTimeModel> UntakenTimes { get; set; } = null!;
    }
}
