using Database.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Entities
{
    [Table(nameof(UserAcademicDegreeAssignation) + "s")]
    public class UserAcademicDegreeAssignation : IPeriodicValueAssignation<int, int, User>
    {
        public DateTime AssignationDate { get; set; }
        public int Value { get; set; }
        public AcademicDegree ValueRef { get; set; } = null!;
        public int ObjectIdentifier { get; set; }
        public User ObjectRef { get; set; } = null!;
    }
}
