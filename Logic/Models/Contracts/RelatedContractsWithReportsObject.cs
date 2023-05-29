using Database.Entities;

namespace Logic.Models.Contracts
{
    public class RelatedContractsWithReportsObject
    {
        public IEnumerable<Contract> Contracts { get; set; } = null!;
        public IEnumerable<MonthReport> Reports { get; set; } = null!;
    }
}
