
using Microsoft.AspNetCore.Mvc;
using PersonalBudgetManager.DTOs.Auth;
using PersonalBudgetManager.Services.Interfaces;

namespace PersonalBudgetManager.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(
        RegisterRequestDto request)
    {
        try
        {
            var response = await _authService.RegisterAsync(request);

            return StatusCode(StatusCodes.Status201Created, new
            {
                message = "Registration successful.",
                data = response
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(
        LoginRequestDto request)
    {
        var response = await _authService.LoginAsync(request);

        if (response == null)
        {
            return Unauthorized(new
            {
                message = "Invalid email or password."
            });
        }

        return Ok(new
        {
            message = "Login successful.",
            data = response
        });
    }
}
