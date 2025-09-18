using AutoMapper;
using Ecommerce.Application.Dtos.Auth;
using Ecommerce.Application.services;
using Ecommerce.Domain.entities;
using Ecommerce.Infrastructure.Persistence;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ecommerce.Infrastructure.services.AuthServices
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IJwtService _jwtService;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly IMapper _mapper;

        public AuthService(AppDbContext context, IJwtService jwtService, IPasswordHasher<User> passwordHasher, IMapper mapper)
        {
            _context = context;
            _jwtService = jwtService;
            _passwordHasher = passwordHasher;
            _mapper = mapper;
        }
        public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users.SingleOrDefaultAsync(u =>
                    u.UserName == dto.UserNameOrEmail || u.Email == dto.UserNameOrEmail);

            if (user == null)
                throw new Exception("Invalid credentials");

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, dto.Password);
            if (result == PasswordVerificationResult.Failed)
                throw new Exception("Invalid credentials");

            var roles = new List<string>();
            if (!string.IsNullOrEmpty(user.Role)) roles.Add(user.Role);
            var accessToken = _jwtService.GenerateAccessToken(user, roles);
            var refreshToken = _jwtService.GenerateRefreshToken();

            user.LastLoginTime = DateTimeOffset.UtcNow;
            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,
                ExpiresAt = refreshToken.ExpiresAt
            };
        }

        public async Task<AuthResponseDto> RefreshTokenAsync(RefreshRequestDto dto)
        {
            var storedToken = await _context.RefreshTokens
            .Include(rt => rt.User) 
            .SingleOrDefaultAsync(rt => rt.Token == dto.RefreshToken);

                if (storedToken == null)
                    throw new Exception("Invalid refresh token");

                if (storedToken.ExpiresAt <= DateTime.UtcNow)
                    throw new Exception("Refresh token expired");

                if (storedToken.Revoked)
                    throw new Exception("Refresh token has been revoked");

                var user = storedToken.User;
                if (user == null)
                    throw new Exception("User not found for this token");

                // Invalidate the old refresh token
                storedToken.Revoked = true;
                _context.RefreshTokens.Update(storedToken);

              
            var roles = new List<string>();
            if (!string.IsNullOrEmpty(user.Role)) roles.Add(user.Role);
            var accessToken = _jwtService.GenerateAccessToken(user, roles);
                var newRefreshToken = _jwtService.GenerateRefreshToken();

               
                var refreshEntity = new RefreshToken
                {
                    Token = newRefreshToken.Token,
                    ExpiresAt = newRefreshToken.ExpiresAt,
                    UserId = user.Id
                };

            _context.RefreshTokens.Add(refreshEntity);
            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshToken.Token,
                ExpiresAt = newRefreshToken.ExpiresAt
            };
        }


        public async Task<UserDto> RegisterAsync(RegisterDto dto)
        {
            // Check for unique username and email
            if (await _context.Users.AnyAsync(u => u.UserName == dto.UserName || u.Email == dto.Email))
                throw new Exception("Username or email already exists");

            // Use AutoMapper to map DTO to entity
            var user = _mapper.Map<User>(dto);
            user.PasswordHash = _passwordHasher.HashPassword(null!, dto.Password);
            user.Role = "User";

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return _mapper.Map<UserDto>(user);
        }


    }
}
