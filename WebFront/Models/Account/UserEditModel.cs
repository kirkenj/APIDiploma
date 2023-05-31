using System.ComponentModel.DataAnnotations;

namespace WebFront.Models.Account
{
    public class UserEditModel
    {
        public string Login { get; set; } = null!;
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string Surname { get; set; } = null!;
        [Required]
        public string Patronymic { get; set; } = null!;
        [Required]
        public int RoleId { get; set; } = -1;
    }
}
