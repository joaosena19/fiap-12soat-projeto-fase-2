using Application.Cadastros.Interfaces;
using Application.Cadastros.Services;
using AutoMapper;
using Domain.Cadastros.Aggregates;
using Domain.Cadastros.Enums;
using FluentAssertions;
using Moq;
using Shared.Exceptions;
using Application;

namespace Tests.Application.Cadastros
{
    public class VeiculoServiceTest
    {
        private readonly Mock<IVeiculoRepository> _veiculoRepositoryMock;
        private readonly Mock<IClienteRepository> _clienteRepositoryMock;
        private readonly IMapper _mapper;
        private readonly VeiculoService _veiculoService;

        public VeiculoServiceTest()
        {
            _veiculoRepositoryMock = new Mock<IVeiculoRepository>();
            _clienteRepositoryMock = new Mock<IClienteRepository>();
            _mapper = AutoMapperConfig.CreateMapper();
            _veiculoService = new VeiculoService(_veiculoRepositoryMock.Object, _clienteRepositoryMock.Object, _mapper);
        }

        [Fact(DisplayName = "Deve criar veículo com dados válidos")]
        [Trait("Metodo", "CriarVeiculo")]
        public async Task CriarVeiculo_ComDadosValidos_DeveRetornarVeiculoCriado()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var placa = "ABC1234";
            var modelo = "Civic";
            var marca = "Honda";
            var cor = "Preto";
            var ano = 2020;
            var tipoVeiculo = TipoVeiculoEnum.Carro;

            var cliente = Cliente.Criar("João Silva", "12345678909");
            var veiculo = Veiculo.Criar(clienteId, placa, modelo, marca, cor, ano, tipoVeiculo);

            _clienteRepositoryMock.Setup(r => r.ObterPorIdAsync(clienteId))
                .ReturnsAsync(cliente);

            _veiculoRepositoryMock.Setup(r => r.ObterPorPlacaAsync(placa))
                .ReturnsAsync((Veiculo?)null);

            _veiculoRepositoryMock.Setup(r => r.SalvarAsync(It.IsAny<Veiculo>()))
                .ReturnsAsync(veiculo);

            // Act
            var result = await _veiculoService.CriarVeiculo(clienteId, placa, modelo, marca, cor, ano, tipoVeiculo);

            // Assert
            result.Should().NotBeNull();
            result.ClienteId.Should().Be(clienteId);
            result.Placa.Should().Be(placa);
            result.Modelo.Should().Be(modelo);
            result.Marca.Should().Be(marca);
            result.Cor.Should().Be(cor);
            result.Ano.Should().Be(ano);
            result.TipoVeiculo.Should().Be(tipoVeiculo.ToString().ToLower());

            _clienteRepositoryMock.Verify(r => r.ObterPorIdAsync(clienteId), Times.Once);
            _veiculoRepositoryMock.Verify(r => r.ObterPorPlacaAsync(placa), Times.Once);
            _veiculoRepositoryMock.Verify(r => r.SalvarAsync(It.IsAny<Veiculo>()), Times.Once);
        }

        [Fact(DisplayName = "Não deve criar veículo se placa já existir")]
        [Trait("Metodo", "CriarVeiculo")]
        public async Task CriarVeiculo_ComPlacaExistente_DeveLancarExcecao()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var placa = "ABC1234";
            var veiculoExistente = Veiculo.Criar(clienteId, placa, "Civic", "Honda", "Preto", 2020, TipoVeiculoEnum.Carro);

            _veiculoRepositoryMock.Setup(r => r.ObterPorPlacaAsync(placa))
                .ReturnsAsync(veiculoExistente);

            // Act & Assert
            await _veiculoService.Invoking(s => s.CriarVeiculo(clienteId, placa, "Corolla", "Toyota", "Branco", 2021, TipoVeiculoEnum.Carro))
                .Should().ThrowAsync<DomainException>()
                .WithMessage("Já existe um veículo cadastrado com esta placa.");

            _veiculoRepositoryMock.Verify(r => r.ObterPorPlacaAsync(placa), Times.Once);
            _veiculoRepositoryMock.Verify(r => r.SalvarAsync(It.IsAny<Veiculo>()), Times.Never);
        }

        [Fact(DisplayName = "Deve atualizar veículo com dados válidos")]
        [Trait("Metodo", "AtualizarVeiculo")]
        public async Task AtualizarVeiculo_ComDadosValidos_DeveRetornarVeiculoAtualizado()
        {
            // Arrange
            var id = Guid.NewGuid();
            var clienteId = Guid.NewGuid();
            var veiculo = Veiculo.Criar(clienteId, "ABC1234", "Civic", "Honda", "Preto", 2020, TipoVeiculoEnum.Carro);

            var novoModelo = "Corolla";
            var novaMarca = "Toyota";
            var novaCor = "Branco";
            var novoAno = 2021;
            var novoTipo = TipoVeiculoEnum.Carro;

            _veiculoRepositoryMock.Setup(r => r.ObterPorIdAsync(id))
                .ReturnsAsync(veiculo);

            _veiculoRepositoryMock.Setup(r => r.AtualizarAsync(It.IsAny<Veiculo>()))
                .ReturnsAsync(veiculo);

            // Act
            var result = await _veiculoService.AtualizarVeiculo(id, novoModelo, novaMarca, novaCor, novoAno, novoTipo);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(veiculo.Id);
            result.ClienteId.Should().Be(clienteId);

            _veiculoRepositoryMock.Verify(r => r.ObterPorIdAsync(id), Times.Once);
            _veiculoRepositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<Veiculo>()), Times.Once);
        }

        [Fact(DisplayName = "Não deve atualizar veículo se não existir")]
        [Trait("Metodo", "AtualizarVeiculo")]
        public async Task AtualizarVeiculo_ComIdInexistente_DeveLancarExcecao()
        {
            // Arrange
            var id = Guid.NewGuid();

            _veiculoRepositoryMock.Setup(r => r.ObterPorIdAsync(id))
                .ReturnsAsync((Veiculo?)null);

            // Act & Assert
            await _veiculoService.Invoking(s => s.AtualizarVeiculo(id, "Corolla", "Toyota", "Branco", 2021, TipoVeiculoEnum.Carro))
                .Should().ThrowAsync<DomainException>()
                .WithMessage("Veículo não encontrado.");

            _veiculoRepositoryMock.Verify(r => r.ObterPorIdAsync(id), Times.Once);
            _veiculoRepositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<Veiculo>()), Times.Never);
        }

        [Fact(DisplayName = "Deve buscar veículo por ID quando existir")]
        [Trait("Metodo", "BuscarPorId")]
        public async Task BuscarPorId_ComIdExistente_DeveRetornarVeiculo()
        {
            // Arrange
            var id = Guid.NewGuid();
            var clienteId = Guid.NewGuid();
            var veiculo = Veiculo.Criar(clienteId, "ABC1234", "Civic", "Honda", "Preto", 2020, TipoVeiculoEnum.Carro);

            _veiculoRepositoryMock.Setup(r => r.ObterPorIdAsync(id))
                .ReturnsAsync(veiculo);

            // Act
            var result = await _veiculoService.BuscarPorId(id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(veiculo.Id);
            result.ClienteId.Should().Be(clienteId);
            result.Placa.Should().Be(veiculo.Placa.Valor);

            _veiculoRepositoryMock.Verify(r => r.ObterPorIdAsync(id), Times.Once);
        }

        [Fact(DisplayName = "Deve lançar exceção ao buscar veículo por ID quando não existir")]
        [Trait("Metodo", "BuscarPorId")]
        public async Task BuscarPorId_ComIdInexistente_DeveLancarExcecao()
        {
            // Arrange
            var id = Guid.NewGuid();

            _veiculoRepositoryMock.Setup(r => r.ObterPorIdAsync(id))
                .ReturnsAsync((Veiculo?)null);

            // Act & Assert
            await _veiculoService.Invoking(s => s.BuscarPorId(id))
                .Should().ThrowAsync<DomainException>()
                .WithMessage("Veículo não encontrado.");

            _veiculoRepositoryMock.Verify(r => r.ObterPorIdAsync(id), Times.Once);
        }

        [Fact(DisplayName = "Deve buscar veículo por placa quando existir")]
        [Trait("Metodo", "BuscarPorPlaca")]
        public async Task BuscarPorPlaca_ComPlacaExistente_DeveRetornarVeiculo()
        {
            // Arrange
            var placa = "ABC1234";
            var clienteId = Guid.NewGuid();
            var veiculo = Veiculo.Criar(clienteId, placa, "Civic", "Honda", "Preto", 2020, TipoVeiculoEnum.Carro);

            _veiculoRepositoryMock.Setup(r => r.ObterPorPlacaAsync(placa))
                .ReturnsAsync(veiculo);

            // Act
            var result = await _veiculoService.BuscarPorPlaca(placa);

            // Assert
            result.Should().NotBeNull();
            result.ClienteId.Should().Be(clienteId);
            result.Placa.Should().Be(placa);

            _veiculoRepositoryMock.Verify(r => r.ObterPorPlacaAsync(placa), Times.Once);
        }

        [Fact(DisplayName = "Deve lançar exceção ao buscar veículo por placa quando não existir")]
        [Trait("Metodo", "BuscarPorPlaca")]
        public async Task BuscarPorPlaca_ComPlacaInexistente_DeveLancarExcecao()
        {
            // Arrange
            var placa = "ABC1234";

            _veiculoRepositoryMock.Setup(r => r.ObterPorPlacaAsync(placa))
                .ReturnsAsync((Veiculo?)null);

            // Act & Assert
            await _veiculoService.Invoking(s => s.BuscarPorPlaca(placa))
                .Should().ThrowAsync<DomainException>()
                .WithMessage("Veículo não encontrado.");

            _veiculoRepositoryMock.Verify(r => r.ObterPorPlacaAsync(placa), Times.Once);
        }

        [Fact(DisplayName = "Deve buscar todos os veículos")]
        [Trait("Metodo", "Buscar")]
        public async Task Buscar_DeveRetornarTodosVeiculos()
        {
            // Arrange
            var clienteId1 = Guid.NewGuid();
            var clienteId2 = Guid.NewGuid();
            var veiculos = new List<Veiculo>
            {
                Veiculo.Criar(clienteId1, "ABC1234", "Civic", "Honda", "Preto", 2020, TipoVeiculoEnum.Carro),
                Veiculo.Criar(clienteId2, "XYZ9876", "CBR600", "Honda", "Azul", 2021, TipoVeiculoEnum.Moto)
            };

            _veiculoRepositoryMock.Setup(r => r.ObterTodosAsync())
                .ReturnsAsync(veiculos);

            // Act
            var result = await _veiculoService.Buscar();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);

            _veiculoRepositoryMock.Verify(r => r.ObterTodosAsync(), Times.Once);
        }

        [Fact(DisplayName = "Não deve criar veículo se cliente não existir")]
        [Trait("Metodo", "CriarVeiculo")]
        public async Task CriarVeiculo_ComClienteInexistente_DeveLancarExcecao()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var placa = "ABC1234";
            var modelo = "Civic";
            var marca = "Honda";
            var cor = "Preto";
            var ano = 2020;
            var tipoVeiculo = TipoVeiculoEnum.Carro;

            _clienteRepositoryMock.Setup(r => r.ObterPorIdAsync(clienteId))
                .ReturnsAsync((Cliente?)null);

            // Act & Assert
            await _veiculoService.Invoking(s => s.CriarVeiculo(clienteId, placa, modelo, marca, cor, ano, tipoVeiculo))
                .Should().ThrowAsync<DomainException>()
                .WithMessage("*Cliente não encontrado*");

            _veiculoRepositoryMock.Verify(r => r.ObterPorPlacaAsync(It.IsAny<string>()), Times.Once);
            _clienteRepositoryMock.Verify(r => r.ObterPorIdAsync(clienteId), Times.Once);
            _veiculoRepositoryMock.Verify(r => r.SalvarAsync(It.IsAny<Veiculo>()), Times.Never);
        }

        [Fact(DisplayName = "Não deve criar veículo se placa já existir (verificação case insensitive)")]
        [Trait("Metodo", "CriarVeiculo")]
        public async Task CriarVeiculo_ComPlacaExistenteCaseInsensitive_DeveLancarExcecao()
        {
            // Arrange
            var clienteId = Guid.NewGuid();
            var placaExistente = "ABC1234"; // Placa em maiúscula no banco
            var placaNova = "abc1234"; // Tentativa de criar com minúscula
            var veiculoExistente = Veiculo.Criar(clienteId, placaExistente, "Civic", "Honda", "Preto", 2020, TipoVeiculoEnum.Carro);

            var cliente = Cliente.Criar("João Silva", "12345678909");

            _clienteRepositoryMock.Setup(r => r.ObterPorIdAsync(clienteId))
                .ReturnsAsync(cliente);

            // Simula que já existe um veículo com a placa em maiúsculas
            _veiculoRepositoryMock.Setup(r => r.ObterPorPlacaAsync(placaExistente))
                .ReturnsAsync(veiculoExistente);

            // Simula que não existe veículo com a placa nova (minúscula)
            _veiculoRepositoryMock.Setup(r => r.ObterPorPlacaAsync(placaNova))
                .ReturnsAsync((Veiculo?)null);

            // Act & Assert
            await _veiculoService.Invoking(s => s.CriarVeiculo(clienteId, placaNova, "Corolla", "Toyota", "Branco", 2021, TipoVeiculoEnum.Carro))
                .Should().ThrowAsync<DomainException>()
                .WithMessage("Já existe um veículo cadastrado com esta placa.");

            _veiculoRepositoryMock.Verify(r => r.ObterPorPlacaAsync(It.IsAny<string>()), Times.Once);
            _veiculoRepositoryMock.Verify(r => r.SalvarAsync(It.IsAny<Veiculo>()), Times.Never);
        }
    }
}
