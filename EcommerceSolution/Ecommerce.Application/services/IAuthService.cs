using Ecommerce.Application.Dtos.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Application.services
{

        public interface IAuthService
        {
            Task<UserDto> RegisterAsync(RegisterDto dto);
            Task<AuthResponseDto> LoginAsync(LoginDto dto);
            Task<AuthResponseDto> RefreshTokenAsync(RefreshRequestDto dto);
        }
    }

