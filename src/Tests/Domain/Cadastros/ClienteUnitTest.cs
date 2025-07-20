using Domain.Cadastros.Aggregates;
using Domain.Cadastros.ValueObjects.Cliente;
using FluentAssertions;

namespace Tests.Domain.Cadastros
{
    public class ClienteTests
    {
        [Theory(DisplayName = "Não deve criar novo Cliente se o Nome for inválido")]
        [InlineData("")]
        [InlineData("nome_com_mais_de_200_caracteres__________________________________________________________________________________________________________________________________________________________________________")]
        [Trait("Cliente.Criar", "Unidade")]
        public void ClienteCriar_Deve_ThrowException_Quando_NomeInvalido(string nomeInvalido)
        {
            // Arrange
            var cpfValido = new Cpf("36050793000");

            // Act & Assert
            FluentActions.Invoking(() => Cliente.Criar(new Nome(nomeInvalido), cpfValido))
                .Should().Throw<ArgumentException>()
                .WithMessage("*nome não pode*");
        }

        [Theory(DisplayName = "Não deve criar novo Cliente se o CPF for inválido")]
        [InlineData("")]
        [InlineData("01234567891")]
        [Trait("Cliente.Criar", "Unidade")]
        public void ClienteCriar_Deve_ThrowException_Quando_CpfInvalido(string cpfInvalido)
        {
            // Arrange
            var nomeValido = new Nome("João");

            // Act & Assert
            FluentActions.Invoking(() => Cliente.Criar(nomeValido, new Cpf(cpfInvalido)))
                .Should().Throw<ArgumentException>()
                .WithMessage("*CPF inválido*");
        }
    }

}
