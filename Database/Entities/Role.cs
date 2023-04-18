using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.Entities
{
    [Table(nameof(Role) + "s")]
    public class Role
    {
        public int ID { get; set; }
        public string Name { get; set; } = null!;
        public IEnumerable<User> Users { get; set; } = null!;
    }
}
