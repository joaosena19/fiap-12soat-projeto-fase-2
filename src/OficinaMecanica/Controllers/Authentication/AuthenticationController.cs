using API.Dtos;
using Application.Authentication.Dtos;
using Application.Authentication.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers.Authentication;

[ApiController]
[Route("api/authentication")]
public class AuthenticationController : ControllerBase
{
    private readonly IAuthenticationService _authService;

    public AuthenticationController(IAuthenticationService authService)
    {
        _authService = authService;
    }

    /// <summary>
    /// Autenticação que recebe client credentials e retorna um Bearer token
    /// </summary>
    /// <param name="request">Client credentials</param>
    /// <returns>JWT access token</returns>
    /// <response code="200">Retorna o token JWT</response>
    /// <response code="400">Dados inválidos fornecidos</response>
    /// <response code="401">Credenciais inválidas</response>
    [HttpPost("token")]
    [ProducesResponseType(typeof(TokenResponseDto), 200)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ErrorResponseDto), StatusCodes.Status401Unauthorized)]
    [AllowAnonymous]
    public ActionResult<TokenResponseDto> GetToken([FromBody] TokenRequestDto request)
    {
        var tokenResponse = _authService.ValidateCredentialsAndGenerateToken(request);
        return Ok(tokenResponse);
    }
}
