using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Domain.entities
{

    public class User
    {
        public int Id { get; set; }
        

        [Required]
        [StringLength(100)]
        public string UserName { get; set; } = string.Empty;
        

        [Required]
        [EmailAddress]
        [StringLength(200)]
        public string Email { get; set; } = string.Empty;
        

        [Required]
        public string PasswordHash { get; set; } = string.Empty;


        public DateTimeOffset? LastLoginTime { get; set; }
        

        // Simple role support (e.g., "User", "Admin")
        [StringLength(50)]
        public string Role { get; set; } = "User";

        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
