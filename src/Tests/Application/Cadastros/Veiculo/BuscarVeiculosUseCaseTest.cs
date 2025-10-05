using Application.Contracts.Presenters;
using Shared.Enums;
using Tests.Application.Cadastros.Veiculo.Helpers;
using Tests.Helpers;
using VeiculoAggregate = Domain.Cadastros.Aggregates.Veiculo;

namespace Tests.Application.Cadastros.Veiculo
{
    public class BuscarVeiculosUseCaseTest
    {
        private readonly VeiculoTestFixture _fixture;

        public BuscarVeiculosUseCaseTest()
        {
            _fixture = new VeiculoTestFixture();
        }

        [Fact(DisplayName = "Deve retornar lista de veículos")]
        [Trait("UseCase", "BuscarVeiculos")]
        public async Task ExecutarAsync_DeveRetornarListaDeVeiculos()
        {
            // Arrange
            var veiculos = new List<VeiculoAggregate> { new VeiculoBuilder().Build(), new VeiculoBuilder().Build() };
            _fixture.VeiculoGatewayMock.AoObterTodos().Retorna(veiculos);

            // Act
            await _fixture.BuscarVeiculosUseCase.ExecutarAsync(_fixture.VeiculoGatewayMock.Object, _fixture.BuscarVeiculosPresenterMock.Object);

            // Assert
            _fixture.BuscarVeiculosPresenterMock.DeveTerApresentadoSucesso<IBuscarVeiculosPresenter, IEnumerable<VeiculoAggregate>>(veiculos);
            _fixture.BuscarVeiculosPresenterMock.NaoDeveTerApresentadoErro<IBuscarVeiculosPresenter, IEnumerable<VeiculoAggregate>>();
        }

        [Fact(DisplayName = "Deve retornar lista vazia quando não houver veículos")]
        [Trait("UseCase", "BuscarVeiculos")]
        public async Task ExecutarAsync_DeveRetornarListaVazia_QuandoNaoHouverVeiculos()
        {
            // Arrange
            var listaVazia = new List<VeiculoAggregate>();
            _fixture.VeiculoGatewayMock.AoObterTodos().Retorna(listaVazia);

            // Act
            await _fixture.BuscarVeiculosUseCase.ExecutarAsync(_fixture.VeiculoGatewayMock.Object, _fixture.BuscarVeiculosPresenterMock.Object);

            // Assert
            _fixture.BuscarVeiculosPresenterMock.DeveTerApresentadoSucesso<IBuscarVeiculosPresenter, IEnumerable<VeiculoAggregate>>(listaVazia);
            _fixture.BuscarVeiculosPresenterMock.NaoDeveTerApresentadoErro<IBuscarVeiculosPresenter, IEnumerable<VeiculoAggregate>>();
        }
    }
}