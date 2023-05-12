using Database.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Entities
{
    [Table(nameof(ContractTypePriceAssignation) + "s")]
    public class ContractTypePriceAssignation : IPeriodicValueAssignation<double, int, ContractType>
    {
        public DateTime AssignationDate { get; set ; }
        public double Value { get; set; }
        public int ObjectIdentifier { get; set; }
        public ContractType ObjectRef { get; set; } = null!;
    }
}
