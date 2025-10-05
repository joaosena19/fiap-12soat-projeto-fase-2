using Application.Contracts.Presenters;
using Shared.Enums;
using Tests.Application.OrdemServico.Helpers;
using OrdemServicoAggregate = Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico;

namespace Tests.Application.OrdemServico
{
    public class BuscarOrdensServicoUseCaseTest
    {
        private readonly OrdemServicoTestFixture _fixture;

        public BuscarOrdensServicoUseCaseTest()
        {
            _fixture = new OrdemServicoTestFixture();
        }

        [Fact(DisplayName = "Deve apresentar sucesso quando existirem ordens de serviço")]
        [Trait("UseCase", "BuscarOrdensServico")]
        public async Task ExecutarAsync_DeveApresentarSucesso_QuandoExistiremOrdensServico()
        {
            // Arrange
            var ordem1 = new OrdemServicoBuilder().Build();
            var ordem2 = new OrdemServicoBuilder().Build();
            var ordensServico = new List<OrdemServicoAggregate> { ordem1, ordem2 };

            _fixture.OrdemServicoGatewayMock.AoObterTodos().Retorna(ordensServico);

            // Act
            await _fixture.BuscarOrdensServicoUseCase.ExecutarAsync(
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.BuscarOrdensServicoPresenterMock.Object);

            // Assert
            _fixture.BuscarOrdensServicoPresenterMock.DeveTerApresentadoSucesso<IBuscarOrdensServicoPresenter, IEnumerable<OrdemServicoAggregate>>(ordensServico);
            _fixture.BuscarOrdensServicoPresenterMock.NaoDeveTerApresentadoErro<IBuscarOrdensServicoPresenter, IEnumerable<OrdemServicoAggregate>>();
        }

        [Fact(DisplayName = "Deve apresentar sucesso com lista vazia quando não existirem ordens de serviço")]
        [Trait("UseCase", "BuscarOrdensServico")]
        public async Task ExecutarAsync_DeveApresentarSucessoComListaVazia_QuandoNaoExistiremOrdensServico()
        {
            // Arrange
            _fixture.OrdemServicoGatewayMock.AoObterTodos().RetornaListaVazia();

            // Act
            await _fixture.BuscarOrdensServicoUseCase.ExecutarAsync(
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.BuscarOrdensServicoPresenterMock.Object);

            // Assert
            _fixture.BuscarOrdensServicoPresenterMock.DeveTerApresentadoSucesso<IBuscarOrdensServicoPresenter, OrdemServicoAggregate>(Enumerable.Empty<OrdemServicoAggregate>());
            _fixture.BuscarOrdensServicoPresenterMock.NaoDeveTerApresentadoErro<IBuscarOrdensServicoPresenter, IEnumerable<OrdemServicoAggregate>>();
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "BuscarOrdensServico")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            _fixture.OrdemServicoGatewayMock.AoObterTodos().LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            await _fixture.BuscarOrdensServicoUseCase.ExecutarAsync(
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.BuscarOrdensServicoPresenterMock.Object);

            // Assert
            _fixture.BuscarOrdensServicoPresenterMock.DeveTerApresentadoErro<IBuscarOrdensServicoPresenter, IEnumerable<OrdemServicoAggregate>>("Erro interno do servidor.", ErrorType.UnexpectedError);
            _fixture.BuscarOrdensServicoPresenterMock.NaoDeveTerApresentadoSucesso<IBuscarOrdensServicoPresenter, IEnumerable<OrdemServicoAggregate>>();
        }
    }
}