using System.ComponentModel.DataAnnotations;

namespace WebFront.Models.Authorize
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
        public string Password { get; set; } = null!;
        [Compare(nameof(Password))]
        public string ConfirmPassword { get; set; } = null!;
    }
}
