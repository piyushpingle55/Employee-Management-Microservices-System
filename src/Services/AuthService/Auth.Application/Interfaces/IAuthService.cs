using Auth.Application.DTOs;

namespace Auth.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task RegisterAsync(RegisterRequest request);
}
