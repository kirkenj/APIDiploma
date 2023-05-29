using Database.Interfaces;
using System.ComponentModel.DataAnnotations.Schema;

namespace Database.Entities
{
    [Table(nameof(Department) + "s")]
    public class Department : IIdObject<int>
    {
        public int ID { get; set; }
        public string Name { get; set; } = null!;
        internal IEnumerable<Contract> Contracts { get; set; } = null!;
    }
}
