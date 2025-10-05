using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.OrdemServico.Interfaces.External;
using Application.OrdemServico.UseCases;
using Moq;

namespace Tests.Application.OrdemServico.Helpers
{
    public class OrdemServicoTestFixture
    {
        public Mock<IOrdemServicoGateway> OrdemServicoGatewayMock { get; }
        public Mock<IEstoqueExternalService> EstoqueExternalServiceMock { get; }
        public Mock<IServicoExternalService> ServicoExternalServiceMock { get; }
        public Mock<IAdicionarItemPresenter> AdicionarItemPresenterMock { get; }
        public Mock<IAdicionarServicosPresenter> AdicionarServicosPresenterMock { get; }

        public AdicionarItemUseCase AdicionarItemUseCase { get; }
        public AdicionarServicosUseCase AdicionarServicosUseCase { get; }

        public OrdemServicoTestFixture()
        {
            OrdemServicoGatewayMock = new Mock<IOrdemServicoGateway>();
            EstoqueExternalServiceMock = new Mock<IEstoqueExternalService>();
            ServicoExternalServiceMock = new Mock<IServicoExternalService>();
            AdicionarItemPresenterMock = new Mock<IAdicionarItemPresenter>();
            AdicionarServicosPresenterMock = new Mock<IAdicionarServicosPresenter>();

            AdicionarItemUseCase = new AdicionarItemUseCase();
            AdicionarServicosUseCase = new AdicionarServicosUseCase();
        }
    }
}