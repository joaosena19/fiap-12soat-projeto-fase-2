using Domain.Estoque.Aggregates;
using Domain.Estoque.Enums;
using FluentAssertions;
using Shared.Exceptions;

namespace Tests.Domain.Estoque
{
    public class ItemEstoqueUnitTest
    {
        [Fact(DisplayName = "Deve criar item de estoque com dados válidos")]
        [Trait("Dados Válidos", "Criar")]
        public void Criar_DeveCriarItemEstoqueComDadosValidos()
        {
            // Arrange
            var nome = "Filtro de Óleo";
            var quantidade = 50;
            var tipoItemEstoque = TipoItemEstoqueEnum.Peca;

            // Act
            var itemEstoque = ItemEstoque.Criar(nome, quantidade, tipoItemEstoque);

            // Assert
            itemEstoque.Should().NotBeNull();
            itemEstoque.Id.Should().NotBeEmpty();
            itemEstoque.Nome.Valor.Should().Be(nome);
            itemEstoque.Quantidade.Valor.Should().Be(quantidade);
            itemEstoque.TipoItemEstoque.Valor.Should().Be(tipoItemEstoque.ToString().ToLower());
        }

        [Fact(DisplayName = "Deve atualizar item de estoque com dados válidos")]
        [Trait("Dados Válidos", "Atualizar")]
        public void Atualizar_DeveAtualizarItemEstoqueComDadosValidos()
        {
            // Arrange
            var nomeOriginal = "Filtro de Óleo";
            var quantidadeOriginal = 50;
            var tipoOriginal = TipoItemEstoqueEnum.Peca;
            
            var novoNome = "Filtro de Óleo Premium";
            var novaQuantidade = 75;
            var novoTipo = TipoItemEstoqueEnum.Insumo;

            var itemEstoque = ItemEstoque.Criar(nomeOriginal, quantidadeOriginal, tipoOriginal);

            // Act
            itemEstoque.Atualizar(novoNome, novaQuantidade, novoTipo);

            // Assert
            itemEstoque.Nome.Valor.Should().Be(novoNome);
            itemEstoque.Quantidade.Valor.Should().Be(novaQuantidade);
            itemEstoque.TipoItemEstoque.Valor.Should().Be(novoTipo.ToString().ToLower());
        }

        [Fact(DisplayName = "Deve atualizar apenas a quantidade do item de estoque")]
        [Trait("Dados Válidos", "AtualizarQuantidade")]
        public void AtualizarQuantidade_DeveAtualizarApenasQuantidade()
        {
            // Arrange
            var nome = "Filtro de Óleo";
            var quantidadeOriginal = 50;
            var novaQuantidade = 100;
            var tipoItemEstoque = TipoItemEstoqueEnum.Peca;

            var itemEstoque = ItemEstoque.Criar(nome, quantidadeOriginal, tipoItemEstoque);

            // Act
            itemEstoque.AtualizarQuantidade(novaQuantidade);

            // Assert
            itemEstoque.Nome.Valor.Should().Be(nome); // Nome não deve mudar
            itemEstoque.Quantidade.Valor.Should().Be(novaQuantidade);
            itemEstoque.TipoItemEstoque.Valor.Should().Be(tipoItemEstoque.ToString().ToLower()); // Tipo não deve mudar
        }

        [Theory(DisplayName = "Deve verificar disponibilidade corretamente")]
        [InlineData(50, 30, true)]   // Estoque 50, solicita 30 - disponível
        [InlineData(50, 50, true)]   // Estoque 50, solicita 50 - disponível
        [InlineData(50, 51, false)]  // Estoque 50, solicita 51 - não disponível
        [InlineData(10, 15, false)]  // Estoque 10, solicita 15 - não disponível
        [Trait("Regra de Negócio", "VerificarDisponibilidade")]
        public void VerificarDisponibilidade_DeveRetornarStatusCorreto(int quantidadeEstoque, int quantidadeSolicitada, bool esperado)
        {
            // Arrange
            var itemEstoque = ItemEstoque.Criar("Filtro de Óleo", quantidadeEstoque, TipoItemEstoqueEnum.Peca);

            // Act
            var disponivel = itemEstoque.VerificarDisponibilidade(quantidadeSolicitada);

            // Assert
            disponivel.Should().Be(esperado);
        }

        [Theory(DisplayName = "Não deve verificar disponibilidade com quantidade inválida")]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-10)]
        [Trait("Regra de Negócio", "VerificarDisponibilidade")]
        public void VerificarDisponibilidade_DeveLancarExcecao_QuandoQuantidadeInvalida(int quantidadeInvalida)
        {
            // Arrange
            var itemEstoque = ItemEstoque.Criar("Filtro de Óleo", 50, TipoItemEstoqueEnum.Peca);

            // Act & Assert
            FluentActions.Invoking(() => itemEstoque.VerificarDisponibilidade(quantidadeInvalida))
                .Should().Throw<DomainException>()
                .WithMessage("Quantidade requisitada deve ser maior que 0");
        }

        #region Testes ValueObject Nome

        [Theory(DisplayName = "Não deve criar item de estoque se o nome for inválido")]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("nome_com_mais_de_200_caracteres__________________________________________________________________________________________________________________________________________________________________________")]
        [Trait("ValueObject", "Nome")]
        public void Criar_ComNomeInvalido_DeveLancarExcecao(string nomeInvalido)
        {
            // Arrange
            var quantidadeValida = 10;
            var tipoValido = TipoItemEstoqueEnum.Peca;

            // Act & Assert
            FluentActions.Invoking(() => ItemEstoque.Criar(nomeInvalido, quantidadeValida, tipoValido))
                .Should().Throw<DomainException>()
                .WithMessage("*Nome não pode*");
        }

        [Fact(DisplayName = "Não deve criar item de estoque se o nome for nulo")]
        [Trait("ValueObject", "Nome")]
        public void Criar_ComNomeNulo_DeveLancarExcecao()
        {
            // Arrange
            string nomeNulo = null!;
            var quantidadeValida = 10;
            var tipoValido = TipoItemEstoqueEnum.Peca;

            // Act & Assert
            FluentActions.Invoking(() => ItemEstoque.Criar(nomeNulo, quantidadeValida, tipoValido))
                .Should().Throw<DomainException>()
                .WithMessage("*Nome não pode*");
        }

        [Theory(DisplayName = "Não deve atualizar item de estoque se o nome for inválido")]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("nome_com_mais_de_200_caracteres__________________________________________________________________________________________________________________________________________________________________________")]
        [Trait("ValueObject", "Nome")]
        public void Atualizar_ComNomeInvalido_DeveLancarExcecao(string nomeInvalido)
        {
            // Arrange
            var itemEstoque = ItemEstoque.Criar("Filtro de Óleo", 50, TipoItemEstoqueEnum.Peca);

            // Act & Assert
            FluentActions.Invoking(() => itemEstoque.Atualizar(nomeInvalido, 50, TipoItemEstoqueEnum.Peca))
                .Should().Throw<DomainException>()
                .WithMessage("*Nome não pode*");
        }

        #endregion

        #region Testes ValueObject Quantidade

        [Theory(DisplayName = "Não deve criar item de estoque se a quantidade for inválida")]
        [InlineData(-1)]
        [InlineData(-10)]
        [InlineData(-100)]
        [Trait("ValueObject", "Quantidade")]
        public void Criar_ComQuantidadeInvalida_DeveLancarExcecao(int quantidadeInvalida)
        {
            // Arrange
            var nomeValido = "Filtro de Óleo";
            var tipoValido = TipoItemEstoqueEnum.Peca;

            // Act & Assert
            FluentActions.Invoking(() => ItemEstoque.Criar(nomeValido, quantidadeInvalida, tipoValido))
                .Should().Throw<DomainException>()
                .WithMessage("Quantidade não pode ser negativa");
        }

        [Theory(DisplayName = "Deve aceitar quantidades válidas")]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(50)]
        [InlineData(1000)]
        [Trait("ValueObject", "Quantidade")]
        public void Criar_ComQuantidadeValida_DeveAceitarQuantidade(int quantidadeValida)
        {
            // Arrange
            var nomeValido = "Filtro de Óleo";
            var tipoValido = TipoItemEstoqueEnum.Peca;

            // Act
            var itemEstoque = ItemEstoque.Criar(nomeValido, quantidadeValida, tipoValido);

            // Assert
            itemEstoque.Quantidade.Valor.Should().Be(quantidadeValida);
        }

        [Theory(DisplayName = "Não deve atualizar item de estoque se a quantidade for inválida")]
        [InlineData(-1)]
        [InlineData(-10)]
        [Trait("ValueObject", "Quantidade")]
        public void Atualizar_ComQuantidadeInvalida_DeveLancarExcecao(int quantidadeInvalida)
        {
            // Arrange
            var itemEstoque = ItemEstoque.Criar("Filtro de Óleo", 50, TipoItemEstoqueEnum.Peca);

            // Act & Assert
            FluentActions.Invoking(() => itemEstoque.Atualizar("Filtro de Óleo", quantidadeInvalida, TipoItemEstoqueEnum.Peca))
                .Should().Throw<DomainException>()
                .WithMessage("Quantidade não pode ser negativa");
        }

        [Theory(DisplayName = "Não deve atualizar quantidade se for inválida")]
        [InlineData(-1)]
        [InlineData(-10)]
        [Trait("ValueObject", "Quantidade")]
        public void AtualizarQuantidade_ComQuantidadeInvalida_DeveLancarExcecao(int quantidadeInvalida)
        {
            // Arrange
            var itemEstoque = ItemEstoque.Criar("Filtro de Óleo", 50, TipoItemEstoqueEnum.Peca);

            // Act & Assert
            FluentActions.Invoking(() => itemEstoque.AtualizarQuantidade(quantidadeInvalida))
                .Should().Throw<DomainException>()
                .WithMessage("Quantidade não pode ser negativa");
        }

        #endregion

        #region Testes ValueObject TipoItemEstoque

        [Theory(DisplayName = "Não deve criar item de estoque se o tipo for inválido")]
        [InlineData((TipoItemEstoqueEnum)0)]
        [InlineData((TipoItemEstoqueEnum)3)]
        [InlineData((TipoItemEstoqueEnum)999)]
        [InlineData((TipoItemEstoqueEnum)(-1))]
        [Trait("ValueObject", "TipoItemEstoque")]
        public void Criar_ComTipoItemEstoqueInvalido_DeveLancarExcecao(TipoItemEstoqueEnum tipoInvalido)
        {
            // Arrange
            var nomeValido = "Filtro de Óleo";
            var quantidadeValida = 50;

            // Act & Assert
            FluentActions.Invoking(() => ItemEstoque.Criar(nomeValido, quantidadeValida, tipoInvalido))
                .Should().Throw<DomainException>()
                .WithMessage("*Tipo de item de estoque*não é válido*");
        }

        [Theory(DisplayName = "Deve aceitar tipos de item de estoque válidos")]
        [InlineData(TipoItemEstoqueEnum.Peca)]
        [InlineData(TipoItemEstoqueEnum.Insumo)]
        [Trait("ValueObject", "TipoItemEstoque")]
        public void Criar_ComTipoItemEstoqueValido_DeveAceitarTipo(TipoItemEstoqueEnum tipoValido)
        {
            // Arrange
            var nomeValido = "Filtro de Óleo";
            var quantidadeValida = 50;

            // Act
            var itemEstoque = ItemEstoque.Criar(nomeValido, quantidadeValida, tipoValido);

            // Assert
            itemEstoque.TipoItemEstoque.Valor.Should().Be(tipoValido.ToString().ToLower());
        }

        [Theory(DisplayName = "Não deve atualizar item de estoque se o tipo for inválido")]
        [InlineData((TipoItemEstoqueEnum)0)]
        [InlineData((TipoItemEstoqueEnum)3)]
        [InlineData((TipoItemEstoqueEnum)999)]
        [Trait("ValueObject", "TipoItemEstoque")]
        public void Atualizar_ComTipoItemEstoqueInvalido_DeveLancarExcecao(TipoItemEstoqueEnum tipoInvalido)
        {
            // Arrange
            var itemEstoque = ItemEstoque.Criar("Filtro de Óleo", 50, TipoItemEstoqueEnum.Peca);

            // Act & Assert
            FluentActions.Invoking(() => itemEstoque.Atualizar("Filtro de Óleo", 50, tipoInvalido))
                .Should().Throw<DomainException>()
                .WithMessage("*Tipo de item de estoque*não é válido*");
        }

        #endregion
    }
}
