using Domain.OrdemServico.ValueObjects.Orcamento;
using FluentAssertions;
using Shared.Exceptions;

namespace Tests.Domain.OrdemServico
{
    public class OrcamentoUnitTest
    {
        #region Testes ValueObject DataCriacao

        [Fact(DisplayName = "Não deve criar data de criação se for vazia")]
        [Trait("ValueObject", "DataCriacao")]
        public void DataCriacao_ComValorVazio_DeveLancarExcecao()
        {
            // Arrange
            var dataInvalida = default(DateTime);

            // Act & Assert
            FluentActions.Invoking(() => new DataCriacao(dataInvalida))
                .Should().Throw<DomainException>()
                .WithMessage("Data de criação não pode ser vazia");
        }

        [Theory(DisplayName = "Deve aceitar datas de criação válidas")]
        [InlineData("2024-01-01")]
        [InlineData("2024-07-15")]
        [InlineData("2024-12-31")]
        [Trait("ValueObject", "DataCriacao")]
        public void DataCriacao_ComValorValido_DeveAceitarData(string dataString)
        {
            // Arrange
            var dataValida = DateTime.Parse(dataString);

            // Act
            var dataCriacao = new DataCriacao(dataValida);

            // Assert
            dataCriacao.Valor.Should().Be(dataValida);
        }

        [Fact(DisplayName = "Deve aceitar data de criação atual")]
        [Trait("ValueObject", "DataCriacao")]
        public void DataCriacao_ComDataAtual_DeveAceitarData()
        {
            // Arrange
            var dataAtual = DateTime.UtcNow;

            // Act
            var dataCriacao = new DataCriacao(dataAtual);

            // Assert
            dataCriacao.Valor.Should().Be(dataAtual);
        }

        #endregion

        #region Testes ValueObject PrecoOrcamento

        [Theory(DisplayName = "Não deve criar preço do orçamento se for negativo")]
        [InlineData(-0.01)]
        [InlineData(-1)]
        [InlineData(-100)]
        [Trait("ValueObject", "PrecoOrcamento")]
        public void PrecoOrcamento_ComValorNegativo_DeveLancarExcecao(decimal precoInvalido)
        {
            // Act & Assert
            FluentActions.Invoking(() => new PrecoOrcamento(precoInvalido))
                .Should().Throw<DomainException>()
                .WithMessage("Preço do orçamento não pode ser negativo");
        }

        [Theory(DisplayName = "Deve aceitar preços de orçamento válidos")]
        [InlineData(0)]
        [InlineData(0.01)]
        [InlineData(150.75)]
        [InlineData(5000)]
        [InlineData(999999.99)]
        [Trait("ValueObject", "PrecoOrcamento")]
        public void PrecoOrcamento_ComValorValido_DeveAceitarPreco(decimal precoValido)
        {
            // Act
            var preco = new PrecoOrcamento(precoValido);

            // Assert
            preco.Valor.Should().Be(precoValido);
        }

        #endregion
    }
}
