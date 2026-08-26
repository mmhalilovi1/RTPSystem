using Application.DTOs;
using Application.Interfaces;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Services
{
    public class AuthService : IAuthService
    {  
        private readonly IApplicationDbContext _context;
        private readonly IPasswordHasher _passwordHasher;

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
    }
}
