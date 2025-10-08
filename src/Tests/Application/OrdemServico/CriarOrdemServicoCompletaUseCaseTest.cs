using Domain.OrdemServico.Enums;
using FluentAssertions;
using Moq;
using Shared.Enums;
using Shared.Exceptions;
using Tests.Application.OrdemServico.Helpers;
using Tests.Application.SharedHelpers;
using OrdemServicoAggregate = Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico;

namespace Tests.Application.OrdemServico
{
    public class CriarOrdemServicoCompletaUseCaseTest
    {
        private readonly CriarOrdemServicoCompletaTestFixture _fixture;

        public CriarOrdemServicoCompletaUseCaseTest()
        {
            _fixture = new CriarOrdemServicoCompletaTestFixture();
        }

        [Fact(DisplayName = "Deve criar ordem de serviço completa com cliente e veículo novos")]
        [Trait("UseCase", "CriarOrdemServicoCompleta")]
        public async Task ExecutarAsync_DeveCriarOrdemServicoCompleta_ComClienteEVeiculoNovos()
        {

        }

        [Fact(DisplayName = "Deve criar ordem de serviço completa com cliente e veículo existentes")]
        [Trait("UseCase", "CriarOrdemServicoCompleta")]
        public async Task ExecutarAsync_DeveCriarOrdemServicoCompleta_ComClienteEVeiculoExistentes()
        {

        }

        [Fact(DisplayName = "Deve adicionar serviços encontrados à ordem de serviço")]
        [Trait("UseCase", "CriarOrdemServicoCompleta")]
        public async Task ExecutarAsync_DeveAdicionarServicos_QuandoEncontrados()
        {

        }

        [Fact(DisplayName = "Deve ignorar serviços não encontrados")]
        [Trait("UseCase", "CriarOrdemServicoCompleta")]
        public async Task ExecutarAsync_DeveIgnorarServicos_QuandoNaoEncontrados()
        {

        }

        [Fact(DisplayName = "Deve adicionar itens encontrados à ordem de serviço")]
        [Trait("UseCase", "CriarOrdemServicoCompleta")]
        public async Task ExecutarAsync_DeveAdicionarItens_QuandoEncontrados()
        {

        }

        [Fact(DisplayName = "Deve ignorar itens não encontrados")]
        [Trait("UseCase", "CriarOrdemServicoCompleta")]
        public async Task ExecutarAsync_DeveIgnorarItens_QuandoNaoEncontrados()
        {

        }

        [Fact(DisplayName = "Deve criar ordem de serviço sem serviços nem itens")]
        [Trait("UseCase", "CriarOrdemServicoCompleta")]
        public async Task ExecutarAsync_DeveCriarOrdemServico_SemServicosNemItens()
        {

        }

        [Fact(DisplayName = "Deve gerar código único para ordem de serviço")]
        [Trait("UseCase", "CriarOrdemServicoCompleta")]
        public async Task ExecutarAsync_DeveGerarCodigoUnico_ParaOrdemServico()
        {

        }

        [Fact(DisplayName = "Deve apresentar erro quando ocorrer exceção de domínio")]
        [Trait("UseCase", "CriarOrdemServicoCompleta")]
        public async Task ExecutarAsync_DeveApresentarErro_QuandoOcorrerExcecaoDominio()
        {

        }

        [Fact(DisplayName = "Deve apresentar erro interno quando ocorrer exceção genérica")]
        [Trait("UseCase", "CriarOrdemServicoCompleta")]
        public async Task ExecutarAsync_DeveApresentarErroInterno_QuandoOcorrerExcecaoGenerica()
        {

        }
    }
}