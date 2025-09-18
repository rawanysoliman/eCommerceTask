using AutoMapper;
using Ecommerce.Application.Dtos.product;
using Ecommerce.Application.services;
using Ecommerce.Domain.entities;
using Ecommerce.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.services.ProductServices
{
    public class ProductService : IProductService
    {
        private readonly AppDbContext _context;
        private readonly IFileStorageService _fileStorageService;
        private readonly IMapper _mapper;

        public ProductService(AppDbContext context, IFileStorageService fileStorageService, IMapper mapper)
        {
            _context = context;
            _fileStorageService = fileStorageService;
            _mapper = mapper;
        }

        public async Task<ProductDto> CreateAsync(CreateProductDto dto)
        {
            string? imageUrl = null;

            if (dto.Image != null)
            {
                imageUrl = await _fileStorageService.SaveFileAsync(dto.Image, "images/products");
            }

            // Generate unique product code 
            var productCode = await GenerateUniqueProductCodeAsync();

           
            var product = _mapper.Map<Product>(dto);
            product.ProductCode = productCode;
            product.ImagePath = imageUrl;

            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return _mapper.Map<ProductDto>(product);
        }

        public async Task<ProductDto> UpdateAsync(int id, UpdateProductDto dto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                throw new Exception("Product not found");

            
            _mapper.Map(dto, product);

            // Handle image replacement
            if (dto.Image != null)
            {
                // Delete old image if exists
                if (!string.IsNullOrEmpty(product.ImagePath))
                {
                    _fileStorageService.DeleteFile(product.ImagePath);
                }

                // Save new image
                product.ImagePath = await _fileStorageService.SaveFileAsync(dto.Image, "images/products");
            }

            product.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return _mapper.Map<ProductDto>(product);
        }

        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                throw new Exception("Product not found");

            // delete image if exists
            if (!string.IsNullOrEmpty(product.ImagePath))
            {
                _fileStorageService.DeleteFile(product.ImagePath);
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<ProductDto>> GetAllAsync()
        {
            var products = await _context.Products.ToListAsync();
            return _mapper.Map<IEnumerable<ProductDto>>(products);
        }

        public async Task<ProductDto> GetByIdAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                throw new Exception("Product not found");

            return _mapper.Map<ProductDto>(product);
        }


        private async Task<string> GenerateUniqueProductCodeAsync()
        {
            //Avoids EF trying to compose more SQL on top of a non-composable stored procedure.
            var results = await _context.Database
                .SqlQuery<string>($"EXEC GetNextProductCode")
                .ToListAsync();

            var nextCode = results.FirstOrDefault();
            return nextCode ?? "P01";
        }

        //using stored procedure to get products by price range
        public async Task<List<Product>> GetProductsByPriceRangeAsync(decimal minPrice, decimal maxPrice)
        {
            return await _context.Products
                .FromSqlRaw("EXEC GetProductsByPriceRange @MinPrice = {0}, @MaxPrice = {1}", minPrice, maxPrice)
                .ToListAsync();
        }

    }
}
