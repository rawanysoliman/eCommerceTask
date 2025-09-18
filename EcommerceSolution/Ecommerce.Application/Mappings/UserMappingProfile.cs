using AutoMapper;
using Ecommerce.Application.Dtos.Auth;
using Ecommerce.Domain.entities;

namespace Ecommerce.Application.Mappings
{

    public class UserMappingProfile : Profile
    {
        public UserMappingProfile()
        {
            // User -> UserDto
            CreateMap<User, UserDto>();

            // RegisterDto -> User
            CreateMap<RegisterDto, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()) // Handled separately
                .ForMember(dest => dest.LastLoginTime, opt => opt.Ignore())
                .ForMember(dest => dest.RefreshTokens, opt => opt.Ignore());
        }
    }
}
