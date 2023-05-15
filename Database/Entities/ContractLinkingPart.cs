using Database.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Entities
{
    [Table(nameof(ContractLinkingPart) + "s")]
    public class ContractLinkingPart : IIdObject<int>
    {
        public int ID { get; set; }
        public IEnumerable<Contract> Assignments { get; set; } = null!;
        public IEnumerable<MonthReport> MonthReports { get; set; } = null!;
        public int SourceContractID { get; set; }
    }
}
