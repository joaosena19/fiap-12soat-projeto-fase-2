namespace Application.Authentication.Dtos;

public record TokenResponseDto(string Token, string TokenType = "Bearer", int ExpiresIn = 3600);
