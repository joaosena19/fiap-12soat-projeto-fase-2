using Application.Cadastros.DTO;
using Domain.Cadastros.Enums;
using FluentAssertions;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;

namespace Tests.Integration.Cadastros
{
    public class CadastroVeiculoControllerTests : IClassFixture<TestWebApplicationFactory<Program>>
    {
        private readonly TestWebApplicationFactory<Program> _factory;
        private readonly HttpClient _client;

        public CadastroVeiculoControllerTests(TestWebApplicationFactory<Program> factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        [Fact(DisplayName = "POST deve retornar 201 Created e persistir novo Veículo no banco de dados.")]
        [Trait("Metodo", "Post")]
        public async Task Post_Deve_Retornar201Created_E_PersistirVeiculo()
        {
            // Arrange
            var dto = new 
            { 
                Placa = "ABC1234", 
                Modelo = "Civic", 
                Marca = "Honda", 
                Cor = "Preto", 
                Ano = 2020, 
                TipoVeiculo = (int)TipoVeiculoEnum.Carro 
            };
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Act
            var response = await _client.PostAsJsonAsync("/api/cadastros/veiculos", dto);
            var veiculoEntity = await context.Veiculos.FirstOrDefaultAsync(v => v.Placa.Valor == "ABC1234");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            veiculoEntity.Should().NotBeNull();
            veiculoEntity!.Modelo.Valor.Should().Be("Civic");
            veiculoEntity.Marca.Valor.Should().Be("Honda");
            veiculoEntity.Cor.Valor.Should().Be("Preto");
            veiculoEntity.Ano.Valor.Should().Be(2020);
            veiculoEntity.TipoVeiculo.Valor.Should().Be(TipoVeiculoEnum.Carro.ToString().ToLower());
        }

        [Fact(DisplayName = "PUT deve retornar 200 OK e atualizar Veículo existente no banco de dados.")]
        [Trait("Metodo", "Put")]
        public async Task Put_Deve_Retornar200OK_E_AtualizarVeiculo()
        {
            // Arrange
            var criarDto = new 
            { 
                Placa = "XYZ5678", 
                Modelo = "Corolla", 
                Marca = "Toyota", 
                Cor = "Branco", 
                Ano = 2021, 
                TipoVeiculo = (int)TipoVeiculoEnum.Carro 
            };
            var atualizarDto = new 
            { 
                Modelo = "Corolla Cross", 
                Marca = "Toyota", 
                Cor = "Prata", 
                Ano = 2022, 
                TipoVeiculo = (int)TipoVeiculoEnum.Carro 
            };

            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Create vehicle first
            var createResponse = await _client.PostAsJsonAsync("/api/cadastros/veiculos", criarDto);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var veiculoCriado = await context.Veiculos.FirstOrDefaultAsync(v => v.Placa.Valor == "XYZ5678");
            veiculoCriado.Should().NotBeNull();

            // Act
            var updateResponse = await _client.PutAsJsonAsync($"/api/cadastros/veiculos/{veiculoCriado!.Id}", atualizarDto);
            
            // Limpa o tracking do EF Core
            context.ChangeTracker.Clear();
            var veiculoAtualizado = await context.Veiculos.FirstOrDefaultAsync(v => v.Id == veiculoCriado.Id);

            // Assert
            updateResponse.StatusCode.Should().Be(HttpStatusCode.OK);
            veiculoAtualizado.Should().NotBeNull();
            veiculoAtualizado!.Modelo.Valor.Should().Be("Corolla Cross");
            veiculoAtualizado.Cor.Valor.Should().Be("Prata");
            veiculoAtualizado.Ano.Valor.Should().Be(2022);
            veiculoAtualizado.Placa.Valor.Should().Be("XYZ5678"); // Placa não deve mudar
        }

        [Fact(DisplayName = "GET deve retornar 200 OK e lista de veículos")]
        [Trait("Metodo", "Get")]
        public async Task Get_Deve_Retornar200OK_E_ListaDeVeiculos()
        {
            // Arrange
            var veiculo1 = new 
            { 
                Placa = "GET0001", 
                Modelo = "Civic", 
                Marca = "Honda", 
                Cor = "Azul", 
                Ano = 2020, 
                TipoVeiculo = (int)TipoVeiculoEnum.Carro 
            };
            var veiculo2 = new 
            { 
                Placa = "GET0002", 
                Modelo = "CBR600", 
                Marca = "Honda", 
                Cor = "Vermelho", 
                Ano = 2021, 
                TipoVeiculo = (int)TipoVeiculoEnum.Moto 
            };

            // Create test vehicles
            await _client.PostAsJsonAsync("/api/cadastros/veiculos", veiculo1);
            await _client.PostAsJsonAsync("/api/cadastros/veiculos", veiculo2);

            // Act
            var response = await _client.GetAsync("/api/cadastros/veiculos");
            var veiculos = await response.Content.ReadFromJsonAsync<IEnumerable<RetornoVeiculoDTO>>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            veiculos.Should().NotBeNull();
            veiculos.Should().HaveCountGreaterThanOrEqualTo(2);
            veiculos.Should().Contain(v => v.Modelo == "Civic" && v.Placa == "GET0001");
            veiculos.Should().Contain(v => v.Modelo == "CBR600" && v.Placa == "GET0002");
        }

        [Fact(DisplayName = "GET deve retornar 200 OK mesmo quando não há veículos")]
        [Trait("Metodo", "Get")]
        public async Task Get_Deve_Retornar200OK_QuandoNaoHaVeiculos()
        {
            // Arrange - Clear database
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            context.Veiculos.RemoveRange(context.Veiculos);
            await context.SaveChangesAsync();

            // Act
            var response = await _client.GetAsync("/api/cadastros/veiculos");
            var veiculos = await response.Content.ReadFromJsonAsync<IEnumerable<RetornoVeiculoDTO>>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            veiculos.Should().NotBeNull();
            veiculos.Should().BeEmpty();
        }

        [Fact(DisplayName = "GET /{id} deve retornar 200 OK e veículo específico")]
        [Trait("Metodo", "GetById")]
        public async Task GetById_Deve_Retornar200OK_E_VeiculoEspecifico()
        {
            // Arrange
            var criarDto = new 
            { 
                Placa = "GID0001", 
                Modelo = "Fit", 
                Marca = "Honda", 
                Cor = "Branco", 
                Ano = 2019, 
                TipoVeiculo = (int)TipoVeiculoEnum.Carro 
            };
            
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Create vehicle first
            var createResponse = await _client.PostAsJsonAsync("/api/cadastros/veiculos", criarDto);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var veiculoCriado = await context.Veiculos.FirstOrDefaultAsync(v => v.Placa.Valor == "GID0001");
            veiculoCriado.Should().NotBeNull();

            // Act
            var response = await _client.GetAsync($"/api/cadastros/veiculos/{veiculoCriado!.Id}");
            var veiculo = await response.Content.ReadFromJsonAsync<RetornoVeiculoDTO>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            veiculo.Should().NotBeNull();
            veiculo!.Id.Should().Be(veiculoCriado.Id);
            veiculo.Modelo.Should().Be("Fit");
            veiculo.Placa.Should().Be("GID0001");
        }

        [Fact(DisplayName = "GET /{id} deve retornar 404 NotFound quando veículo não existe")]
        [Trait("Metodo", "GetById")]
        public async Task GetById_Deve_Retornar404NotFound_QuandoVeiculoNaoExiste()
        {
            // Arrange
            var idInexistente = Guid.NewGuid();

            // Act
            var response = await _client.GetAsync($"/api/cadastros/veiculos/{idInexistente}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        [Fact(DisplayName = "GET /placa/{placa} deve retornar 200 OK e veículo específico")]
        [Trait("Metodo", "GetByPlaca")]
        public async Task GetByPlaca_Deve_Retornar200OK_E_VeiculoEspecifico()
        {
            // Arrange
            var criarDto = new
            {
                Placa = "GPL0001",
                Modelo = "Onix",
                Marca = "Chevrolet",
                Cor = "Prata",
                Ano = 2022,
                TipoVeiculo = TipoVeiculoEnum.Carro.ToString()
            };
            
            using var scope = _factory.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // Create vehicle first
            var createResponse = await _client.PostAsJsonAsync("/api/cadastros/veiculos", criarDto);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var veiculoCriado = await context.Veiculos.FirstOrDefaultAsync(v => v.Placa.Valor == "GPL0001");
            veiculoCriado.Should().NotBeNull();

            // Act
            var response = await _client.GetAsync($"/api/cadastros/veiculos/placa/GPL0001");
            var veiculo = await response.Content.ReadFromJsonAsync<RetornoVeiculoDTO>();

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            veiculo.Should().NotBeNull();
            veiculo!.Id.Should().Be(veiculoCriado!.Id);
            veiculo.Modelo.Should().Be("Onix");
            veiculo.Placa.Should().Be("GPL0001");
        }

        [Fact(DisplayName = "GET /placa/{placa} deve retornar 404 NotFound quando veículo não existe")]
        [Trait("Metodo", "GetByPlaca")]
        public async Task GetByPlaca_Deve_Retornar404NotFound_QuandoVeiculoNaoExiste()
        {
            // Arrange
            var placaInexistente = "XXX9999";

            // Act
            var response = await _client.GetAsync($"/api/cadastros/veiculos/placa/{placaInexistente}");

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
