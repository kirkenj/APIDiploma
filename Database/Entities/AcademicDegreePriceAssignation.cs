using Database.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Entities
{
    [Table(nameof(AcademicDegreePriceAssignation) + "s")]
    public class AcademicDegreePriceAssignation : IPeriodicValueAssignment<double, int, AcademicDegree>
    {
        public DateTime AssignmentDate { get; set ; }
        public double Value { get; set; }
        public int ObjectIdentifier { get; set; }
        public AcademicDegree ObjectRef { get; set; } = null!;
    }
}
