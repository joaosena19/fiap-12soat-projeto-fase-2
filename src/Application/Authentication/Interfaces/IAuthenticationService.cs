using Application.Authentication.Dtos;

namespace Application.Authentication.Interfaces;

public interface IAuthenticationService
{
    TokenResponseDto ValidateCredentialsAndGenerateToken(TokenRequestDto request);
}