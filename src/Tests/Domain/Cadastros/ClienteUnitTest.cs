using Domain.Cadastros.Aggregates;
using FluentAssertions;
using Shared.Exceptions;

namespace Tests.Domain.Cadastros
{
    public class ClienteTests
    {
        [Fact(DisplayName = "Deve criar novo Cliente com dados válidos")]
        [Trait("Dados Válidos", "Criar")]
        public void ClienteCriar_Deve_CriarCliente_Quando_DadosValidos()
        {
            // Arrange
            var nome = "João da Silva";
            var cpf = "36050793000";

            // Act
            var cliente = Cliente.Criar(nome, cpf);

            // Assert
            cliente.Should().NotBeNull();
            cliente.Id.Should().NotBe(Guid.Empty);
            cliente.Nome.Valor.Should().Be(nome);
            cliente.Cpf.Valor.Should().Be(cpf);
        }

        [Fact(DisplayName = "Deve atualizar cliente com dados válidos")]
        [Trait("Dados Válidos", "Atualizar")]
        public void ClienteAtualizar_Deve_AtualizarCliente_Quando_DadosValidos()
        {
            // Arrange
            var nomeOriginal = "João da Silva";
            var cpfOriginal = "36050793000";
            var novoNome = "João Silva Santos";

            var cliente = Cliente.Criar(nomeOriginal, cpfOriginal);

            // Act
            cliente.Atualizar(novoNome);

            // Assert
            cliente.Nome.Valor.Should().Be(novoNome);
            cliente.Cpf.Valor.Should().Be(cpfOriginal); // CPF não deve ter mudado
        }

        [Theory(DisplayName = "Não deve criar novo Cliente se o Nome for inválido")]
        [InlineData("")]
        [InlineData("nome_com_mais_de_200_caracteres__________________________________________________________________________________________________________________________________________________________________________")]
        [Trait("ValueObject", "Nome")]
        public void ClienteCriar_Deve_ThrowException_Quando_NomeInvalido(string nomeInvalido)
        {
            // Arrange
            var cpfValido = "36050793000";

            // Act & Assert
            FluentActions.Invoking(() => Cliente.Criar(nomeInvalido, cpfValido))
                .Should().Throw<DomainException>()
                .WithMessage("*nome não pode*");
        }

        [Theory(DisplayName = "Não deve criar novo Cliente se o CPF for inválido")]
        [InlineData("")]
        [InlineData("01234567891")]
        [Trait("ValueObject", "Cpf")]
        public void ClienteCriar_Deve_ThrowException_Quando_CpfInvalido(string cpfInvalido)
        {
            // Arrange
            var nomeValido = "João";

            // Act & Assert
            FluentActions.Invoking(() => Cliente.Criar(nomeValido, cpfInvalido))
                .Should().Throw<DomainException>()
                .WithMessage("*CPF inválido*");
        }

        [Theory(DisplayName = "Não deve atualizar cliente se o nome for inválido")]
        [InlineData("")]
        [InlineData("nome_com_mais_de_200_caracteres__________________________________________________________________________________________________________________________________________________________________________")]
        [Trait("ValueObject", "Nome")]
        public void ClienteAtualizar_Deve_ThrowException_Quando_NomeInvalido(string nomeInvalido)
        {
            // Arrange
            var cliente = Cliente.Criar("João da Silva", "36050793000");

            // Act & Assert
            FluentActions.Invoking(() => cliente.Atualizar(nomeInvalido))
                .Should().Throw<DomainException>()
                .WithMessage("*nome não pode*");
        }
    }

}
