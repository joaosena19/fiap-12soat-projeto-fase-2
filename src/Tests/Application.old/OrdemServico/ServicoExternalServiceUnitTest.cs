using Application.Cadastros.Interfaces;
using Application.OrdemServico.Dtos.External;
using Domain.Cadastros.Aggregates;
using FluentAssertions;
using Infrastructure.AntiCorruptionLayer.OrdemServico;
using Moq;

namespace Tests.Application.OrdemServico
{
    public class ServicoExternalServiceUnitTest
    {
        private readonly Mock<IServicoRepository> _servicoRepositoryMock;
        private readonly ServicoExternalService _service;

        public ServicoExternalServiceUnitTest()
        {
            _servicoRepositoryMock = new Mock<IServicoRepository>();
            _service = new ServicoExternalService(_servicoRepositoryMock.Object);
        }

        #region Método ObterServicoPorIdAsync Tests

        [Fact(DisplayName = "Deve retornar serviço quando existir")]
        [Trait("Método", "ObterServicoPorIdAsync")]
        public async Task ObterServicoPorIdAsync_DeveRetornarServico_QuandoExistir()
        {
            // Arrange
            var servicoId = Guid.NewGuid();
            var nomeServico = "Troca de Óleo";
            var precoServico = 50.00m;

            var servico = Servico.Criar(nomeServico, precoServico);

            _servicoRepositoryMock.Setup(r => r.ObterPorIdAsync(servicoId))
                .ReturnsAsync(servico);

            // Act
            var resultado = await _service.ObterServicoPorIdAsync(servicoId);

            // Assert
            resultado.Should().NotBeNull();
            resultado!.Id.Should().Be(servico.Id);
            resultado.Nome.Should().Be(nomeServico);
            resultado.Preco.Should().Be(precoServico);

            _servicoRepositoryMock.Verify(r => r.ObterPorIdAsync(servicoId), Times.Once);
        }

        [Fact(DisplayName = "Deve retornar null quando serviço não existir")]
        [Trait("Método", "ObterServicoPorIdAsync")]
        public async Task ObterServicoPorIdAsync_DeveRetornarNull_QuandoServicoNaoExistir()
        {
            // Arrange
            var servicoId = Guid.NewGuid();

            _servicoRepositoryMock.Setup(r => r.ObterPorIdAsync(servicoId))
                .ReturnsAsync((Servico?)null);

            // Act
            var resultado = await _service.ObterServicoPorIdAsync(servicoId);

            // Assert
            resultado.Should().BeNull();

            _servicoRepositoryMock.Verify(r => r.ObterPorIdAsync(servicoId), Times.Once);
        }

        [Fact(DisplayName = "Deve mapear corretamente os dados do serviço para o DTO")]
        [Trait("Método", "ObterServicoPorIdAsync")]
        public async Task ObterServicoPorIdAsync_DeveMappearCorretamente_DadosDoServicoParaDTO()
        {
            // Arrange
            var servicoId = Guid.NewGuid();
            var nomeEsperado = "Alinhamento e Balanceamento";
            var precoEsperado = 120.00m;

            var servico = Servico.Criar(nomeEsperado, precoEsperado);

            _servicoRepositoryMock.Setup(r => r.ObterPorIdAsync(servicoId))
                .ReturnsAsync(servico);

            // Act
            var resultado = await _service.ObterServicoPorIdAsync(servicoId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeOfType<ServicoExternalDto>();
            resultado!.Id.Should().Be(servico.Id);
            resultado.Nome.Should().Be(servico.Nome.Valor);
            resultado.Preco.Should().Be(servico.Preco.Valor);

            _servicoRepositoryMock.Verify(r => r.ObterPorIdAsync(servicoId), Times.Once);
            _servicoRepositoryMock.VerifyNoOtherCalls();
        }

        #endregion
    }
}
