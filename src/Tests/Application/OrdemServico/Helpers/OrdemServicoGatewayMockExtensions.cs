using Application.Contracts.Gateways;
using Application.OrdemServico.Interfaces.External;
using Moq;
using OrdemServicoAggregate = Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico;
using Application.OrdemServico.Dtos.External;

namespace Tests.Application.OrdemServico.Helpers
{
    public class OrdemServicoGatewayObterPorIdSetupBuilder
    {
        private readonly Mock<IOrdemServicoGateway> _mock;
        private readonly Guid _id;

        public OrdemServicoGatewayObterPorIdSetupBuilder(Mock<IOrdemServicoGateway> mock, Guid id)
        {
            _mock = mock;
            _id = id;
        }

        public void Retorna(OrdemServicoAggregate ordemServico) => _mock.Setup(g => g.ObterPorIdAsync(_id)).ReturnsAsync(ordemServico);

        public void NaoRetornaNada() => _mock.Setup(g => g.ObterPorIdAsync(_id)).ReturnsAsync((OrdemServicoAggregate?)null);

        public void LancaExcecao(Exception excecao) => _mock.Setup(g => g.ObterPorIdAsync(_id)).ThrowsAsync(excecao);
    }

    public class OrdemServicoGatewayAtualizarSetupBuilder
    {
        private readonly Mock<IOrdemServicoGateway> _mock;

        public OrdemServicoGatewayAtualizarSetupBuilder(Mock<IOrdemServicoGateway> mock)
        {
            _mock = mock;
        }

        public void Retorna(OrdemServicoAggregate ordemServico) => _mock.Setup(g => g.AtualizarAsync(It.IsAny<OrdemServicoAggregate>())).ReturnsAsync(ordemServico);

        public void ComCallback(Action<OrdemServicoAggregate> callback) => _mock.Setup(g => g.AtualizarAsync(It.IsAny<OrdemServicoAggregate>())).Callback<OrdemServicoAggregate>(callback).ReturnsAsync((OrdemServicoAggregate os) => os);

        public void LancaExcecao(Exception excecao) => _mock.Setup(g => g.AtualizarAsync(It.IsAny<OrdemServicoAggregate>())).ThrowsAsync(excecao);
    }

    public class EstoqueExternalServiceObterItemEstoquePorIdSetupBuilder
    {
        private readonly Mock<IEstoqueExternalService> _mock;
        private readonly Guid _itemId;

        public EstoqueExternalServiceObterItemEstoquePorIdSetupBuilder(Mock<IEstoqueExternalService> mock, Guid itemId)
        {
            _mock = mock;
            _itemId = itemId;
        }

        public void Retorna(ItemEstoqueExternalDto itemEstoque) => _mock.Setup(s => s.ObterItemEstoquePorIdAsync(_itemId)).ReturnsAsync(itemEstoque);

        public void NaoRetornaNada() => _mock.Setup(s => s.ObterItemEstoquePorIdAsync(_itemId)).ReturnsAsync((ItemEstoqueExternalDto?)null);

        public void LancaExcecao(Exception excecao) => _mock.Setup(s => s.ObterItemEstoquePorIdAsync(_itemId)).ThrowsAsync(excecao);
    }

    public class EstoqueExternalServiceVerificarDisponibilidadeSetupBuilder
    {
        private readonly Mock<IEstoqueExternalService> _mock;
        private readonly Guid _itemId;
        private readonly int _quantidade;

        public EstoqueExternalServiceVerificarDisponibilidadeSetupBuilder(Mock<IEstoqueExternalService> mock, Guid itemId, int quantidade)
        {
            _mock = mock;
            _itemId = itemId;
            _quantidade = quantidade;
        }

        public void Retorna(bool disponivel) => _mock.Setup(s => s.VerificarDisponibilidadeAsync(_itemId, _quantidade)).ReturnsAsync(disponivel);

        public void LancaExcecao(Exception excecao) => _mock.Setup(s => s.VerificarDisponibilidadeAsync(_itemId, _quantidade)).ThrowsAsync(excecao);
    }

    public class EstoqueExternalServiceAtualizarQuantidadeSetupBuilder
    {
        private readonly Mock<IEstoqueExternalService> _mock;
        private readonly Guid _itemId;
        private readonly int _novaQuantidade;

        public EstoqueExternalServiceAtualizarQuantidadeSetupBuilder(Mock<IEstoqueExternalService> mock, Guid itemId, int novaQuantidade)
        {
            _mock = mock;
            _itemId = itemId;
            _novaQuantidade = novaQuantidade;
        }

        public void Completa() => _mock.Setup(s => s.AtualizarQuantidadeEstoqueAsync(_itemId, _novaQuantidade)).Returns(Task.CompletedTask);

        public void LancaExcecao(Exception excecao) => _mock.Setup(s => s.AtualizarQuantidadeEstoqueAsync(_itemId, _novaQuantidade)).ThrowsAsync(excecao);
    }

    public class ServicoExternalServiceObterServicoPorIdSetupBuilder
    {
        private readonly Mock<IServicoExternalService> _mock;
        private readonly Guid _servicoId;

        public ServicoExternalServiceObterServicoPorIdSetupBuilder(Mock<IServicoExternalService> mock, Guid servicoId)
        {
            _mock = mock;
            _servicoId = servicoId;
        }

        public void Retorna(ServicoExternalDto servico) => _mock.Setup(s => s.ObterServicoPorIdAsync(_servicoId)).ReturnsAsync(servico);

        public void NaoRetornaNada() => _mock.Setup(s => s.ObterServicoPorIdAsync(_servicoId)).ReturnsAsync((ServicoExternalDto?)null);

        public void LancaExcecao(Exception excecao) => _mock.Setup(s => s.ObterServicoPorIdAsync(_servicoId)).ThrowsAsync(excecao);
    }

    public static class OrdemServicoGatewayMockExtensions
    {
        public static OrdemServicoGatewayObterPorIdSetupBuilder AoObterPorId(this Mock<IOrdemServicoGateway> mock, Guid id) => new OrdemServicoGatewayObterPorIdSetupBuilder(mock, id);

        public static OrdemServicoGatewayAtualizarSetupBuilder AoAtualizar(this Mock<IOrdemServicoGateway> mock) => new OrdemServicoGatewayAtualizarSetupBuilder(mock);

        public static EstoqueExternalServiceObterItemEstoquePorIdSetupBuilder AoObterItemEstoquePorId(this Mock<IEstoqueExternalService> mock, Guid itemId) => new EstoqueExternalServiceObterItemEstoquePorIdSetupBuilder(mock, itemId);

        public static EstoqueExternalServiceVerificarDisponibilidadeSetupBuilder AoVerificarDisponibilidade(this Mock<IEstoqueExternalService> mock, Guid itemId, int quantidade) => new EstoqueExternalServiceVerificarDisponibilidadeSetupBuilder(mock, itemId, quantidade);

        public static EstoqueExternalServiceAtualizarQuantidadeSetupBuilder AoAtualizarQuantidade(this Mock<IEstoqueExternalService> mock, Guid itemId, int novaQuantidade) => new EstoqueExternalServiceAtualizarQuantidadeSetupBuilder(mock, itemId, novaQuantidade);

        public static ServicoExternalServiceObterServicoPorIdSetupBuilder AoObterServicoPorId(this Mock<IServicoExternalService> mock, Guid servicoId) => new ServicoExternalServiceObterServicoPorIdSetupBuilder(mock, servicoId);

        public static void DeveTerAtualizadoQuantidade(this Mock<IEstoqueExternalService> mock, Guid itemId, int novaQuantidade)
        {
            mock.Verify(s => s.AtualizarQuantidadeEstoqueAsync(itemId, novaQuantidade), Times.Once);
        }
    }
}