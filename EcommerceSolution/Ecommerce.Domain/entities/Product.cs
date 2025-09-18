using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Domain.entities
{

    public class Product
    {
        public int Id { get; set; }

        [Required]
        [StringLength(10)]
        public string ProductCode { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Category { get; set; } = string.Empty;
        

        [StringLength(500)]
        public string? ImagePath { get; set; }
        
 
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }
        

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Minimum quantity must be at least 1")]
        public int MinimumQuantity { get; set; }
        
 
        [Required]
        [Range(0, 100, ErrorMessage = "Discount rate must be between 0 and 100")]
        public decimal DiscountRate { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
