using API.Controllers.Authentication;
using Application.Authentication.Dtos;
using Application.Authentication.Interfaces;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shared.Enums;
using Shared.Exceptions;
using System.Net;
using System.Net.Http.Json;
using Tests.Integration;

namespace Tests.Other.Authentication
{
    public class AuthenticationControllerTests : IClassFixture<TestWebApplicationFactory<Program>>
    {
        private readonly TestWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;
        private readonly Mock<IAuthenticationService> _authServiceMock;
        private readonly AuthenticationController _controller;

        public AuthenticationControllerTests(TestWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient(); // Usa client sem autenticação

            // Setup for unit tests
            _authServiceMock = new Mock<IAuthenticationService>();
            _controller = new AuthenticationController(_authServiceMock.Object);
        }

        #region Endpoints que precisam de Authorize

        [Theory]
        // ClienteController endpoints
        [InlineData("GET", "/api/cadastros/clientes")]
        [InlineData("GET", "/api/cadastros/clientes/00000000-0000-0000-0000-000000000000")]
        [InlineData("GET", "/api/cadastros/clientes/documento/12345678901")]
        [InlineData("POST", "/api/cadastros/clientes")]
        [InlineData("PUT", "/api/cadastros/clientes/00000000-0000-0000-0000-000000000000")]
        // ServicoController endpoints
        [InlineData("GET", "/api/cadastros/servicos")]
        [InlineData("GET", "/api/cadastros/servicos/00000000-0000-0000-0000-000000000000")]
        [InlineData("POST", "/api/cadastros/servicos")]
        [InlineData("PUT", "/api/cadastros/servicos/00000000-0000-0000-0000-000000000000")]
        // VeiculoController endpoints
        [InlineData("GET", "/api/cadastros/veiculos")]
        [InlineData("GET", "/api/cadastros/veiculos/00000000-0000-0000-0000-000000000000")]
        [InlineData("GET", "/api/cadastros/veiculos/placa/ABC1234")]
        [InlineData("GET", "/api/cadastros/veiculos/cliente/00000000-0000-0000-0000-000000000000")]
        [InlineData("POST", "/api/cadastros/veiculos")]
        [InlineData("PUT", "/api/cadastros/veiculos/00000000-0000-0000-0000-000000000000")]
        // EstoqueItemController endpoints
        [InlineData("GET", "/api/estoque/itens")]
        [InlineData("GET", "/api/estoque/itens/00000000-0000-0000-0000-000000000000")]
        [InlineData("POST", "/api/estoque/itens")]
        [InlineData("PUT", "/api/estoque/itens/00000000-0000-0000-0000-000000000000")]
        [InlineData("PATCH", "/api/estoque/itens/00000000-0000-0000-0000-000000000000/quantidade")]
        [InlineData("GET", "/api/estoque/itens/00000000-0000-0000-0000-000000000000/disponibilidade?quantidadeRequisitada=1")]
        // OrdemServicoController endpoints
        [InlineData("GET", "/api/ordens-servico")]
        [InlineData("GET", "/api/ordens-servico/00000000-0000-0000-0000-000000000000")]
        [InlineData("GET", "/api/ordens-servico/codigo/OS123")]
        [InlineData("POST", "/api/ordens-servico")]
        [InlineData("POST", "/api/ordens-servico/00000000-0000-0000-0000-000000000000/servicos")]
        [InlineData("POST", "/api/ordens-servico/00000000-0000-0000-0000-000000000000/itens")]
        [InlineData("DELETE", "/api/ordens-servico/00000000-0000-0000-0000-000000000000/servicos/00000000-0000-0000-0000-000000000000")]
        [InlineData("DELETE", "/api/ordens-servico/00000000-0000-0000-0000-000000000000/itens/00000000-0000-0000-0000-000000000000")]
        [InlineData("POST", "/api/ordens-servico/00000000-0000-0000-0000-000000000000/cancelar")]
        [InlineData("POST", "/api/ordens-servico/00000000-0000-0000-0000-000000000000/iniciar-diagnostico")]
        [InlineData("POST", "/api/ordens-servico/00000000-0000-0000-0000-000000000000/orcamento")]
        [InlineData("POST", "/api/ordens-servico/00000000-0000-0000-0000-000000000000/orcamento/aprovar")]
        [InlineData("POST", "/api/ordens-servico/00000000-0000-0000-0000-000000000000/orcamento/desaprovar")]
        [InlineData("POST", "/api/ordens-servico/00000000-0000-0000-0000-000000000000/finalizar-execucao")]
        [InlineData("POST", "/api/ordens-servico/00000000-0000-0000-0000-000000000000/entregar")]
        [InlineData("GET", "/api/ordens-servico/tempo-medio")]
        public async Task Endpoints_SemAutenticacao_DevemRetornarUnauthorized(string method, string url)
        {
            // Arrange
            var request = new HttpRequestMessage(new HttpMethod(method), url);

            // Para métodos como POST, PUT, PATCH, é comum precisar de um corpo na requisição, mesmo que vazio, para simular uma requisição válida.
            if (method.ToUpper() == "POST" || method.ToUpper() == "PUT" || method.ToUpper() == "PATCH")
                request.Content = JsonContent.Create(new { });

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        #endregion

        #region Endpoints que não podem ter Authorize

        [Theory]
        [InlineData("POST", "/api/authentication/token")]
        [InlineData("POST", "/api/ordens-servico/busca-publica")]
        public async Task Endpoints_ComAllowAnonymous_NaoDevemRetornarUnauthorized(string method, string url)
        {
            // Arrange
            var request = new HttpRequestMessage(new HttpMethod(method), url);

            // Para métodos como POST, PUT, PATCH, é comum precisar de um corpo na requisição, mesmo que vazio, para simular uma requisição válida.
            if (method.ToUpper() == "POST" || method.ToUpper() == "PUT" || method.ToUpper() == "PATCH")
                request.Content = JsonContent.Create(new { });

            // Act
            var response = await _client.SendAsync(request);

            // Assert
            Assert.NotEqual(HttpStatusCode.Unauthorized, response.StatusCode);
            Assert.NotEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        #endregion

        #region Método GetToken 

        [Fact(DisplayName = "GetToken deve retornar 200 OK com TokenResponseDto quando credenciais são válidas")]
        [Trait("Método", "GetToken")]
        public void GetToken_Deve_Retornar200OK_Com_TokenResponseDto_Quando_CredenciaisSaoValidas()
        {
            // Arrange
            var request = new TokenRequestDto("valid-client-id", "valid-client-secret");
            var expectedResponse = new TokenResponseDto("jwt-token-123", "Bearer", 3600);

            _authServiceMock.Setup(s => s.ValidateCredentialsAndGenerateToken(request))
                           .Returns(expectedResponse);

            // Act
            var result = _controller.GetToken(request);

            // Assert
            result.Should().NotBeNull();
            result.Result.Should().BeOfType<OkObjectResult>();

            var okResult = result.Result as OkObjectResult;
            okResult!.Value.Should().BeEquivalentTo(expectedResponse);

            _authServiceMock.Verify(s => s.ValidateCredentialsAndGenerateToken(request), Times.Once);
        }

        [Fact(DisplayName = "GetToken deve retornar 400 BadRequest quando request é inválido")]
        [Trait("Método", "GetToken")]
        public void GetToken_Deve_Retornar400BadRequest_Quando_RequestEhInvalido()
        {
            // Arrange
            var request = new TokenRequestDto("", "");
            var domainException = new DomainException("ClientId e ClientSecret requeridos.", ErrorType.InvalidInput);

            _authServiceMock.Setup(s => s.ValidateCredentialsAndGenerateToken(request))
                           .Throws(domainException);

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => _controller.GetToken(request));

            exception.Message.Should().Be("ClientId e ClientSecret requeridos.");
            exception.ErrorType.Should().Be(ErrorType.InvalidInput);

            _authServiceMock.Verify(s => s.ValidateCredentialsAndGenerateToken(request), Times.Once);
        }

        [Fact(DisplayName = "GetToken deve retornar 401 Unauthorized quando credenciais são inválidas")]
        [Trait("Método", "GetToken")]
        public void GetToken_Deve_Retornar401Unauthorized_Quando_CredenciaisSaoInvalidas()
        {
            // Arrange
            var request = new TokenRequestDto("invalid-client-id", "invalid-client-secret");
            var domainException = new DomainException("Credenciais inválidas.", ErrorType.Unauthorized);

            _authServiceMock.Setup(s => s.ValidateCredentialsAndGenerateToken(request))
                           .Throws(domainException);

            // Act & Assert
            var exception = Assert.Throws<DomainException>(() => _controller.GetToken(request));

            exception.Message.Should().Be("Credenciais inválidas.");
            exception.ErrorType.Should().Be(ErrorType.Unauthorized);

            _authServiceMock.Verify(s => s.ValidateCredentialsAndGenerateToken(request), Times.Once);
        }

        [Fact(DisplayName = "GetToken deve chamar serviço de autenticação com parâmetros corretos")]
        [Trait("Método", "GetToken")]
        public void GetToken_Deve_ChamarServicoDeAutenticacao_Com_ParametrosCorretos()
        {
            // Arrange
            var clientId = "test-client-id";
            var clientSecret = "test-client-secret";
            var request = new TokenRequestDto(clientId, clientSecret);
            var expectedResponse = new TokenResponseDto("token", "Bearer", 3600);

            _authServiceMock.Setup(s => s.ValidateCredentialsAndGenerateToken(It.IsAny<TokenRequestDto>()))
                           .Returns(expectedResponse);

            // Act
            _controller.GetToken(request);

            // Assert
            _authServiceMock.Verify(s => s.ValidateCredentialsAndGenerateToken(
                It.Is<TokenRequestDto>(r => r.ClientId == clientId && r.ClientSecret == clientSecret)),
                Times.Once);
        }

        #endregion
    }
}
