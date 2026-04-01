using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Auth.Application.DTOs;
using Auth.Application.Interfaces;
using Auth.Domain.Entities;
using Auth.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Auth.Application.Services;

public class AuthService : IAuthService
{
    private readonly UserRepository _repository;
    private readonly IConfiguration _configuration;
    private readonly PasswordHasher<User> _passwordHasher;

    public AuthService(UserRepository repository, IConfiguration configuration)
    {
        _repository = repository;
        _configuration = configuration;
        _passwordHasher = new PasswordHasher<User>();
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await _repository.GetByEmailAsync(request.Email.Trim().ToLowerInvariant());
        if (user == null)
            throw new UnauthorizedAccessException("Invalid credentials");

        var verify = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (verify == PasswordVerificationResult.Failed)
            throw new UnauthorizedAccessException("Invalid credentials");

        var token = GenerateJwtToken(user);
        return new LoginResponse { Token = token, Expiration = DateTime.UtcNow.AddHours(1) };
    }

    public async Task RegisterAsync(RegisterRequest request)
    {
        var existing = await _repository.GetByEmailAsync(request.Email.Trim().ToLowerInvariant());
        if (existing != null)
            throw new InvalidOperationException("User already exists");

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = request.Email.Trim().ToLowerInvariant(),
            Role = request.Role,
            CreatedAt = DateTime.UtcNow
        };

        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        await _repository.AddAsync(user);
    }

    private string GenerateJwtToken(User user)
    {
        var secret = _configuration["Jwt:Key"];
        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];


        if (string.IsNullOrEmpty(secret))
            throw new InvalidOperationException("JWT key is not configured.");

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(ClaimTypes.Name, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var expires = DateTime.UtcNow.AddHours(1);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
