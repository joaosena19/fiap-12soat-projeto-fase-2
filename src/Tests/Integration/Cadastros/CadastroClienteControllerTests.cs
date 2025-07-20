using FluentAssertions;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace Tests.Integration.Cadastros
{
    public class CadastroClienteControllerTests : IClassFixture<TestWebApplicationFactory<Program>>
    {
        private readonly TestWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public CadastroClienteControllerTests(TestWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact(DisplayName = "POST deve retornar 201 Created e persistir novo Cliente no banco de dados.")]
        public async Task Post_Deve_Retornar201Created_E_PersistirCliente()
        {
            // Arrange
            var dto = new { Nome = "João", Cpf = "12345678909" };
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Act
            var response = await _client.PostAsJsonAsync("/api/cadastros/clientes", dto);
            var clientEntity = await context.Clientes.FirstOrDefaultAsync(c => c.Cpf.Valor == "12345678909");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            clientEntity.Should().NotBeNull();
            clientEntity.Nome.Valor.Should().Be("João");
            clientEntity.Cpf.Valor.Should().Be("12345678909");
        }
    }
}