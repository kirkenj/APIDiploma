﻿using Database.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Entities
{
    [Table(nameof(UserAcademicDegreeAssignment) + "s")]
    public class UserAcademicDegreeAssignment : IPeriodicValueAssignment<int, int, User>
    {
        public DateTime AssignmentDate { get; set; }
        public int Value { get; set; }
        public AcademicDegree ValueRef { get; set; } = null!;
        public int ObjectIdentifier { get; set; }
        public User ObjectRef { get; set; } = null!;
    }
}
