using Application.Authentication.DTO;
using Application.Authentication.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Application.Authentication.Services;

public class AuthenticationService : IAuthenticationService
{
    private readonly IConfiguration _configuration;
    private readonly ITokenService _tokenService;

    public AuthenticationService(IConfiguration configuration, ITokenService tokenService)
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