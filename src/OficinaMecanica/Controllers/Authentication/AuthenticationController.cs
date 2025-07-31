using Application.Auth.DTO;
using Application.Auth.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Authentication;

[ApiController]
[Route("api/authentication")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthenticationController(IAuthService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Autenticação que recebe client credentials e retorna um Bearer token
    /// </summary>
    /// <param name="request">Client credentials</param>
    /// <returns>JWT access token</returns>
    /// <response code="200">Retorna o token JWT</response>
    /// <response code="401">Credenciais inválidas</response>
    [HttpPost("token")]
    [ProducesResponseType(typeof(TokenResponseDto), 200)]
    [ProducesResponseType(401)]
    [AllowAnonymous]
    public ActionResult<TokenResponseDto> GetToken([FromBody] TokenRequestDto request)
    {
        if (string.IsNullOrEmpty(request.ClientId) || string.IsNullOrEmpty(request.ClientSecret))
        {
            return BadRequest("ClientId e ClientSecret requeridos.");
        }

        var tokenResponse = _authService.ValidateCredentialsAndGenerateToken(request);
        
        if (tokenResponse == null)
        {
            return Unauthorized("Credenciais inválidas.");
        }

        return Ok(tokenResponse);
    }
}
