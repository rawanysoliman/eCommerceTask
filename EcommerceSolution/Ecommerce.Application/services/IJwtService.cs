using Ecommerce.Application.Dtos.Auth;
using Ecommerce.Domain.entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Ecommerce.Application.services
{
    public interface IJwtService
    {
        string GenerateAccessToken(User user, IEnumerable<string>? roles = null);
        RefreshTokenResult GenerateRefreshToken();

    }
}
