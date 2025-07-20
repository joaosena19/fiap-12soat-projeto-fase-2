using Domain.Cadastros.Aggregates;
using FluentAssertions;
using Shared.Exceptions;

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
            var cpfValido = "36050793000";

            // Act & Assert
            FluentActions.Invoking(() => Cliente.Criar(nomeInvalido, cpfValido))
                .Should().Throw<DomainException>()
                .WithMessage("*nome não pode*");
        }

        [Theory(DisplayName = "Não deve criar novo Cliente se o CPF for inválido")]
        [InlineData("")]
        [InlineData("01234567891")]
        [Trait("Cliente.Criar", "Unidade")]
        public void ClienteCriar_Deve_ThrowException_Quando_CpfInvalido(string cpfInvalido)
        {
            // Arrange
            var nomeValido = "João";

            // Act & Assert
            FluentActions.Invoking(() => Cliente.Criar(nomeValido, cpfInvalido))
                .Should().Throw<DomainException>()
                .WithMessage("*CPF inválido*");
        }
    }

}
