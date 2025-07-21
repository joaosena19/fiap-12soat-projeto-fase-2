using Domain.Cadastros.Aggregates;
using FluentAssertions;
using Shared.Exceptions;

namespace Tests.Domain.Cadastros
{
    public class ServicoTests
    {
        [Theory(DisplayName = "Não deve criar novo Serviço se o Nome for inválido")]
        [InlineData("")]
        [InlineData("nome_com_mais_de_500_caracteres__________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________")]
        [Trait("Metodo", "Criar")]
        public void ServicoCriar_Deve_ThrowException_Quando_NomeInvalido(string nomeInvalido)
        {
            // Arrange
            var precoValido = 100.00M;

            // Act & Assert
            FluentActions.Invoking(() => Servico.Criar(nomeInvalido, precoValido))
                .Should().Throw<DomainException>()
                .WithMessage("*nome não pode*");
        }

        [Theory(DisplayName = "Não deve criar novo Serviço se o Preço for inválido")]
        [InlineData(-0.01)]
        [InlineData(-100.00)]
        [InlineData(-1)]
        [Trait("Metodo", "Criar")]
        public void ServicoCriar_Deve_ThrowException_Quando_PrecoInvalido(decimal precoInvalido)
        {
            // Arrange
            var nomeValido = "Troca de óleo";

            // Act & Assert
            FluentActions.Invoking(() => Servico.Criar(nomeValido, precoInvalido))
                .Should().Throw<DomainException>()
                .WithMessage("*Preço não pode ser negativo*");
        }

        [Fact(DisplayName = "Deve criar novo Serviço com dados válidos")]
        [Trait("Metodo", "Criar")]
        public void ServicoCriar_Deve_CriarServico_Quando_DadosValidos()
        {
            // Arrange
            var nome = "Troca de óleo";
            var preco = 150.00M;

            // Act
            var servico = Servico.Criar(nome, preco);

            // Assert
            servico.Should().NotBeNull();
            servico.Id.Should().NotBe(Guid.Empty);
            servico.Nome.Valor.Should().Be(nome);
            servico.Preco.Valor.Should().Be(preco);
        }

        [Theory(DisplayName = "Deve aceitar preços válidos")]
        [InlineData(0)]
        [InlineData(0.01)]
        [InlineData(100.50)]
        [InlineData(999999.99)]
        [Trait("Metodo", "Criar")]
        public void ServicoCriar_Deve_AceitarPrecos_Quando_PrecoValido(decimal precoValido)
        {
            // Arrange
            var nome = "Serviço de teste";

            // Act
            var servico = Servico.Criar(nome, precoValido);

            // Assert
            servico.Should().NotBeNull();
            servico.Preco.Valor.Should().Be(precoValido);
        }

        [Fact(DisplayName = "Deve atualizar serviço com dados válidos")]
        [Trait("Metodo", "Atualizar")]
        public void ServicoAtualizar_Deve_AtualizarServico_Quando_DadosValidos()
        {
            // Arrange
            var nomeOriginal = "Troca de óleo";
            var precoOriginal = 100.00M;
            var novoNome = "Troca de óleo premium";
            var novoPreco = 200.00M;

            var servico = Servico.Criar(nomeOriginal, precoOriginal);

            // Act
            servico.Atualizar(novoNome, novoPreco);

            // Assert
            servico.Nome.Valor.Should().Be(novoNome);
            servico.Preco.Valor.Should().Be(novoPreco);
        }

        [Theory(DisplayName = "Não deve atualizar serviço se o nome for inválido")]
        [InlineData("")]
        [InlineData("nome_com_mais_de_500_caracteres__________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________________")]
        [Trait("Metodo", "Atualizar")]
        public void ServicoAtualizar_Deve_ThrowException_Quando_NomeInvalido(string nomeInvalido)
        {
            // Arrange
            var servico = Servico.Criar("Troca de óleo", 100.00M);
            var precoValido = 150.00M;

            // Act & Assert
            FluentActions.Invoking(() => servico.Atualizar(nomeInvalido, precoValido))
                .Should().Throw<DomainException>()
                .WithMessage("*nome não pode*");
        }

        [Theory(DisplayName = "Não deve atualizar serviço se o preço for inválido")]
        [InlineData(-0.01)]
        [InlineData(-100.00)]
        [Trait("Metodo", "Atualizar")]
        public void ServicoAtualizar_Deve_ThrowException_Quando_PrecoInvalido(decimal precoInvalido)
        {
            // Arrange
            var servico = Servico.Criar("Troca de óleo", 100.00M);
            var nomeValido = "Troca de óleo premium";

            // Act & Assert
            FluentActions.Invoking(() => servico.Atualizar(nomeValido, precoInvalido))
                .Should().Throw<DomainException>()
                .WithMessage("*Preço não pode ser negativo*");
        }
    }
}
