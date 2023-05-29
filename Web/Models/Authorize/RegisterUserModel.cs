using System.ComponentModel.DataAnnotations;

namespace WebFront.RequestModels.Authorize
{
    public class RegisterUserModel
    {
        [Required]
        public string Name { get; set; } = null!;
        [Required]
        public string Surname { get; set; } = null!;
        [Required]
        public string Patronymic { get; set; } = null!;
        [Required]
        public string Login { get; set; } = null!;
        [Required]
        [MinLength(6)]
        [RegularExpression("^[a-zA-Z0-9]+$")]
        public string PasswordStr { get; set; } = null!;
    }
}
