using Database.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Entities
{
    [Table(nameof(AcademicDegree) + "s")]
    public class AcademicDegree : IIdObject<int>, IPeriodicValueObject<AcademicDegreePriceAssignment, int, double, AcademicDegree>
    {
        public int ID { get; set; }
        public string Name { get; set; } = null!;
        public IEnumerable<AcademicDegreePriceAssignment> Assignments { get; set; } = null!;
        public IEnumerable<UserAcademicDegreeAssignament> UserAssignations { get; set; } = null!;
    }
}
