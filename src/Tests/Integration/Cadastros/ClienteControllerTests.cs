using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using Application.Cadastros.DTO;
using Infrastructure.Database;

namespace Tests.Integration.Cadastros
{
    public class ClienteControllerTests : IClassFixture<TestWebApplicationFactory<Program>>
    {
        private readonly TestWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public ClienteControllerTests(TestWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact(DisplayName = "POST deve retornar 201 Created e persistir novo Cliente no banco de dados.")]
        [Trait("Metodo", "Post")]
        public async Task Post_Deve_Retornar201Created_E_PersistirCliente()
        {
            // Arrange
            var dto = new { Nome = "João", Cpf = "49622601030" };
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Act
            var response = await _client.PostAsJsonAsync("/api/cadastros/clientes", dto);
            var clientEntity = await context.Clientes.FirstOrDefaultAsync(c => c.Cpf.Valor == "49622601030");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            clientEntity.Should().NotBeNull();
            clientEntity.Nome.Valor.Should().Be("João");
            clientEntity.Cpf.Valor.Should().Be("49622601030");
        }

        [Fact(DisplayName = "PUT deve retornar 200 OK e atualizar Cliente existente no banco de dados.")]
        [Trait("Metodo", "Put")]
        public async Task Put_Deve_Retornar200OK_E_AtualizarCliente()
        {
            // Arrange
            var criarDto = new { Nome = "João", Cpf = "42103574052" };
            var atualizarDto = new { Nome = "João Silva Atualizado" };

            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Create client first
            var createResponse = await _client.PostAsJsonAsync("/api/cadastros/clientes", criarDto);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var clienteCriado = await context.Clientes.FirstOrDefaultAsync(c => c.Cpf.Valor == "42103574052");
            clienteCriado.Should().NotBeNull();

            // Act
            var updateResponse = await _client.PutAsJsonAsync($"/api/cadastros/clientes/{clienteCriado!.Id}", atualizarDto);
            
            // Limpa o tracking do EF Core
            context.ChangeTracker.Clear();
            var clienteAtualizado = await context.Clientes.FirstOrDefaultAsync(c => c.Id == clienteCriado.Id);

            // Assert
            updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            clienteAtualizado.Should().NotBeNull();
            clienteAtualizado!.Nome.Valor.Should().Be("João Silva Atualizado");
            clienteAtualizado.Cpf.Valor.Should().Be("42103574052"); // CPF não deve mudar
        }

        [Fact(DisplayName = "GET deve retornar 200 OK e lista de clientes")]
        [Trait("Metodo", "Get")]
        public async Task Get_Deve_Retornar200OK_E_ListaDeClientes()
        {
            // Arrange
            var cliente1 = new { Nome = "João", Cpf = "12345678909" };
            var cliente2 = new { Nome = "Maria", Cpf = "84405205060" };

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
            clientes.Should().Contain(c => c.Nome == "João" && c.Cpf == "12345678909");
            clientes.Should().Contain(c => c.Nome == "Maria" && c.Cpf == "84405205060");
        }

        [Fact(DisplayName = "GET deve retornar 200 OK mesmo quando não há clientes")]
        [Trait("Metodo", "Get")]
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
        [Trait("Metodo", "GetById")]
        public async Task GetById_Deve_Retornar200OK_E_ClienteEspecifico()
        {
            // Arrange
            var criarDto = new { Nome = "João", Cpf = "56227045020" };
            
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Create client first
            var createResponse = await _client.PostAsJsonAsync("/api/cadastros/clientes", criarDto);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var clienteCriado = await context.Clientes.FirstOrDefaultAsync(c => c.Cpf.Valor == "56227045020");
            clienteCriado.Should().NotBeNull();

            // Act
            var response = await _client.GetAsync($"/api/cadastros/clientes/{clienteCriado!.Id}");
            var cliente = await response.Content.ReadFromJsonAsync<RetornoClienteDTO>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            cliente.Should().NotBeNull();
            cliente.Id.Should().Be(clienteCriado.Id);
            cliente.Nome.Should().Be("João");
            cliente.Cpf.Should().Be("56227045020");
        }

        [Fact(DisplayName = "GET /{id} deve retornar 404 NotFound quando cliente não existe")]
        [Trait("Metodo", "GetById")]
        public async Task GetById_Deve_Retornar404NotFound_QuandoClienteNaoExiste()
        {
            // Arrange
            var idInexistente = Guid.NewGuid();

            // Act
            var response = await _client.GetAsync($"/api/cadastros/clientes/{idInexistente}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact(DisplayName = "GET /cpf/{cpf} deve retornar 200 OK e cliente específico")]
        [Trait("Metodo", "GetByCpf")]
        public async Task GetByCpf_Deve_Retornar200OK_E_ClienteEspecifico()
        {
            // Arrange
            var criarDto = new { Nome = "João", Cpf = "34806653063" };
            
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Create client first
            var createResponse = await _client.PostAsJsonAsync("/api/cadastros/clientes", criarDto);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var clienteCriado = await context.Clientes.FirstOrDefaultAsync(c => c.Cpf.Valor == "34806653063");
            clienteCriado.Should().NotBeNull();

            // Act
            var response = await _client.GetAsync($"/api/cadastros/clientes/cpf/34806653063");
            var cliente = await response.Content.ReadFromJsonAsync<RetornoClienteDTO>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            cliente.Should().NotBeNull();
            cliente.Id.Should().Be(clienteCriado!.Id);
            cliente.Nome.Should().Be("João");
            cliente.Cpf.Should().Be("34806653063");
        }

        [Fact(DisplayName = "GET /cpf/{cpf} deve retornar 404 NotFound quando cliente não existe")]
        [Trait("Metodo", "GetByCpf")]
        public async Task GetByCpf_Deve_Retornar404NotFound_QuandoClienteNaoExiste()
        {
            // Arrange
            var cpfInexistente = "99999999999";

            // Act
            var response = await _client.GetAsync($"/api/cadastros/clientes/cpf/{cpfInexistente}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}