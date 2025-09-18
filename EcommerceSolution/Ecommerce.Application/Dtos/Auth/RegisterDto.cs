using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Application.Dtos.Auth
{
    public class RegisterDto
    {

        [Required]
        [MinLength(3, ErrorMessage = "Username must be at least 3 characters.")]
        public string UserName { get; set; } = string.Empty;

        [Required]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters.")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, and one number.")]


        public string Password { get; set; } = string.Empty;

    }
}
