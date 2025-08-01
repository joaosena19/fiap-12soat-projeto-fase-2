namespace Application.Authentication.Interfaces;

public interface ITokenService
{
    string GenerateToken(string clientId);
}
