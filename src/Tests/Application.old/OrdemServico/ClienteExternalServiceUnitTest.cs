using Application.Cadastros.Interfaces;
using Application.OrdemServico.Dtos.External;
using Domain.Cadastros.Aggregates;
using Domain.Cadastros.Enums;
using FluentAssertions;
using Infrastructure.AntiCorruptionLayer.OrdemServico;
using Moq;

namespace Tests.Application.OrdemServico
{
    public class ClienteExternalServiceUnitTest
    {
        private readonly Mock<IVeiculoRepository> _veiculoRepositoryMock;
        private readonly Mock<IClienteRepository> _clienteRepositoryMock;
        private readonly ClienteExternalService _service;

        public ClienteExternalServiceUnitTest()
        {
            _veiculoRepositoryMock = new Mock<IVeiculoRepository>();
            _clienteRepositoryMock = new Mock<IClienteRepository>();
            _service = new ClienteExternalService(_veiculoRepositoryMock.Object, _clienteRepositoryMock.Object);
        }

        #region Método ObterClientePorVeiculoIdAsync Tests

        [Fact(DisplayName = "Deve retornar cliente quando veículo e cliente existirem")]
        [Trait("Método", "ObterClientePorVeiculoIdAsync")]
        public async Task ObterClientePorVeiculoIdAsync_DeveRetornarCliente_QuandoVeiculoEClienteExistirem()
        {
            // Arrange
            var veiculoId = Guid.NewGuid();
            var clienteId = Guid.NewGuid();
            var nomeCliente = "João da Silva";
            var cpfCliente = "12345678909";

            var cliente = Cliente.Criar(nomeCliente, cpfCliente);
            var veiculo = Veiculo.Criar(clienteId, "ABC1234", "Corolla", "Toyota", "Prata", 2020, TipoVeiculoEnum.Carro);

            _veiculoRepositoryMock.Setup(r => r.ObterPorIdAsync(veiculoId))
                .ReturnsAsync(veiculo);

            _clienteRepositoryMock.Setup(r => r.ObterPorIdAsync(clienteId))
                .ReturnsAsync(cliente);

            // Act
            var resultado = await _service.ObterClientePorVeiculoIdAsync(veiculoId);

            // Assert
            resultado.Should().NotBeNull();
            resultado!.Id.Should().Be(cliente.Id);
            resultado.Nome.Should().Be(nomeCliente);
            resultado.DocumentoIdentificador.Should().Be(cpfCliente);

            _veiculoRepositoryMock.Verify(r => r.ObterPorIdAsync(veiculoId), Times.Once);
            _clienteRepositoryMock.Verify(r => r.ObterPorIdAsync(clienteId), Times.Once);
        }

        [Fact(DisplayName = "Deve retornar null quando veículo não existir")]
        [Trait("Método", "ObterClientePorVeiculoIdAsync")]
        public async Task ObterClientePorVeiculoIdAsync_DeveRetornarNull_QuandoVeiculoNaoExistir()
        {
            // Arrange
            var veiculoId = Guid.NewGuid();

            _veiculoRepositoryMock.Setup(r => r.ObterPorIdAsync(veiculoId))
                .ReturnsAsync((Veiculo?)null);

            // Act
            var resultado = await _service.ObterClientePorVeiculoIdAsync(veiculoId);

            // Assert
            resultado.Should().BeNull();

            _veiculoRepositoryMock.Verify(r => r.ObterPorIdAsync(veiculoId), Times.Once);
            _clienteRepositoryMock.Verify(r => r.ObterPorIdAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact(DisplayName = "Deve retornar null quando cliente não existir")]
        [Trait("Método", "ObterClientePorVeiculoIdAsync")]
        public async Task ObterClientePorVeiculoIdAsync_DeveRetornarNull_QuandoClienteNaoExistir()
        {
            // Arrange
            var veiculoId = Guid.NewGuid();
            var clienteId = Guid.NewGuid();

            var veiculo = Veiculo.Criar(clienteId, "ABC1234", "Corolla", "Toyota", "Azul", 2020, TipoVeiculoEnum.Carro);

            _veiculoRepositoryMock.Setup(r => r.ObterPorIdAsync(veiculoId))
                .ReturnsAsync(veiculo);

            _clienteRepositoryMock.Setup(r => r.ObterPorIdAsync(clienteId))
                .ReturnsAsync((Cliente?)null);

            // Act
            var resultado = await _service.ObterClientePorVeiculoIdAsync(veiculoId);

            // Assert
            resultado.Should().BeNull();

            _veiculoRepositoryMock.Verify(r => r.ObterPorIdAsync(veiculoId), Times.Once);
            _clienteRepositoryMock.Verify(r => r.ObterPorIdAsync(clienteId), Times.Once);
        }

        [Fact(DisplayName = "Deve mapear corretamente os dados do cliente para o DTO")]
        [Trait("Método", "ObterClientePorVeiculoIdAsync")]
        public async Task ObterClientePorVeiculoIdAsync_DeveMappearCorretamente_DadosDoClienteParaDTO()
        {
            // Arrange
            var veiculoId = Guid.NewGuid();
            var clienteId = Guid.NewGuid();
            var nomeCliente = "Maria Santos";
            var cnpjCliente = "12345678000195";

            var cliente = Cliente.Criar(nomeCliente, cnpjCliente);
            var veiculo = Veiculo.Criar(clienteId, "XYZ9876", "Civic", "Honda", "Branco", 2021, TipoVeiculoEnum.Carro);

            _veiculoRepositoryMock.Setup(r => r.ObterPorIdAsync(veiculoId))
                .ReturnsAsync(veiculo);

            _clienteRepositoryMock.Setup(r => r.ObterPorIdAsync(clienteId))
                .ReturnsAsync(cliente);

            // Act
            var resultado = await _service.ObterClientePorVeiculoIdAsync(veiculoId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeOfType<ClienteExternalDto>();
            resultado!.Id.Should().Be(cliente.Id);
            resultado.Nome.Should().Be(cliente.Nome.Valor);
            resultado.DocumentoIdentificador.Should().Be(cliente.DocumentoIdentificador.Valor);
        }

        #endregion
    }
}
