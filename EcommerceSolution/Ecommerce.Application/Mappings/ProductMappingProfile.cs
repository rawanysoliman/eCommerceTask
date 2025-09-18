using AutoMapper;
using Ecommerce.Application.Dtos.product;
using Ecommerce.Domain.entities;

namespace Ecommerce.Application.Mappings
{

    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            // Product -> ProductDto
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImagePath));

            // CreateProductDto -> Product
            CreateMap<CreateProductDto, Product>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ProductCode, opt => opt.Ignore()) // Auto-generated
                .ForMember(dest => dest.ImagePath, opt => opt.Ignore()) // Handled separately
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());

            // UpdateProductDto -> Product
            CreateMap<UpdateProductDto, Product>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.ProductCode, opt => opt.Ignore()) // Not updatable
                .ForMember(dest => dest.ImagePath, opt => opt.Ignore()) // Handled separately
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore());
        }
    }
}
