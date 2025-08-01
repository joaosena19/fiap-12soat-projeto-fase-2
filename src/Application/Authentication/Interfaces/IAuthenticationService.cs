using Application.Authentication.DTO;

namespace Application.Authentication.Interfaces;

public interface IAuthenticationService
{
    TokenResponseDto? ValidateCredentialsAndGenerateToken(TokenRequestDto request);
}