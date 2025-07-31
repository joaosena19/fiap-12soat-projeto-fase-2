using Application.Auth.DTO;
using Application.Auth.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Application.Auth.Services;

public class AuthService : IAuthService
{
    private readonly IConfiguration _configuration;
    private readonly ITokenService _tokenService;

    public AuthService(IConfiguration configuration, ITokenService tokenService)
    {
        _configuration = configuration;
        _tokenService = tokenService;
    }

    public TokenResponseDto? ValidateCredentialsAndGenerateToken(TokenRequestDto request)
    {
        var configuredClientId = _configuration["ApiCredentials:ClientId"];
        var configuredClientSecret = _configuration["ApiCredentials:ClientSecret"];

        if (string.IsNullOrEmpty(configuredClientId) || string.IsNullOrEmpty(configuredClientSecret))
        {
            return null;
        }

        if (request.ClientId != configuredClientId || request.ClientSecret != configuredClientSecret)
        {
            return null;
        }

        var token = _tokenService.GenerateToken(request.ClientId);
        return new TokenResponseDto(token);
    }
}