
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using PersonalBudgetManager.DTOs.Auth;
using PersonalBudgetManager.Models;
using PersonalBudgetManager.Services.Interfaces;

namespace PersonalBudgetManager.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;

    public AuthService(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<AuthResponseDto> RegisterAsync(
        RegisterRequestDto request)
    {
        var existingUser = await _userManager.FindByEmailAsync(request.Email);

        if (existingUser != null)
        {
            throw new InvalidOperationException(
                "An account with this email already exists.");
        }

        var user = new ApplicationUser
        {
            FullName = request.FullName.Trim(),
            Email = request.Email.Trim(),
            UserName = request.Email.Trim()
        };

        var result = await _userManager.CreateAsync(
            user,
            request.Password);

        if (!result.Succeeded)
        {
            var errors = string.Join(
                " ",
                result.Errors.Select(error => error.Description));

            throw new InvalidOperationException(errors);
        }

        return GenerateAuthResponse(user);
    }

    public async Task<AuthResponseDto?> LoginAsync(
        LoginRequestDto request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            return null;
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(
            user,
            request.Password);

        if (!isPasswordValid)
        {
            return null;
        }

        return GenerateAuthResponse(user);
    }

    private AuthResponseDto GenerateAuthResponse(ApplicationUser user)
    {
        var jwtKey = _configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("JWT Key is missing.");

        var issuer = _configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException("JWT Issuer is missing.");

        var audience = _configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException("JWT Audience is missing.");

        var expiryMinutes = _configuration.GetValue<int>(
            "Jwt:ExpiryInMinutes");

        if (expiryMinutes <= 0)
        {
            expiryMinutes = 60;
        }

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var expiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Email, user.Email ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials
        );

        var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

        return new AuthResponseDto
        {
            UserId = user.Id,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            Token = tokenString,
            ExpiresAt = expiresAt
        };
    }
}
