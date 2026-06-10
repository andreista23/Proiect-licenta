using Microsoft.AspNetCore.Mvc;
using RecruitmentPlatform.Application.DTOs.Auth;
using RecruitmentPlatform.Application.Interfaces;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace RecruitmentPlatform.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
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
        try
        {
            var response = await _authService.RegisterAsync(request);
            return Ok(response);
        }
        catch (InvalidOperationException exception)
        {
            return BadRequest(new
            {
                message = exception.Message
            });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequestDto request)
    {
        try
        {
            var response = await _authService.LoginAsync(request);
            return Ok(response);
        }
        catch (UnauthorizedAccessException exception)
        {
            return Unauthorized(new
            {
                message = exception.Message
            });
        }
    }

    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> Me()
    {
        var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Unauthorized(new
            {
                message = "Invalid token."
            });
        }

        try
        {
            var response = await _authService.GetCurrentUserAsync(userId);
            return Ok(response);
        }
        catch (UnauthorizedAccessException exception)
        {
            return Unauthorized(new
            {
                message = exception.Message
            });
        }
    }
}