namespace Application.Auth.Interfaces;

public interface ITokenService
{
    string GenerateToken(string clientId);
}
