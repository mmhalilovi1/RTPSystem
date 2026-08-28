using Application.DTOs;
using Application.Interfaces;
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
    public async Task<IActionResult> Login(RegisterRequestDto request)
    {
        var authResponse = await _authService.Login(request);
        return Ok(authResponse);
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenDto refreshToken)
    {
        var authResponse = await _authService.Refresh(refreshToken);
        return Ok(authResponse);
    }
}
