using Application.Contracts.Presenters;
using FluentAssertions;
using Moq;
using Shared.Enums;
using Tests.Application.OrdemServico.Helpers;
using Tests.Helpers;
using OrdemServicoAggregate = Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico;
using Domain.OrdemServico.Enums;

namespace Tests.Application.OrdemServico
{
    public class AprovarOrcamentoUseCaseTest
    {
        private readonly OrdemServicoTestFixture _fixture;

        public AprovarOrcamentoUseCaseTest()
        {
            _fixture = new OrdemServicoTestFixture();
        }

        [Fact(DisplayName = "Deve aprovar orçamento com sucesso quando ordem de serviço existir e itens estiverem disponíveis")]
        [Trait("UseCase", "AprovarOrcamento")]
        public async Task ExecutarAsync_DeveAprovarOrcamentoComSucesso_QuandoOrdemServicoExistirEItensEstiveremDisponiveis()
        {
            // Arrange
            var ordemServico = new OrdemServicoBuilder().Build();
            ordemServico.IniciarDiagnostico();
            
            var servico = new ServicoExternalDtoBuilder().Build();
            ordemServico.AdicionarServico(servico.Id, servico.Nome, servico.Preco);
            
            var itemEstoque = new ItemEstoqueExternalDtoBuilder().Build();
            ordemServico.AdicionarItem(itemEstoque.Id, itemEstoque.Nome, itemEstoque.Preco, 2, TipoItemIncluidoEnum.Peca);
            
            ordemServico.GerarOrcamento();

            OrdemServicoAggregate? ordemServicoAtualizada = null;

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServico.Id).Retorna(ordemServico);
            _fixture.EstoqueExternalServiceMock.AoVerificarDisponibilidade(itemEstoque.Id, 2).Retorna(true);
            _fixture.EstoqueExternalServiceMock.AoObterItemEstoquePorId(itemEstoque.Id).Retorna(itemEstoque);
            _fixture.EstoqueExternalServiceMock.AoAtualizarQuantidade(itemEstoque.Id, itemEstoque.Quantidade - 2).Completa();
            _fixture.OrdemServicoGatewayMock.AoAtualizar().ComCallback(os => ordemServicoAtualizada = os);

            // Act
            await _fixture.AprovarOrcamentoUseCase.ExecutarAsync(
                ordemServico.Id,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.EstoqueExternalServiceMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object);

            // Assert
            ordemServicoAtualizada.Should().NotBeNull();
            ordemServicoAtualizada!.Status.Valor.Should().Be(StatusOrdemServicoEnum.EmExecucao);

            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoSucesso();
            _fixture.OperacaoOrdemServicoPresenterMock.NaoDeveTerApresentadoErro();
        }

        [Fact(DisplayName = "Deve verificar se a quantidade no estoque foi atualizada após aprovação")]
        [Trait("UseCase", "AprovarOrcamento")]
        public async Task ExecutarAsync_DeveAtualizarQuantidadeNoEstoque_AposAprovacao()
        {
            // Arrange
            var ordemServico = new OrdemServicoBuilder().Build();
            ordemServico.IniciarDiagnostico();
            
            var itemEstoque = new ItemEstoqueExternalDtoBuilder().Build();
            var quantidadeRequisitada = 3;
            ordemServico.AdicionarItem(itemEstoque.Id, itemEstoque.Nome, itemEstoque.Preco, quantidadeRequisitada, TipoItemIncluidoEnum.Peca);
            
            ordemServico.GerarOrcamento();

            var quantidadeEsperadaAposAtualizacao = itemEstoque.Quantidade - quantidadeRequisitada;

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServico.Id).Retorna(ordemServico);
            _fixture.EstoqueExternalServiceMock.AoVerificarDisponibilidade(itemEstoque.Id, quantidadeRequisitada).Retorna(true);
            _fixture.EstoqueExternalServiceMock.AoObterItemEstoquePorId(itemEstoque.Id).Retorna(itemEstoque);
            _fixture.EstoqueExternalServiceMock.AoAtualizarQuantidade(itemEstoque.Id, quantidadeEsperadaAposAtualizacao).Completa();
            _fixture.OrdemServicoGatewayMock.AoAtualizar().ComCallback(_ => { });

            // Act
            await _fixture.AprovarOrcamentoUseCase.ExecutarAsync(
                ordemServico.Id,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.EstoqueExternalServiceMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object);

            // Assert
            _fixture.EstoqueExternalServiceMock.DeveTerAtualizadoQuantidade(itemEstoque.Id, quantidadeEsperadaAposAtualizacao);
            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoSucesso();
            _fixture.OperacaoOrdemServicoPresenterMock.NaoDeveTerApresentadoErro();
        }

        [Fact(DisplayName = "Deve apresentar erro quando ordem de serviço não existir")]
        [Trait("UseCase", "AprovarOrcamento")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoOrdemServicoNaoExistir()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServicoId).NaoRetornaNada();

            // Act
            await _fixture.AprovarOrcamentoUseCase.ExecutarAsync(
                ordemServicoId,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.EstoqueExternalServiceMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object);

            // Assert
            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoErro("Ordem de serviço não encontrada.", ErrorType.ResourceNotFound);
            _fixture.OperacaoOrdemServicoPresenterMock.NaoDeveTerApresentadoSucesso();
        }

        [Fact(DisplayName = "Deve apresentar erro quando item não estiver disponível no estoque")]
        [Trait("UseCase", "AprovarOrcamento")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoItemNaoEstiverDisponivelNoEstoque()
        {
            // Arrange
            var ordemServico = new OrdemServicoBuilder().Build();
            ordemServico.IniciarDiagnostico();
            
            var itemEstoque = new ItemEstoqueExternalDtoBuilder().Build();
            ordemServico.AdicionarItem(itemEstoque.Id, itemEstoque.Nome, itemEstoque.Preco, 5, TipoItemIncluidoEnum.Peca);
            
            ordemServico.GerarOrcamento();

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServico.Id).Retorna(ordemServico);
            _fixture.EstoqueExternalServiceMock.AoVerificarDisponibilidade(itemEstoque.Id, 5).Retorna(false);

            // Act
            await _fixture.AprovarOrcamentoUseCase.ExecutarAsync(
                ordemServico.Id,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.EstoqueExternalServiceMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object);

            // Assert
            var itemIncluido = ordemServico.ItensIncluidos.First();
            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoErro($"Item '{itemIncluido.Nome.Valor}' não está disponível no estoque na quantidade necessária ({itemIncluido.Quantidade.Valor}).", ErrorType.DomainRuleBroken);
            _fixture.OperacaoOrdemServicoPresenterMock.NaoDeveTerApresentadoSucesso();
        }

        [Fact(DisplayName = "Deve apresentar erro de domínio quando ocorrer DomainException")]
        [Trait("UseCase", "AprovarOrcamento")]
        public async Task ExecutarAsync_DeveApresentarErroDominio_QuandoOcorrerDomainException()
        {
            // Arrange
            var ordemServico = new OrdemServicoBuilder().Build();
            // Não gerar orçamento para provocar DomainException

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServico.Id).Retorna(ordemServico);

            // Act
            await _fixture.AprovarOrcamentoUseCase.ExecutarAsync(
                ordemServico.Id,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.EstoqueExternalServiceMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object);

            // Assert
            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoErro("Não existe orçamento para aprovar. É necessário gerar o orçamento primeiro.", ErrorType.DomainRuleBroken);
            _fixture.OperacaoOrdemServicoPresenterMock.NaoDeveTerApresentadoSucesso();
        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "AprovarOrcamento")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {
            // Arrange
            var ordemServico = new OrdemServicoBuilder().Build();
            ordemServico.IniciarDiagnostico();
            
            var servico = new ServicoExternalDtoBuilder().Build();
            ordemServico.AdicionarServico(servico.Id, servico.Nome, servico.Preco);
            
            ordemServico.GerarOrcamento();

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServico.Id).Retorna(ordemServico);
            _fixture.OrdemServicoGatewayMock.AoAtualizar().LancaExcecao(new InvalidOperationException("Erro de banco de dados"));

            // Act
            await _fixture.AprovarOrcamentoUseCase.ExecutarAsync(
                ordemServico.Id,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.EstoqueExternalServiceMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object);

            // Assert
            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoErro("Erro interno do servidor.", ErrorType.UnexpectedError);
            _fixture.OperacaoOrdemServicoPresenterMock.NaoDeveTerApresentadoSucesso();
        }

        [Fact(DisplayName = "Deve verificar disponibilidade de múltiplos itens antes de aprovar")]
        [Trait("UseCase", "AprovarOrcamento")]
        public async Task ExecutarAsync_DeveVerificarDisponibilidadeMultiplosItens_AntesDeAprovar()
        {
            // Arrange
            var ordemServico = new OrdemServicoBuilder().Build();
            ordemServico.IniciarDiagnostico();
            
            var item1 = new ItemEstoqueExternalDtoBuilder().Build();
            var item2 = new ItemEstoqueExternalDtoBuilder().Build();
            
            ordemServico.AdicionarItem(item1.Id, item1.Nome, item1.Preco, 2, TipoItemIncluidoEnum.Peca);
            ordemServico.AdicionarItem(item2.Id, item2.Nome, item2.Preco, 3, TipoItemIncluidoEnum.Peca);
            
            ordemServico.GerarOrcamento();

            OrdemServicoAggregate? ordemServicoAtualizada = null;

            _fixture.OrdemServicoGatewayMock.AoObterPorId(ordemServico.Id).Retorna(ordemServico);
            _fixture.EstoqueExternalServiceMock.AoVerificarDisponibilidade(item1.Id, 2).Retorna(true);
            _fixture.EstoqueExternalServiceMock.AoVerificarDisponibilidade(item2.Id, 3).Retorna(true);
            _fixture.EstoqueExternalServiceMock.AoObterItemEstoquePorId(item1.Id).Retorna(item1);
            _fixture.EstoqueExternalServiceMock.AoObterItemEstoquePorId(item2.Id).Retorna(item2);
            _fixture.EstoqueExternalServiceMock.AoAtualizarQuantidade(item1.Id, item1.Quantidade - 2).Completa();
            _fixture.EstoqueExternalServiceMock.AoAtualizarQuantidade(item2.Id, item2.Quantidade - 3).Completa();
            _fixture.OrdemServicoGatewayMock.AoAtualizar().ComCallback(os => ordemServicoAtualizada = os);

            // Act
            await _fixture.AprovarOrcamentoUseCase.ExecutarAsync(
                ordemServico.Id,
                _fixture.OrdemServicoGatewayMock.Object,
                _fixture.EstoqueExternalServiceMock.Object,
                _fixture.OperacaoOrdemServicoPresenterMock.Object);

            // Assert
            ordemServicoAtualizada.Should().NotBeNull();
            ordemServicoAtualizada!.Status.Valor.Should().Be(StatusOrdemServicoEnum.EmExecucao);

            _fixture.OperacaoOrdemServicoPresenterMock.DeveTerApresentadoSucesso();
            _fixture.OperacaoOrdemServicoPresenterMock.NaoDeveTerApresentadoErro();
        }
    }
}