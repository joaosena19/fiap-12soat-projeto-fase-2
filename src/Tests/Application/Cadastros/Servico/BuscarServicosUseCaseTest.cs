using Application.Contracts.Presenters;
using Tests.Application.Cadastros.Servico.Helpers;
using ServicoAggregate = Domain.Cadastros.Aggregates.Servico;

namespace Tests.Application.Cadastros.Servico
{
    public class BuscarServicosUseCaseTest
    {
        private readonly ServicoTestFixture _fixture;

        public BuscarServicosUseCaseTest()
        {
            _fixture = new ServicoTestFixture();
        }

        [Fact(DisplayName = "Deve retornar lista de serviços")]
        [Trait("UseCase", "BuscarServicos")]
        public async Task ExecutarAsync_DeveRetornarListaDeServicos()
        {
            // Arrange
            var servicos = new List<ServicoAggregate> { new ServicoBuilder().Build(), new ServicoBuilder().Build() };
            _fixture.ServicoGatewayMock.AoObterTodos().Retorna(servicos);

            // Act
            await _fixture.BuscarServicosUseCase.ExecutarAsync(_fixture.ServicoGatewayMock.Object, _fixture.BuscarServicosPresenterMock.Object);

            // Assert
            _fixture.BuscarServicosPresenterMock.DeveTerApresentadoSucesso<IBuscarServicosPresenter, IEnumerable<ServicoAggregate>>(servicos);
            _fixture.BuscarServicosPresenterMock.NaoDeveTerApresentadoErro<IBuscarServicosPresenter, IEnumerable<ServicoAggregate>>();
        }

        [Fact(DisplayName = "Deve retornar lista vazia quando não houver serviços")]
        [Trait("UseCase", "BuscarServicos")]
        public async Task ExecutarAsync_DeveRetornarListaVazia_QuandoNaoHouverServicos()
        {
            // Arrange
            var listaVazia = new List<ServicoAggregate>();
            _fixture.ServicoGatewayMock.AoObterTodos().Retorna(listaVazia);

            // Act
            await _fixture.BuscarServicosUseCase.ExecutarAsync(_fixture.ServicoGatewayMock.Object, _fixture.BuscarServicosPresenterMock.Object);

            // Assert
            _fixture.BuscarServicosPresenterMock.DeveTerApresentadoSucesso<IBuscarServicosPresenter, IEnumerable<ServicoAggregate>>(listaVazia);
            _fixture.BuscarServicosPresenterMock.NaoDeveTerApresentadoErro<IBuscarServicosPresenter, IEnumerable<ServicoAggregate>>();
        }
    }
}