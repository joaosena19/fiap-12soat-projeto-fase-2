using Application.Auth.DTO;

namespace Application.Auth.Interfaces;

public interface IAuthService
{
    TokenResponseDto? ValidateCredentialsAndGenerateToken(TokenRequestDto request);
}