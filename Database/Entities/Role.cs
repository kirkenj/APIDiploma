using Database.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Entities
{
    [Table(nameof(Role) + "s")]
    public class Role : IIdObject<int>
    {
        public int ID { get; set; }
        public string Name { get; set; } = null!;
        public IEnumerable<User> Users { get; set; } = null!;
    }
}
