using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.OrdemServico.Dtos;
using Application.OrdemServico.UseCases;
using Domain.Cadastros.Aggregates;
using Domain.Estoque.Aggregates;
using Moq;
using Tests.Application.SharedHelpers.Gateways;
using OrdemServicoAggregate = Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico;

namespace Tests.Application.OrdemServico.Helpers
{
    public class CriarOrdemServicoCompletaTestFixture
    {
        public CriarOrdemServicoCompletaUseCase UseCase { get; }
        public Mock<IOrdemServicoGateway> OrdemServicoGatewayMock { get; }
        public Mock<IClienteGateway> ClienteGatewayMock { get; }
        public Mock<IVeiculoGateway> VeiculoGatewayMock { get; }
        public Mock<IServicoGateway> ServicoGatewayMock { get; }
        public Mock<IItemEstoqueGateway> ItemEstoqueGatewayMock { get; }
        public Mock<ICriarOrdemServicoCompletaPresenter> PresenterMock { get; }

        public CriarOrdemServicoCompletaTestFixture()
        {
            UseCase = new CriarOrdemServicoCompletaUseCase();
            OrdemServicoGatewayMock = new Mock<IOrdemServicoGateway>();
            ClienteGatewayMock = new Mock<IClienteGateway>();
            VeiculoGatewayMock = new Mock<IVeiculoGateway>();
            ServicoGatewayMock = new Mock<IServicoGateway>();
            ItemEstoqueGatewayMock = new Mock<IItemEstoqueGateway>();
            PresenterMock = new Mock<ICriarOrdemServicoCompletaPresenter>();
        }



        public void ConfigurarClienteNaoExistente(string documento)
        {
            ClienteGatewayMock.AoObterPorDocumento(documento).NaoRetornaNada();
            ClienteGatewayMock.Setup(g => g.SalvarAsync(It.IsAny<Cliente>())).ReturnsAsync((Cliente c) => c);
        }

        public void ConfigurarClienteExistente(string documento, Cliente cliente)
        {
            ClienteGatewayMock.AoObterPorDocumento(documento).Retorna(cliente);
        }

        public void ConfigurarVeiculoNaoExistente(string placa)
        {
            VeiculoGatewayMock.AoObterPorPlaca(placa).NaoRetornaNada();
            VeiculoGatewayMock.Setup(g => g.SalvarAsync(It.IsAny<Veiculo>())).ReturnsAsync((Veiculo v) => v);
        }

        public void ConfigurarVeiculoExistente(string placa, Veiculo veiculo)
        {
            VeiculoGatewayMock.AoObterPorPlaca(placa).Retorna(veiculo);
        }

        public void ConfigurarServicoExistente(Guid servicoId, Servico servico)
        {
            ServicoGatewayMock.AoObterPorId(servicoId).Retorna(servico);
        }

        public void ConfigurarServicoNaoExistente(Guid servicoId)
        {
            ServicoGatewayMock.AoObterPorId(servicoId).NaoRetornaNada();
        }

        public void ConfigurarItemEstoqueExistente(Guid itemId, ItemEstoque item)
        {
            ItemEstoqueGatewayMock.AoObterPorId(itemId).Retorna(item);
        }

        public void ConfigurarItemEstoqueNaoExistente(Guid itemId)
        {
            ItemEstoqueGatewayMock.AoObterPorId(itemId).NaoRetornaNada();
        }

        public void ConfigurarCodigoOrdemServicoUnico()
        {
            OrdemServicoGatewayMock.Setup(g => g.ObterPorCodigoAsync(It.IsAny<string>())).ReturnsAsync((OrdemServicoAggregate?)null);
        }

        public void ConfigurarEntidadesBasicas(CriarOrdemServicoCompletaDto dto)
        {
            ConfigurarClienteNaoExistente(dto.Cliente.DocumentoIdentificador);
            ConfigurarVeiculoNaoExistente(dto.Veiculo.Placa);
            ConfigurarCodigoOrdemServicoUnico();
            OrdemServicoGatewayMock.AoSalvar().ComCallback(os => { /* callback vazio apenas para permitir o retorno */ });
        }
    }
}