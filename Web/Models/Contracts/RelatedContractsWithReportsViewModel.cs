using WebFront.RequestModels.Contracts;

namespace WebFront.Models.Contracts
{
    public class RelatedContractsWithReportsViewModel
    {
        public List<ContractViewModel> RelatedContracts { get; set; } = null!;
        public List<MonthReportViewModel> MonthReports { get; set; } = null!;
    }
}
