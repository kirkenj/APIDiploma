using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Entities
{
    [Table(nameof(User) + "s")]
    public class User
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
    }
}
