using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Ecommerce.Application.Dtos.product
{

    public class CreateProductDto
    {

        [Required(ErrorMessage = "Product name is required")]
        [StringLength(200, ErrorMessage = "Product name cannot exceed 200 characters")]
        public string Name { get; set; } = string.Empty;
        

        [Required(ErrorMessage = "Category is required")]
        [StringLength(100, ErrorMessage = "Category cannot exceed 100 characters")]
        public string Category { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }
        

        [Required(ErrorMessage = "Minimum quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Minimum quantity must be at least 1")]
        public int MinimumQuantity { get; set; }
        

        [Required(ErrorMessage = "Discount rate is required")]
        [Range(0, 100, ErrorMessage = "Discount rate must be between 0 and 100")]
        public decimal DiscountRate { get; set; }
        

        public IFormFile? Image { get; set; }
    }
}
