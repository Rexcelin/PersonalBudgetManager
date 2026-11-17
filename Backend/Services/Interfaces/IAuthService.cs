using PersonalBudgetManager.DTOs.Auth;

namespace PersonalBudgetManager.Services.Interfaces;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);

    Task<AuthResponseDto?> LoginAsync(LoginRequestDto request);
}
