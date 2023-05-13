using Database.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Entities
{
    [Table(nameof(ContractTypePriceAssignment) + "s")]
    public class ContractTypePriceAssignment : IPeriodicValueAssignment<double, int, ContractType>
    {
        public DateTime AssignmentDate { get; set ; }
        public double Value { get; set; }
        public int ObjectIdentifier { get; set; }
        public ContractType ObjectRef { get; set; } = null!;
    }
}
