using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Infrastructure.Services
{
    public class AuthService : IAuthService
    {  
        private readonly IApplicationDbContext _context;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;

        public AuthService(IApplicationDbContext context, IPasswordHasher passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task<bool> Register(RegisterRequestDto registerRequestDto)
        {
            var emailExists = await _context.Users.AnyAsync(u => u.Email == registerRequestDto.Email);

            if (emailExists) return false;

            var passwordHash = _passwordHasher.Hash(registerRequestDto.Password);

            var user = new User(registerRequestDto.Email, passwordHash);

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<AuthResponseDto> Login(RegisterRequestDto loginRequestDto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == loginRequestDto.Email);

            if (user is null) 
                throw new UnauthorizedAccessException("Neispravan email");

            var isValidPassword = _passwordHasher.Verify(loginRequestDto.Password, user.PasswordHash);

            if (!isValidPassword)
                throw new UnauthorizedAccessException("Neispravna lozinka");

            var accessToken = _jwtTokenGenerator.GenerateToken(user);
            var refreshToken = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

            var refreshTokenHash = _passwordHasher.Hash(refreshToken);
            var refreshTokenEntity = new RefreshToken(user.Id, refreshTokenHash);

            _context.RefreshTokens.Add(refreshTokenEntity);

            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
            };
        }

        public async Task<AuthResponseDto> Refresh(RefreshTokenDto refreshTokenRequestDto)
        {
            var refreshTokens = await _context.RefreshTokens
                .Where(x =>
                    x.RevokedAt == null &&
                    x.ExpiresAt > DateTime.UtcNow)
                .ToListAsync();

            RefreshToken? refreshToken = null;

            foreach (var token in refreshTokens)
            {
                if (_passwordHasher.Verify(refreshTokenRequestDto.RefreshToken, token.TokenHash))
                {
                    refreshToken = token;
                    break;
                }
            }

            if (refreshToken is null)
                throw new UnauthorizedAccessException("Neispravan refresh token.");

            var user = await _context.Users
                .FirstOrDefaultAsync(x => x.Id == refreshToken.UserId);

            if (user is null)
                throw new UnauthorizedAccessException("Neispravan refresh token.");

            refreshToken.Revoke();

            var accessToken = _jwtTokenGenerator.GenerateToken(user);
            var newRefreshTokenValue = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

            var newRefreshTokenHash = _passwordHasher.Hash(newRefreshTokenValue);
            var newRefreshToken = new RefreshToken(user.Id, newRefreshTokenHash);

            _context.RefreshTokens.Add(newRefreshToken);

            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = newRefreshTokenValue
            };
        }
    }
}
