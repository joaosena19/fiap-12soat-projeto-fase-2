using Domain.OrdemServico.Enums;
using Domain.OrdemServico.ValueObjects.ItemIncluido;
using FluentAssertions;
using Shared.Exceptions;

namespace Tests.Domain.OrdemServico
{
    public class ItemIncluidoUnitTest
    {
        #region Testes ValueObject Nome

        [Theory(DisplayName = "Não deve criar nome se for inválido")]
        [InlineData("")]
        [InlineData("   ")]
        [Trait("ValueObject", "Nome")]
        public void Nome_ComValorInvalido_DeveLancarExcecao(string nomeInvalido)
        {
            // Act & Assert
            FluentActions.Invoking(() => new Nome(nomeInvalido))
                .Should().Throw<DomainException>()
                .WithMessage("Nome não pode ser vazio");
        }

        [Fact(DisplayName = "Não deve criar nome se for nulo")]
        [Trait("ValueObject", "Nome")]
        public void Nome_ComValorNulo_DeveLancarExcecao()
        {
            // Arrange
            string nomeNulo = null!;

            // Act & Assert
            FluentActions.Invoking(() => new Nome(nomeNulo))
                .Should().Throw<DomainException>()
                .WithMessage("Nome não pode ser vazio");
        }

        [Fact(DisplayName = "Não deve criar nome se exceder 200 caracteres")]
        [Trait("ValueObject", "Nome")]
        public void Nome_ComMaisDe200Caracteres_DeveLancarExcecao()
        {
            // Arrange
            var nomeInvalido = new string('A', 201);

            // Act & Assert
            FluentActions.Invoking(() => new Nome(nomeInvalido))
                .Should().Throw<DomainException>()
                .WithMessage("Nome não pode ter mais de 200 caracteres");
        }

        [Theory(DisplayName = "Deve aceitar nomes válidos")]
        [InlineData("Item válido")]
        [InlineData("A")]
        [InlineData("Peça de reposição para motor")]
        [Trait("ValueObject", "Nome")]
        public void Nome_ComValorValido_DeveAceitarNome(string nomeValido)
        {
            // Act
            var nome = new Nome(nomeValido);

            // Assert
            nome.Valor.Should().Be(nomeValido);
        }

        [Fact(DisplayName = "Deve aceitar nome com exatamente 200 caracteres")]
        [Trait("ValueObject", "Nome")]
        public void Nome_ComExatamente200Caracteres_DeveAceitarNome()
        {
            // Arrange
            var nomeValido = new string('A', 200);

            // Act
            var nome = new Nome(nomeValido);

            // Assert
            nome.Valor.Should().Be(nomeValido);
            nome.Valor.Length.Should().Be(200);
        }

        #endregion

        #region Testes ValueObject PrecoItem

        [Theory(DisplayName = "Não deve criar preço se for negativo")]
        [InlineData(-0.01)]
        [InlineData(-1)]
        [InlineData(-100)]
        [Trait("ValueObject", "PrecoItem")]
        public void PrecoItem_ComValorNegativo_DeveLancarExcecao(decimal precoInvalido)
        {
            // Act & Assert
            FluentActions.Invoking(() => new PrecoItem(precoInvalido))
                .Should().Throw<DomainException>()
                .WithMessage("Preço não pode ser negativo");
        }

        [Theory(DisplayName = "Deve aceitar preços válidos")]
        [InlineData(0)]
        [InlineData(0.01)]
        [InlineData(25.50)]
        [InlineData(1000)]
        [InlineData(999999.99)]
        [Trait("ValueObject", "PrecoItem")]
        public void PrecoItem_ComValorValido_DeveAceitarPreco(decimal precoValido)
        {
            // Act
            var preco = new PrecoItem(precoValido);

            // Assert
            preco.Valor.Should().Be(precoValido);
        }

        #endregion

        #region Testes ValueObject Quantidade

        [Theory(DisplayName = "Não deve criar quantidade se for negativa")]
        [InlineData(-1)]
        [InlineData(-10)]
        [InlineData(-100)]
        [Trait("ValueObject", "Quantidade")]
        public void Quantidade_ComValorNegativo_DeveLancarExcecao(int quantidadeInvalida)
        {
            // Act & Assert
            FluentActions.Invoking(() => new Quantidade(quantidadeInvalida))
                .Should().Throw<DomainException>()
                .WithMessage("Quantidade não pode ser negativa");
        }

        [Theory(DisplayName = "Deve aceitar quantidades válidas")]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(50)]
        [InlineData(1000)]
        [Trait("ValueObject", "Quantidade")]
        public void Quantidade_ComValorValido_DeveAceitarQuantidade(int quantidadeValida)
        {
            // Act
            var quantidade = new Quantidade(quantidadeValida);

            // Assert
            quantidade.Valor.Should().Be(quantidadeValida);
        }

        #endregion

        #region Testes ValueObject TipoItemIncluido

        [Theory(DisplayName = "Não deve criar tipo de item incluído se enum for inválido")]
        [InlineData((TipoItemIncluidoEnum)0)]
        [InlineData((TipoItemIncluidoEnum)3)]
        [InlineData((TipoItemIncluidoEnum)999)]
        [InlineData((TipoItemIncluidoEnum)(-1))]
        [Trait("ValueObject", "TipoItemIncluido")]
        public void TipoItemIncluido_ComEnumInvalido_DeveLancarExcecao(TipoItemIncluidoEnum tipoInvalido)
        {
            // Act & Assert
            FluentActions.Invoking(() => new TipoItemIncluido(tipoInvalido))
                .Should().Throw<DomainException>()
                .WithMessage("*Tipo de item incluí­do na Ordem de Serviço*não é válido*");
        }

        [Theory(DisplayName = "Deve aceitar tipos de item incluído válidos")]
        [InlineData(TipoItemIncluidoEnum.Peca)]
        [InlineData(TipoItemIncluidoEnum.Insumo)]
        [Trait("ValueObject", "TipoItemIncluido")]
        public void TipoItemIncluido_ComEnumValido_DeveAceitarTipo(TipoItemIncluidoEnum tipoValido)
        {
            // Act
            var tipo = new TipoItemIncluido(tipoValido);

            // Assert
            tipo.Valor.Should().Be(tipoValido);
        }

        #endregion
    }
}
