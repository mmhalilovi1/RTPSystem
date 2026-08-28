using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<bool> Register(RegisterRequestDto registerRequestDto);
        Task<AuthResponseDto> Login(RegisterRequestDto loginDto);
        Task<AuthResponseDto> Refresh(RefreshTokenDto refreshTokenDto);
    }
}
