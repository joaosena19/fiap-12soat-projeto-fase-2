using Application.Auth.DTO;
using Application.Auth.Interfaces;
using Application.Auth.Services;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;

namespace Tests.Application.Auth
{
    public class AuthServiceTests
    {
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<ITokenService> _tokenServiceMock;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _configurationMock = new Mock<IConfiguration>();
            _tokenServiceMock = new Mock<ITokenService>();
            _authService = new AuthService(_configurationMock.Object, _tokenServiceMock.Object);
        }

        #region Testes Método ValidateCredentialsAndGenerateToken

        [Fact(DisplayName = "Deve retornar token quando credenciais são válidas")]
        [Trait("Metodo", "ValidateCredentialsAndGenerateToken")]
        public void ValidateCredentialsAndGenerateToken_DeveRetornarToken_QuandoCredenciaisSaoValidas()
        {
            // Arrange
            const string clientId = "test-client-id";
            const string clientSecret = "test-client-secret";
            const string expectedToken = "fake-jwt-token";

            _configurationMock.Setup(c => c["ApiCredentials:ClientId"]).Returns(clientId);
            _configurationMock.Setup(c => c["ApiCredentials:ClientSecret"]).Returns(clientSecret);
            _tokenServiceMock.Setup(t => t.GenerateToken(clientId)).Returns(expectedToken);

            var request = new TokenRequestDto(clientId, clientSecret);

            // Act
            var result = _authService.ValidateCredentialsAndGenerateToken(request);

            // Assert
            result.Should().NotBeNull();
            result.Token.Should().Be(expectedToken);
            _tokenServiceMock.Verify(t => t.GenerateToken(clientId), Times.Once);
        }

        [Fact(DisplayName = "Deve retornar null quando ClientId é inválido")]
        [Trait("Metodo", "ValidateCredentialsAndGenerateToken")]
        public void ValidateCredentialsAndGenerateToken_DeveRetornarNull_QuandoClientIdEhInvalido()
        {
            // Arrange
            const string correctClientId = "correct-client-id";
            const string wrongClientId = "wrong-client-id";
            const string clientSecret = "test-client-secret";

            _configurationMock.Setup(c => c["ApiCredentials:ClientId"]).Returns(correctClientId);
            _configurationMock.Setup(c => c["ApiCredentials:ClientSecret"]).Returns(clientSecret);

            var request = new TokenRequestDto(wrongClientId, clientSecret);

            // Act
            var result = _authService.ValidateCredentialsAndGenerateToken(request);

            // Assert
            result.Should().BeNull();
            _tokenServiceMock.Verify(t => t.GenerateToken(It.IsAny<string>()), Times.Never);
        }

        [Fact(DisplayName = "Deve retornar null quando ClientSecret é inválido")]
        [Trait("Metodo", "ValidateCredentialsAndGenerateToken")]
        public void ValidateCredentialsAndGenerateToken_DeveRetornarNull_QuandoClientSecretEhInvalido()
        {
            // Arrange
            const string clientId = "test-client-id";
            const string correctClientSecret = "correct-client-secret";
            const string wrongClientSecret = "wrong-client-secret";

            _configurationMock.Setup(c => c["ApiCredentials:ClientId"]).Returns(clientId);
            _configurationMock.Setup(c => c["ApiCredentials:ClientSecret"]).Returns(correctClientSecret);

            var request = new TokenRequestDto(clientId, wrongClientSecret);

            // Act
            var result = _authService.ValidateCredentialsAndGenerateToken(request);

            // Assert
            result.Should().BeNull();
            _tokenServiceMock.Verify(t => t.GenerateToken(It.IsAny<string>()), Times.Never);
        }

        [Fact(DisplayName = "Deve retornar null quando configuração ClientId é nula")]
        [Trait("Metodo", "ValidateCredentialsAndGenerateToken")]
        public void ValidateCredentialsAndGenerateToken_DeveRetornarNull_QuandoConfiguracaoClientIdEhNula()
        {
            // Arrange
            const string clientId = "test-client-id";
            const string clientSecret = "test-client-secret";

            _configurationMock.Setup(c => c["ApiCredentials:ClientId"]).Returns((string?)null);
            _configurationMock.Setup(c => c["ApiCredentials:ClientSecret"]).Returns(clientSecret);

            var request = new TokenRequestDto(clientId, clientSecret);

            // Act
            var result = _authService.ValidateCredentialsAndGenerateToken(request);

            // Assert
            result.Should().BeNull();
            _tokenServiceMock.Verify(t => t.GenerateToken(It.IsAny<string>()), Times.Never);
        }

        [Fact(DisplayName = "Deve retornar null quando configuração ClientSecret é nula")]
        [Trait("Metodo", "ValidateCredentialsAndGenerateToken")]
        public void ValidateCredentialsAndGenerateToken_DeveRetornarNull_QuandoConfiguracaoClientSecretEhNula()
        {
            // Arrange
            const string clientId = "test-client-id";
            const string clientSecret = "test-client-secret";

            _configurationMock.Setup(c => c["ApiCredentials:ClientId"]).Returns(clientId);
            _configurationMock.Setup(c => c["ApiCredentials:ClientSecret"]).Returns((string?)null);

            var request = new TokenRequestDto(clientId, clientSecret);

            // Act
            var result = _authService.ValidateCredentialsAndGenerateToken(request);

            // Assert
            result.Should().BeNull();
            _tokenServiceMock.Verify(t => t.GenerateToken(It.IsAny<string>()), Times.Never);
        }

        [Fact(DisplayName = "Deve retornar null quando configuração ClientId é vazia")]
        [Trait("Metodo", "ValidateCredentialsAndGenerateToken")]
        public void ValidateCredentialsAndGenerateToken_DeveRetornarNull_QuandoConfiguracaoClientIdEhVazia()
        {
            // Arrange
            const string clientId = "test-client-id";
            const string clientSecret = "test-client-secret";

            _configurationMock.Setup(c => c["ApiCredentials:ClientId"]).Returns(string.Empty);
            _configurationMock.Setup(c => c["ApiCredentials:ClientSecret"]).Returns(clientSecret);

            var request = new TokenRequestDto(clientId, clientSecret);

            // Act
            var result = _authService.ValidateCredentialsAndGenerateToken(request);

            // Assert
            result.Should().BeNull();
            _tokenServiceMock.Verify(t => t.GenerateToken(It.IsAny<string>()), Times.Never);
        }

        [Fact(DisplayName = "Deve retornar null quando configuração ClientSecret é vazia")]
        [Trait("Metodo", "ValidateCredentialsAndGenerateToken")]
        public void ValidateCredentialsAndGenerateToken_DeveRetornarNull_QuandoConfiguracaoClientSecretEhVazia()
        {
            // Arrange
            const string clientId = "test-client-id";
            const string clientSecret = "test-client-secret";

            _configurationMock.Setup(c => c["ApiCredentials:ClientId"]).Returns(clientId);
            _configurationMock.Setup(c => c["ApiCredentials:ClientSecret"]).Returns(string.Empty);

            var request = new TokenRequestDto(clientId, clientSecret);

            // Act
            var result = _authService.ValidateCredentialsAndGenerateToken(request);

            // Assert
            result.Should().BeNull();
            _tokenServiceMock.Verify(t => t.GenerateToken(It.IsAny<string>()), Times.Never);
        }

        #endregion
    }
}
