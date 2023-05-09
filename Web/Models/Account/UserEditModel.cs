using System.ComponentModel.DataAnnotations;

namespace Web.RequestModels.Account
{
    public class UserEditModel
    {
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string Surname { get; set; } = null!;
        [Required]
        public string Patronymic { get; set; } = null!;
        [Required]
        public string Login { get; set; } = null!;
        public int RoleID { get; set; } = -1;
    }
}
