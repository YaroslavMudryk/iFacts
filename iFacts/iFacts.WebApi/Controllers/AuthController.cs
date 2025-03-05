using iFacts.Shared.Api;
using iFacts.WebApi.Dtos;
using iFacts.WebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace iFacts.WebApi.Controllers;

[Route("/api/v1/identity")]
[ApiController]
public class AuthController(AuthService authService) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> RegisterAsync(RegisterDto register)
    {
        await authService.RegisterAsync(register);
        return Ok();
    }

    [AllowAnonymous]
    [HttpPost("sign-in")]
    public async Task<IActionResult> LoginAsync(LoginDto login)
    {
        var loginDto = await authService.LoginAsync(login);
        return Ok(loginDto.MapToResponse());
    }

    [HttpDelete("sign-out")]
    public async Task<IActionResult> LogoutAsync()
    {
        await authService.LogoutAsync();
        return NoContent();
    }

    [HttpGet("sessions")]
    public async Task<IActionResult> GetSessionsAsync()
    {
        var sessionsResponse = await authService.GetUserSessionsAsync();
        return Ok(sessionsResponse.MapToResponse());
    }

    [HttpDelete("sessions")]
    public async Task<IActionResult> RevokeSessionsAsync(IReadOnlyList<string> sessionIds)
    {
        await authService.RevokeSessionsAsync(sessionIds);
        return NoContent();
    }
}
