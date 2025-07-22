using Domain.Cadastros.Aggregates;
using Domain.Cadastros.Enums;
using Domain.Cadastros.ValueObjects.Veiculo;
using FluentAssertions;
using Shared.Exceptions;

namespace Tests.Domain.Cadastros
{
    public class VeiculoUnitTest
    {
        [Fact(DisplayName = "Deve criar veículo com dados válidos")]
        [Trait("Dados Válidos", "Criar")]
        public void Criar_DeveCriarVeiculoComDadosValidos()
        {
            // Arrange
            var placa = "ABC1234";
            var modelo = "Civic";
            var marca = "Honda";
            var cor = "Preto";
            var ano = 2020;
            var tipoVeiculo = TipoVeiculoEnum.Carro;

            // Act
            var veiculo = Veiculo.Criar(placa, modelo, marca, cor, ano, tipoVeiculo);

            // Assert
            veiculo.Should().NotBeNull();
            veiculo.Id.Should().NotBeEmpty();
            veiculo.Placa.Valor.Should().Be(placa);
            veiculo.Modelo.Valor.Should().Be(modelo);
            veiculo.Marca.Valor.Should().Be(marca);
            veiculo.Cor.Valor.Should().Be(cor);
            veiculo.Ano.Valor.Should().Be(ano);
            veiculo.TipoVeiculo.Valor.Should().Be(tipoVeiculo.ToString().ToLower());
        }

        [Fact(DisplayName = "Deve atualizar veículo com dados válidos")]
        [Trait("Dados Válidos", "Atualizar")]
        public void Atualizar_DeveAtualizarVeiculoComDadosValidos()
        {
            // Arrange
            var veiculo = Veiculo.Criar("ABC1234", "Civic", "Honda", "Preto", 2020, TipoVeiculoEnum.Carro);
            var novoModelo = "Corolla";
            var novaMarca = "Toyota";
            var novaCor = "Branco";
            var novoAno = 2021;
            var novoTipo = TipoVeiculoEnum.Carro;

            // Act
            veiculo.Atualizar(novoModelo, novaMarca, novaCor, novoAno, novoTipo);

            // Assert
            veiculo.Modelo.Valor.Should().Be(novoModelo);
            veiculo.Marca.Valor.Should().Be(novaMarca);
            veiculo.Cor.Valor.Should().Be(novaCor);
            veiculo.Ano.Valor.Should().Be(novoAno);
            veiculo.TipoVeiculo.Valor.Should().Be(novoTipo.ToString().ToLower());
        }

        [Theory(DisplayName = "Não deve criar veículo se a placa for inválida")]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("ABC123")] // 6 caracteres
        [InlineData("ABC12345")] // 8 caracteres
        [InlineData("ABC-123")] // Com hífen mas não 7 caracteres após remoção
        [Trait("ValueObject", "Placa")]
        public void Criar_ComPlacaInvalida_DeveLancarExcecao(string placaInvalida)
        {
            // Act & Assert
            Action act = () => Veiculo.Criar(placaInvalida, "Civic", "Honda", "Preto", 2020, TipoVeiculoEnum.Carro);
            act.Should().Throw<DomainException>()
                .WithMessage("*Placa*");
        }

        [Theory(DisplayName = "Não deve criar veículo se o modelo for inválido")]
        [InlineData("")]
        [InlineData("   ")]
        [Trait("ValueObject", "Modelo")]
        public void Criar_ComModeloInvalido_DeveLancarExcecao(string modeloInvalido)
        {
            // Act & Assert
            Action act = () => Veiculo.Criar("ABC1234", modeloInvalido, "Honda", "Preto", 2020, TipoVeiculoEnum.Carro);
            act.Should().Throw<DomainException>()
                .WithMessage("*Modelo não pode*");
        }

        [Fact(DisplayName = "Não deve criar veículo se o modelo for nulo")]
        [Trait("ValueObject", "Modelo")]
        public void Criar_ComModeloNulo_DeveLancarExcecao()
        {
            // Act & Assert
            Action act = () => Veiculo.Criar("ABC1234", null!, "Honda", "Preto", 2020, TipoVeiculoEnum.Carro);
            act.Should().Throw<DomainException>()
                .WithMessage("*Modelo não pode*");
        }

        [Theory(DisplayName = "Não deve criar veículo se a marca for inválida")]
        [InlineData("")]
        [InlineData("   ")]
        [Trait("ValueObject", "Marca")]
        public void Criar_ComMarcaInvalida_DeveLancarExcecao(string marcaInvalida)
        {
            // Act & Assert
            Action act = () => Veiculo.Criar("ABC1234", "Civic", marcaInvalida, "Preto", 2020, TipoVeiculoEnum.Carro);
            act.Should().Throw<DomainException>()
                .WithMessage("*Marca não pode*");
        }

        [Fact(DisplayName = "Não deve criar veículo se a marca for nula")]
        [Trait("ValueObject", "Marca")]
        public void Criar_ComMarcaNula_DeveLancarExcecao()
        {
            // Act & Assert
            Action act = () => Veiculo.Criar("ABC1234", "Civic", null!, "Preto", 2020, TipoVeiculoEnum.Carro);
            act.Should().Throw<DomainException>()
                .WithMessage("*Marca não pode*");
        }

        [Theory(DisplayName = "Não deve criar veículo se a cor for inválida")]
        [InlineData("")]
        [InlineData("   ")]
        [Trait("ValueObject", "Cor")]
        public void Criar_ComCorInvalida_DeveLancarExcecao(string corInvalida)
        {
            // Act & Assert
            Action act = () => Veiculo.Criar("ABC1234", "Civic", "Honda", corInvalida, 2020, TipoVeiculoEnum.Carro);
            act.Should().Throw<DomainException>()
                .WithMessage("*Cor não pode*");
        }

        [Fact(DisplayName = "Não deve criar veículo se a cor for nula")]
        [Trait("ValueObject", "Cor")]
        public void Criar_ComCorNula_DeveLancarExcecao()
        {
            // Act & Assert
            Action act = () => Veiculo.Criar("ABC1234", "Civic", "Honda", null!, 2020, TipoVeiculoEnum.Carro);
            act.Should().Throw<DomainException>()
                .WithMessage("*Cor não pode*");
        }

        [Theory(DisplayName = "Não deve criar veículo se o ano for inválido")]
        [InlineData(1884)]
        [Trait("ValueObject", "Ano")]
        public void Criar_ComAnoInvalido_DeveLancarExcecao(int anoInvalido)
        {
            // Act & Assert
            Action act = () => Veiculo.Criar("ABC1234", "Civic", "Honda", "Preto", anoInvalido, TipoVeiculoEnum.Carro);
            act.Should().Throw<DomainException>()
                .WithMessage("*Ano deve estar entre*");
        }

        [Fact(DisplayName = "Não deve criar veículo se o ano for futuro demais")]
        [Trait("ValueObject", "Ano")]
        public void Criar_ComAnoFuturoInvalido_DeveLancarExcecao()
        {
            // Arrange
            var anoInvalido = DateTime.Now.Year + 2;

            // Act & Assert
            Action act = () => Veiculo.Criar("ABC1234", "Civic", "Honda", "Preto", anoInvalido, TipoVeiculoEnum.Carro);
            act.Should().Throw<DomainException>()
                .WithMessage("*Ano deve estar entre*");
        }

        [Theory(DisplayName = "Não deve criar veículo se o tipo de veículo for inválido")]
        [InlineData((TipoVeiculoEnum)999)]
        [InlineData((TipoVeiculoEnum)0)]
        [InlineData((TipoVeiculoEnum)3)]
        [InlineData((TipoVeiculoEnum)(-1))]
        [Trait("ValueObject", "TipoVeiculo")]
        public void Criar_ComTipoVeiculoInvalido_DeveLancarExcecao(TipoVeiculoEnum tipoInvalido)
        {
            // Act & Assert
            Action act = () => Veiculo.Criar("ABC1234", "Civic", "Honda", "Preto", 2020, tipoInvalido);
            act.Should().Throw<DomainException>()
                .WithMessage("*Tipo de veículo*não é válido*");
        }
    }
}
