using Domain.OrdemServico.Enums;
using Domain.OrdemServico.ValueObjects.OrdemServico;
using FluentAssertions;
using Shared.Exceptions;

namespace Tests.Domain.OrdemServico
{
    public class OrdemServicoUnitTest
    {
        #region Testes ValueObject Codigo

        [Theory(DisplayName = "Não deve criar código se o formato for inválido")]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("OS-123")]
        [InlineData("OS-20240101")]
        [InlineData("OS-20240101-ABC")]
        [InlineData("OS-2024-123456")]
        [InlineData("ABC-20240101-123456")]
        [Trait("ValueObject", "Codigo")]
        public void Codigo_ComFormatoInvalido_DeveLancarExcecao(string codigoInvalido)
        {
            // Act & Assert
            FluentActions.Invoking(() => new Codigo(codigoInvalido))
                .Should().Throw<DomainException>()
                .WithMessage("*Código*inválido. Formato esperado: OS-YYYYMMDD-ABC123");
        }

        [Fact(DisplayName = "Não deve criar código se for nulo")]
        [Trait("ValueObject", "Codigo")]
        public void Codigo_ComValorNulo_DeveLancarExcecao()
        {
            // Arrange
            string codigoNulo = null!;

            // Act & Assert
            FluentActions.Invoking(() => new Codigo(codigoNulo))
                .Should().Throw<DomainException>()
                .WithMessage("*Código*inválido. Formato esperado: OS-YYYYMMDD-ABC123");
        }

        [Theory(DisplayName = "Deve aceitar códigos com formato válido")]
        [InlineData("OS-20240101-ABC123")]
        [InlineData("OS-20241231-XYZ789")]
        [InlineData("OS-20240701-A1B2C3")]
        [InlineData("os-20240101-abc123")]
        [InlineData(" os-20240101-abc123 ")]
        [Trait("ValueObject", "Codigo")]
        public void Codigo_ComFormatoValido_DeveAceitarCodigo(string codigoValido)
        {
            // Act
            var codigo = new Codigo(codigoValido);

            // Assert
            codigo.Valor.Should().Be(codigoValido.Trim().ToUpper());
        }

        [Fact(DisplayName = "Deve gerar novo código com formato correto")]
        [Trait("ValueObject", "Codigo")]
        public void GerarNovo_DeveGerarCodigoComFormatoCorreto()
        {
            // Act
            var codigo = Codigo.GerarNovo();

            // Assert
            codigo.Valor.Should().StartWith("OS-");
            codigo.Valor.Should().MatchRegex(@"^OS-\d{8}-[A-Z0-9]{6}$");
        }

        #endregion

        #region Testes ValueObject HistoricoTemporal

        [Fact(DisplayName = "Não deve criar histórico temporal se data de criação for vazia")]
        [Trait("ValueObject", "HistoricoTemporal")]
        public void HistoricoTemporal_ComDataCriacaoVazia_DeveLancarExcecao()
        {
            // Arrange
            var dataInvalida = default(DateTime);

            // Act & Assert
            FluentActions.Invoking(() => new HistoricoTemporal(dataInvalida))
                .Should().Throw<DomainException>()
                .WithMessage("A data de criação é obrigatória.");
        }

        [Fact(DisplayName = "Não deve criar histórico temporal se data de início de execução for anterior à data de criação")]
        [Trait("ValueObject", "HistoricoTemporal")]
        public void HistoricoTemporal_ComDataInicioExecucaoAnteriorACriacao_DeveLancarExcecao()
        {
            // Arrange
            var dataCriacao = DateTime.UtcNow;
            var dataInicioExecucao = dataCriacao.AddDays(-1);

            // Act & Assert
            FluentActions.Invoking(() => new HistoricoTemporal(dataCriacao, dataInicioExecucao))
                .Should().Throw<DomainException>()
                .WithMessage("A data de início de execução não pode ser anterior à data de criação.");
        }

        [Fact(DisplayName = "Não deve criar histórico temporal se data de finalização for anterior à data de início de execução")]
        [Trait("ValueObject", "HistoricoTemporal")]
        public void HistoricoTemporal_ComDataFinalizacaoAnteriorAInicioExecucao_DeveLancarExcecao()
        {
            // Arrange
            var dataCriacao = DateTime.UtcNow;
            var dataInicioExecucao = dataCriacao.AddHours(1);
            var dataFinalizacao = dataInicioExecucao.AddHours(-1);

            // Act & Assert
            FluentActions.Invoking(() => new HistoricoTemporal(dataCriacao, dataInicioExecucao, dataFinalizacao))
                .Should().Throw<DomainException>()
                .WithMessage("A data de finalização não pode ser anterior à data de início de execução.");
        }

        [Fact(DisplayName = "Não deve criar histórico temporal se data de entrega for anterior à data de finalização")]
        [Trait("ValueObject", "HistoricoTemporal")]
        public void HistoricoTemporal_ComDataEntregaAnteriorAFinalizacao_DeveLancarExcecao()
        {
            // Arrange
            var dataCriacao = DateTime.UtcNow;
            var dataInicioExecucao = dataCriacao.AddHours(1);
            var dataFinalizacao = dataInicioExecucao.AddHours(1);
            var dataEntrega = dataFinalizacao.AddHours(-1);

            // Act & Assert
            FluentActions.Invoking(() => new HistoricoTemporal(dataCriacao, dataInicioExecucao, dataFinalizacao, dataEntrega))
                .Should().Throw<DomainException>()
                .WithMessage("A data de entrega não pode ser anterior à data de finalização.");
        }

        [Fact(DisplayName = "Deve aceitar histórico temporal com apenas data de criação")]
        [Trait("ValueObject", "HistoricoTemporal")]
        public void HistoricoTemporal_ComApenasDataCriacao_DeveAceitarHistorico()
        {
            // Arrange
            var dataCriacao = DateTime.UtcNow;

            // Act
            var historico = new HistoricoTemporal(dataCriacao);

            // Assert
            historico.DataCriacao.Should().Be(dataCriacao);
            historico.DataInicioExecucao.Should().BeNull();
            historico.DataFinalizacao.Should().BeNull();
            historico.DataEntrega.Should().BeNull();
        }

        [Fact(DisplayName = "Deve aceitar histórico temporal com datas válidas")]
        [Trait("ValueObject", "HistoricoTemporal")]
        public void HistoricoTemporal_ComDatasValidas_DeveAceitarHistorico()
        {
            // Arrange
            var dataCriacao = DateTime.UtcNow;
            var dataInicioExecucao = dataCriacao.AddHours(1);
            var dataFinalizacao = dataInicioExecucao.AddHours(2);
            var dataEntrega = dataFinalizacao.AddHours(1);

            // Act
            var historico = new HistoricoTemporal(dataCriacao, dataInicioExecucao, dataFinalizacao, dataEntrega);

            // Assert
            historico.DataCriacao.Should().Be(dataCriacao);
            historico.DataInicioExecucao.Should().Be(dataInicioExecucao);
            historico.DataFinalizacao.Should().Be(dataFinalizacao);
            historico.DataEntrega.Should().Be(dataEntrega);
        }

        [Fact(DisplayName = "Deve marcar data de início de execução")]
        [Trait("ValueObject", "HistoricoTemporal")]
        public void MarcarDataInicioExecucao_DeveMarcarDataCorretamente()
        {
            // Arrange
            var dataCriacao = DateTime.UtcNow;
            var historico = new HistoricoTemporal(dataCriacao);
            var dataInicioExecucao = dataCriacao.AddHours(1);

            // Act
            var novoHistorico = historico.MarcarDataInicioExecucao(dataInicioExecucao);

            // Assert
            novoHistorico.DataCriacao.Should().Be(dataCriacao);
            novoHistorico.DataInicioExecucao.Should().Be(dataInicioExecucao);
        }

        [Fact(DisplayName = "Deve marcar data de finalização")]
        [Trait("ValueObject", "HistoricoTemporal")]
        public void MarcarDataFinalizadaExecucao_DeveMarcarDataCorretamente()
        {
            // Arrange
            var dataCriacao = DateTime.UtcNow;
            var dataInicioExecucao = dataCriacao.AddHours(1);
            var historico = new HistoricoTemporal(dataCriacao, dataInicioExecucao);
            var dataFinalizacao = dataInicioExecucao.AddHours(2);

            // Act
            var novoHistorico = historico.MarcarDataFinalizadaExecucao(dataFinalizacao);

            // Assert
            novoHistorico.DataCriacao.Should().Be(dataCriacao);
            novoHistorico.DataInicioExecucao.Should().Be(dataInicioExecucao);
            novoHistorico.DataFinalizacao.Should().Be(dataFinalizacao);
        }

        [Fact(DisplayName = "Deve marcar data de entrega")]
        [Trait("ValueObject", "HistoricoTemporal")]
        public void MarcarDataEntrega_DeveMarcarDataCorretamente()
        {
            // Arrange
            var dataCriacao = DateTime.UtcNow;
            var dataInicioExecucao = dataCriacao.AddHours(1);
            var dataFinalizacao = dataInicioExecucao.AddHours(2);
            var historico = new HistoricoTemporal(dataCriacao, dataInicioExecucao, dataFinalizacao);
            var dataEntrega = dataFinalizacao.AddHours(1);

            // Act
            var novoHistorico = historico.MarcarDataEntrega(dataEntrega);

            // Assert
            novoHistorico.DataCriacao.Should().Be(dataCriacao);
            novoHistorico.DataInicioExecucao.Should().Be(dataInicioExecucao);
            novoHistorico.DataFinalizacao.Should().Be(dataFinalizacao);
            novoHistorico.DataEntrega.Should().Be(dataEntrega);
        }

        #endregion

        #region Testes ValueObject Status

        [Theory(DisplayName = "Não deve criar status se enum for inválido")]
        [InlineData((StatusOrdemServicoEnum)(-1))]
        [InlineData((StatusOrdemServicoEnum)7)]
        [InlineData((StatusOrdemServicoEnum)999)]
        [Trait("ValueObject", "Status")]
        public void Status_ComEnumInvalido_DeveLancarExcecao(StatusOrdemServicoEnum statusInvalido)
        {
            // Act & Assert
            FluentActions.Invoking(() => new Status(statusInvalido))
                .Should().Throw<DomainException>()
                .WithMessage("*Status da Ordem de Serviço*não é válido*");
        }

        [Theory(DisplayName = "Deve aceitar status válidos")]
        [InlineData(StatusOrdemServicoEnum.Cancelada)]
        [InlineData(StatusOrdemServicoEnum.Recebida)]
        [InlineData(StatusOrdemServicoEnum.EmDiagnostico)]
        [InlineData(StatusOrdemServicoEnum.AguardandoAprovacao)]
        [InlineData(StatusOrdemServicoEnum.EmExecucao)]
        [InlineData(StatusOrdemServicoEnum.Finalizada)]
        [InlineData(StatusOrdemServicoEnum.Entregue)]
        [Trait("ValueObject", "Status")]
        public void Status_ComEnumValido_DeveAceitarStatus(StatusOrdemServicoEnum statusValido)
        {
            // Act
            var status = new Status(statusValido);

            // Assert
            status.Valor.Should().Be(statusValido);
        }

        #endregion
    }
}
