using FluentAssertions;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using Application.Cadastros.DTO;

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

        [Fact(DisplayName = "PUT deve retornar 200 OK e atualizar Cliente existente no banco de dados.")]
        public async Task Put_Deve_Retornar200OK_E_AtualizarCliente()
        {
            // Arrange
            var criarDto = new { Nome = "João", Cpf = "12345678909" };
            var atualizarDto = new { Nome = "João Silva Atualizado" };

            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Create client first
            var createResponse = await _client.PostAsJsonAsync("/api/cadastros/clientes", criarDto);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var clienteCriado = await context.Clientes.FirstOrDefaultAsync(c => c.Cpf.Valor == "12345678909");
            clienteCriado.Should().NotBeNull();

            // Act
            var updateResponse = await _client.PutAsJsonAsync($"/api/cadastros/clientes/{clienteCriado!.Id}", atualizarDto);
            var clienteAtualizado = await context.Clientes.FirstOrDefaultAsync(c => c.Id == clienteCriado.Id);

            // Assert
            updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            clienteAtualizado.Should().NotBeNull();
            clienteAtualizado!.Nome.Valor.Should().Be("João Silva Atualizado");
            clienteAtualizado.Cpf.Valor.Should().Be("12345678909"); // CPF não deve mudar
        }

        [Fact(DisplayName = "GET deve retornar 200 OK e lista de clientes")]
        public async Task Get_Deve_Retornar200OK_E_ListaDeClientes()
        {
            // Arrange
            var cliente1 = new { Nome = "João", Cpf = "12345678901" };
            var cliente2 = new { Nome = "Maria", Cpf = "12345678902" };

            // Create test clients
            await _client.PostAsJsonAsync("/api/cadastros/clientes", cliente1);
            await _client.PostAsJsonAsync("/api/cadastros/clientes", cliente2);

            // Act
            var response = await _client.GetAsync("/api/cadastros/clientes");
            var clientes = await response.Content.ReadFromJsonAsync<IEnumerable<RetornoClienteDTO>>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            clientes.Should().NotBeNull();
            clientes.Should().HaveCountGreaterThanOrEqualTo(2);
            clientes.Should().Contain(c => c.Nome == "João" && c.Cpf == "12345678901");
            clientes.Should().Contain(c => c.Nome == "Maria" && c.Cpf == "12345678902");
        }

        [Fact(DisplayName = "GET deve retornar 200 OK mesmo quando não há clientes")]
        public async Task Get_Deve_Retornar200OK_QuandoNaoHaClientes()
        {
            // Arrange - Clear database
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Clientes.RemoveRange(context.Clientes);
            await context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/cadastros/clientes");
            var clientes = await response.Content.ReadFromJsonAsync<IEnumerable<RetornoClienteDTO>>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            clientes.Should().NotBeNull();
            clientes.Should().BeEmpty();
        }

        [Fact(DisplayName = "GET /{id} deve retornar 200 OK e cliente específico")]
        public async Task GetById_Deve_Retornar200OK_E_ClienteEspecifico()
        {
            // Arrange
            var criarDto = new { Nome = "João", Cpf = "12345678909" };
            
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Create client first
            var createResponse = await _client.PostAsJsonAsync("/api/cadastros/clientes", criarDto);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var clienteCriado = await context.Clientes.FirstOrDefaultAsync(c => c.Cpf.Valor == "12345678909");
            clienteCriado.Should().NotBeNull();

            // Act
            var response = await _client.GetAsync($"/api/cadastros/clientes/{clienteCriado!.Id}");
            var cliente = await response.Content.ReadFromJsonAsync<RetornoClienteDTO>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            cliente.Should().NotBeNull();
            cliente.Id.Should().Be(clienteCriado.Id);
            cliente.Nome.Should().Be("João");
            cliente.Cpf.Should().Be("12345678909");
        }

        [Fact(DisplayName = "GET /{id} deve retornar 404 NotFound quando cliente não existe")]
        public async Task GetById_Deve_Retornar404NotFound_QuandoClienteNaoExiste()
        {
            // Arrange
            var idInexistente = Guid.NewGuid();

            // Act
            var response = await _client.GetAsync($"/api/cadastros/clientes/{idInexistente}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}