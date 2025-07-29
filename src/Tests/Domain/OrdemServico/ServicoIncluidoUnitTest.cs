using Domain.OrdemServico.ValueObjects.ServicoIncluido;
using FluentAssertions;
using Shared.Exceptions;

namespace Tests.Domain.OrdemServico
{
    public class ServicoIncluidoUnitTest
    {
        #region Testes ValueObject NomeServico

        [Theory(DisplayName = "Não deve criar nome do serviço se for inválido")]
        [InlineData("")]
        [InlineData("   ")]
        [Trait("ValueObject", "NomeServico")]
        public void NomeServico_ComValorInvalido_DeveLancarExcecao(string nomeInvalido)
        {
            // Act & Assert
            FluentActions.Invoking(() => new NomeServico(nomeInvalido))
                .Should().Throw<DomainException>()
                .WithMessage("Nome não pode ser vazio");
        }

        [Fact(DisplayName = "Não deve criar nome do serviço se for nulo")]
        [Trait("ValueObject", "NomeServico")]
        public void NomeServico_ComValorNulo_DeveLancarExcecao()
        {
            // Arrange
            string nomeNulo = null!;

            // Act & Assert
            FluentActions.Invoking(() => new NomeServico(nomeNulo))
                .Should().Throw<DomainException>()
                .WithMessage("Nome não pode ser vazio");
        }

        [Fact(DisplayName = "Não deve criar nome do serviço se exceder 500 caracteres")]
        [Trait("ValueObject", "NomeServico")]
        public void NomeServico_ComMaisDe500Caracteres_DeveLancarExcecao()
        {
            // Arrange
            var nomeInvalido = new string('A', 501);

            // Act & Assert
            FluentActions.Invoking(() => new NomeServico(nomeInvalido))
                .Should().Throw<DomainException>()
                .WithMessage("Nome não pode ter mais de 500 caracteres");
        }

        [Theory(DisplayName = "Deve aceitar nomes de serviço válidos")]
        [InlineData("Troca de óleo")]
        [InlineData("A")]
        [InlineData("Revisão completa do sistema de freios e suspensão do veículo")]
        [Trait("ValueObject", "NomeServico")]
        public void NomeServico_ComValorValido_DeveAceitarNome(string nomeValido)
        {
            // Act
            var nome = new NomeServico(nomeValido);

            // Assert
            nome.Valor.Should().Be(nomeValido);
        }

        [Fact(DisplayName = "Deve aceitar nome do serviço com exatamente 500 caracteres")]
        [Trait("ValueObject", "NomeServico")]
        public void NomeServico_ComExatamente500Caracteres_DeveAceitarNome()
        {
            // Arrange
            var nomeValido = new string('A', 500);

            // Act
            var nome = new NomeServico(nomeValido);

            // Assert
            nome.Valor.Should().Be(nomeValido);
            nome.Valor.Length.Should().Be(500);
        }

        #endregion

        #region Testes ValueObject PrecoServico

        [Theory(DisplayName = "Não deve criar preço do serviço se for negativo")]
        [InlineData(-0.01)]
        [InlineData(-1)]
        [InlineData(-100)]
        [Trait("ValueObject", "PrecoServico")]
        public void PrecoServico_ComValorNegativo_DeveLancarExcecao(decimal precoInvalido)
        {
            // Act & Assert
            FluentActions.Invoking(() => new PrecoServico(precoInvalido))
                .Should().Throw<DomainException>()
                .WithMessage("Preço não pode ser negativo");
        }

        [Theory(DisplayName = "Deve aceitar preços de serviço válidos")]
        [InlineData(0)]
        [InlineData(0.01)]
        [InlineData(150.75)]
        [InlineData(5000)]
        [InlineData(999999.99)]
        [Trait("ValueObject", "PrecoServico")]
        public void PrecoServico_ComValorValido_DeveAceitarPreco(decimal precoValido)
        {
            // Act
            var preco = new PrecoServico(precoValido);

            // Assert
            preco.Valor.Should().Be(precoValido);
        }

        #endregion
    }
}
