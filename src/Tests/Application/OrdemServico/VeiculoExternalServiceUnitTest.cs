using Application.Cadastros.Interfaces;
using Domain.Cadastros.Aggregates;
using Domain.Cadastros.Enums;
using FluentAssertions;
using Infrastructure.AntiCorruptionLayer.OrdemServico;
using Moq;

namespace Tests.Application.OrdemServico
{
    public class VeiculoExternalServiceUnitTest
    {
        private readonly Mock<IVeiculoRepository> _veiculoRepositoryMock;
        private readonly VeiculoExternalService _service;

        public VeiculoExternalServiceUnitTest()
        {
            _veiculoRepositoryMock = new Mock<IVeiculoRepository>();
            _service = new VeiculoExternalService(_veiculoRepositoryMock.Object);
        }

        #region Método VerificarExistenciaVeiculo Tests

        [Fact(DisplayName = "Deve retornar true quando veículo existir")]
        [Trait("Método", "VerificarExistenciaVeiculo")]
        public async Task VerificarExistenciaVeiculo_DeveRetornarTrue_QuandoVeiculoExistir()
        {
            // Arrange
            var veiculoId = Guid.NewGuid();
            var clienteId = Guid.NewGuid();
            var veiculo = Veiculo.Criar(clienteId, "ABC1234", "Corolla", "Toyota", "Prata", 2020, TipoVeiculoEnum.Carro);

            _veiculoRepositoryMock.Setup(r => r.ObterPorIdAsync(veiculoId))
                .ReturnsAsync(veiculo);

            // Act
            var resultado = await _service.VerificarExistenciaVeiculo(veiculoId);

            // Assert
            resultado.Should().BeTrue();

            _veiculoRepositoryMock.Verify(r => r.ObterPorIdAsync(veiculoId), Times.Once);
        }

        [Fact(DisplayName = "Deve retornar false quando veículo não existir")]
        [Trait("Método", "VerificarExistenciaVeiculo")]
        public async Task VerificarExistenciaVeiculo_DeveRetornarFalse_QuandoVeiculoNaoExistir()
        {
            // Arrange
            var veiculoId = Guid.NewGuid();

            _veiculoRepositoryMock.Setup(r => r.ObterPorIdAsync(veiculoId))
                .ReturnsAsync((Veiculo?)null);

            // Act
            var resultado = await _service.VerificarExistenciaVeiculo(veiculoId);

            // Assert
            resultado.Should().BeFalse();

            _veiculoRepositoryMock.Verify(r => r.ObterPorIdAsync(veiculoId), Times.Once);
            _veiculoRepositoryMock.VerifyNoOtherCalls();
        }

        [Fact(DisplayName = "Deve usar o ID correto na chamada do repositório")]
        [Trait("Método", "VerificarExistenciaVeiculo")]
        public async Task VerificarExistenciaVeiculo_DeveUsarIdCorreto_NaChamadaDoRepositorio()
        {
            // Arrange
            var veiculoIdEsperado = Guid.NewGuid();
            var clienteId = Guid.NewGuid();
            var veiculo = Veiculo.Criar(clienteId, "LMN3579", "Fiesta", "Ford", "Vermelho", 2018, TipoVeiculoEnum.Carro);

            _veiculoRepositoryMock.Setup(r => r.ObterPorIdAsync(veiculoIdEsperado))
                .ReturnsAsync(veiculo);

            // Act
            await _service.VerificarExistenciaVeiculo(veiculoIdEsperado);

            // Assert
            _veiculoRepositoryMock.Verify(r => r.ObterPorIdAsync(veiculoIdEsperado), Times.Once);
        }

        #endregion
    }
}
