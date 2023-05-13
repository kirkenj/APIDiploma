﻿using Database.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Entities
{
    [Table(nameof(User) + "s")]
    public class User : IConfirmableByAdminObject, IIdObject<int>, IPeriodicValueObject<UserAcademicDegreeAssignament, int, int, User>
    {
        public int ID { get; set; }
        public string Name { get; set; } = null!;
        public string Surname { get; set; } = null!;
        public string Patronymic { get; set; } = null!;
        public IEnumerable<Contract> Contracts { get; set; } = null!;
        public int RoleId { get; set; } = 2;
        public Role Role { get; set; } = null!;
        public string Login { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public IEnumerable<Contract> ConfirmedContracts { get; set; } = null!;
        public IEnumerable<User> ConfirmedUsers { get; set; } = null!;
        public bool IsConfirmed => ConfirmedByUserID != null;
        public int? ConfirmedByUserID { get; set; }
        public User? ConfirmedByUser { get; set; }
        public IEnumerable<UserAcademicDegreeAssignament> Assignments { get; set; } = null!;
    }
}
