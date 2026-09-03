using Application.DTOs;
using Application.Interfaces;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/auth/")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequestDto request)
    {
        var success = await _authService.Register(request);

        if(!success)
        {
            return Conflict("Korisnik sa navedenim email-om već postoji.");
        }

        return Ok("Uspješna registracija!");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto request)
    {
        try
        {
            var authResponse = await _authService.Login(request);
            return Ok(authResponse);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenDto refreshToken)
    {
        try
        {
            var authResponse = await _authService.Refresh(refreshToken);
            return Ok(authResponse);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
    }

    [HttpPost("approve-recruiter/{userId}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> ApproveRecruiter(Guid userId)
    {
        try
        {
            await _authService.ApproveRecruiter(userId);
            return Ok();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ex.Message);
        }
    }
}
