using Database.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Entities
{
    [Table(nameof(ContractType) + "s")]
    public class ContractType : IIdObject<int>, IPeriodicValueObject<ContractTypePriceAssignation, int, double, ContractType>
    {
        public int ID { get; set; }
        public string Name { get; set; } = null!;
        public IEnumerable<Contract> Contracts { get; set; } = null!;
        public IEnumerable<ContractTypePriceAssignation> Assignations { get; set; } = null!;
    }
}
