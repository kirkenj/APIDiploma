using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Logic.RequestModels.Authorize
{
    public class RegisterUserModel : IValidatableObject
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
        public string PasswordStr { get; set; } = null!;
        [Required]
        public string ConfirmPasswordStr { get; set; } = null!;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrEmpty(Name))
            {
                yield return new ValidationResult($"{nameof(Name)} is null or empty");
            }

            if (string.IsNullOrEmpty(Surname))
            {
                yield return new ValidationResult($"{nameof(Surname)} is null or empty");
            }

            if (string.IsNullOrEmpty(Patronymic))
            {
                yield return new ValidationResult($"{nameof(Patronymic)} is null or empty");
            }

            if (string.IsNullOrEmpty(Login))
            {
                yield return new ValidationResult($"{nameof(Login)} is null or empty");
            }

            if (PasswordStr != ConfirmPasswordStr)
            {
                yield return new ValidationResult($"PasswordStr != ConfirmPasswordStr");
            }

            if (PasswordStr.Length < 6)
            {
                yield return new ValidationResult($"PasswordStr.Length < 6");
            }

            if (!Regex.IsMatch(PasswordStr, "^[a-zA-Z0-9]+$"))
            {
                yield return new ValidationResult($"{nameof(PasswordStr)} does not match regex: \"^[a-zA-Z0-9]+$\"");
            }
        }
    }
}
