using Application.Estoque.Interfaces;
using Application.OrdemServico.DTO.External;
using Domain.Estoque.Aggregates;
using Domain.Estoque.Enums;
using Domain.OrdemServico.Enums;
using FluentAssertions;
using Infrastructure.AntiCorruptionLayer.OrdemServico;
using Moq;
using Shared.Enums;
using Shared.Exceptions;

namespace Tests.Application.OrdemServico
{
    public class EstoqueExternalServiceUnitTest
    {
        private readonly Mock<IItemEstoqueRepository> _itemEstoqueRepositoryMock;
        private readonly EstoqueExternalService _service;

        public EstoqueExternalServiceUnitTest()
        {
            _itemEstoqueRepositoryMock = new Mock<IItemEstoqueRepository>();
            _service = new EstoqueExternalService(_itemEstoqueRepositoryMock.Object);
        }

        #region Método ObterItemEstoquePorIdAsync Tests

        [Fact(DisplayName = "Deve retornar item de estoque quando existir")]
        [Trait("Método", "ObterItemEstoquePorIdAsync")]
        public async Task ObterItemEstoquePorIdAsync_DeveRetornarItem_QuandoExistir()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var nomeItem = "Filtro de Óleo";
            var precoItem = 25.50m;
            var quantidadeItem = 10;
            var tipoItem = TipoItemEstoqueEnum.Peca;

            var item = ItemEstoque.Criar(nomeItem, quantidadeItem, tipoItem, precoItem);

            _itemEstoqueRepositoryMock.Setup(r => r.ObterPorIdAsync(itemId))
                .ReturnsAsync(item);

            // Act
            var resultado = await _service.ObterItemEstoquePorIdAsync(itemId);

            // Assert
            resultado.Should().NotBeNull();
            resultado!.Id.Should().Be(item.Id);
            resultado.Nome.Should().Be(nomeItem);
            resultado.Preco.Should().Be(precoItem);
            resultado.Quantidade.Should().Be(quantidadeItem);
            resultado.TipoItemIncluido.Should().Be(TipoItemIncluidoEnum.Peca);

            _itemEstoqueRepositoryMock.Verify(r => r.ObterPorIdAsync(itemId), Times.Once);
        }

        [Fact(DisplayName = "Deve retornar null quando item não existir")]
        [Trait("Método", "ObterItemEstoquePorIdAsync")]
        public async Task ObterItemEstoquePorIdAsync_DeveRetornarNull_QuandoItemNaoExistir()
        {
            // Arrange
            var itemId = Guid.NewGuid();

            _itemEstoqueRepositoryMock.Setup(r => r.ObterPorIdAsync(itemId))
                .ReturnsAsync((ItemEstoque?)null);

            // Act
            var resultado = await _service.ObterItemEstoquePorIdAsync(itemId);

            // Assert
            resultado.Should().BeNull();

            _itemEstoqueRepositoryMock.Verify(r => r.ObterPorIdAsync(itemId), Times.Once);
        }

        [Fact(DisplayName = "Deve converter corretamente TipoItemEstoque.Peca para TipoItemIncluido.Peca")]
        [Trait("Método", "ObterItemEstoquePorIdAsync")]
        public async Task ObterItemEstoquePorIdAsync_DeveConverterCorretamente_TipoItemEstoquePecaParaTipoItemIncluido()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var item = ItemEstoque.Criar("Pastilha de Freio", 5, TipoItemEstoqueEnum.Peca, 45.00m);

            _itemEstoqueRepositoryMock.Setup(r => r.ObterPorIdAsync(itemId))
                .ReturnsAsync(item);

            // Act
            var resultado = await _service.ObterItemEstoquePorIdAsync(itemId);

            // Assert
            resultado.Should().NotBeNull();
            resultado!.TipoItemIncluido.Should().Be(TipoItemIncluidoEnum.Peca);
        }

        [Fact(DisplayName = "Deve converter corretamente TipoItemEstoque.Insumo para TipoItemIncluido.Insumo")]
        [Trait("Método", "ObterItemEstoquePorIdAsync")]
        public async Task ObterItemEstoquePorIdAsync_DeveConverterCorretamente_TipoItemEstoqueInsumoParaTipoItemIncluido()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var item = ItemEstoque.Criar("Óleo Motor", 8, TipoItemEstoqueEnum.Insumo, 35.00m);

            _itemEstoqueRepositoryMock.Setup(r => r.ObterPorIdAsync(itemId))
                .ReturnsAsync(item);

            // Act
            var resultado = await _service.ObterItemEstoquePorIdAsync(itemId);

            // Assert
            resultado.Should().NotBeNull();
            resultado!.TipoItemIncluido.Should().Be(TipoItemIncluidoEnum.Insumo);
        }

        [Fact(DisplayName = "Deve mapear corretamente todos os campos do item para o DTO")]
        [Trait("Método", "ObterItemEstoquePorIdAsync")]
        public async Task ObterItemEstoquePorIdAsync_DeveMappearCorretamente_CamposDoItemParaDTO()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var nomeEsperado = "Vela de Ignição";
            var precoEsperado = 12.75m;
            var quantidadeEsperada = 20;
            var tipoEsperado = TipoItemEstoqueEnum.Peca;

            var item = ItemEstoque.Criar(nomeEsperado, quantidadeEsperada, tipoEsperado, precoEsperado);

            _itemEstoqueRepositoryMock.Setup(r => r.ObterPorIdAsync(itemId))
                .ReturnsAsync(item);

            // Act
            var resultado = await _service.ObterItemEstoquePorIdAsync(itemId);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeOfType<ItemEstoqueExternalDTO>();
            resultado!.Id.Should().Be(item.Id);
            resultado.Nome.Should().Be(item.Nome.Valor);
            resultado.Preco.Should().Be(item.Preco.Valor);
            resultado.Quantidade.Should().Be(item.Quantidade.Valor);
            resultado.TipoItemIncluido.Should().Be(TipoItemIncluidoEnum.Peca);
        }

        #endregion

        #region VerificarDisponibilidadeAsync Tests

        [Fact(DisplayName = "Deve retornar true quando item existe e tem disponibilidade suficiente")]
        [Trait("Método", "VerificarDisponibilidadeAsync")]
        public async Task VerificarDisponibilidadeAsync_DeveRetornarTrue_QuandoItemExisteETemDisponibilidade()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var quantidadeNecessaria = 3;
            var item = ItemEstoque.Criar("Filtro de Ar", 5, TipoItemEstoqueEnum.Peca, 20.00m);

            _itemEstoqueRepositoryMock.Setup(r => r.ObterPorIdAsync(itemId))
                .ReturnsAsync(item);

            // Act
            var resultado = await _service.VerificarDisponibilidadeAsync(itemId, quantidadeNecessaria);

            // Assert
            resultado.Should().BeTrue();

            _itemEstoqueRepositoryMock.Verify(r => r.ObterPorIdAsync(itemId), Times.Once);
        }

        [Fact(DisplayName = "Deve retornar false quando item existe mas não tem disponibilidade suficiente")]
        [Trait("Método", "VerificarDisponibilidadeAsync")]
        public async Task VerificarDisponibilidadeAsync_DeveRetornarFalse_QuandoItemExisteMasNaoTemDisponibilidade()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var quantidadeNecessaria = 10;
            var item = ItemEstoque.Criar("Correia Dentada", 2, TipoItemEstoqueEnum.Peca, 80.00m);

            _itemEstoqueRepositoryMock.Setup(r => r.ObterPorIdAsync(itemId))
                .ReturnsAsync(item);

            // Act
            var resultado = await _service.VerificarDisponibilidadeAsync(itemId, quantidadeNecessaria);

            // Assert
            resultado.Should().BeFalse();

            _itemEstoqueRepositoryMock.Verify(r => r.ObterPorIdAsync(itemId), Times.Once);
        }

        [Fact(DisplayName = "Deve retornar false quando item não existir")]
        [Trait("Método", "VerificarDisponibilidadeAsync")]
        public async Task VerificarDisponibilidadeAsync_DeveRetornarFalse_QuandoItemNaoExistir()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var quantidadeNecessaria = 1;

            _itemEstoqueRepositoryMock.Setup(r => r.ObterPorIdAsync(itemId))
                .ReturnsAsync((ItemEstoque?)null);

            // Act
            var resultado = await _service.VerificarDisponibilidadeAsync(itemId, quantidadeNecessaria);

            // Assert
            resultado.Should().BeFalse();

            _itemEstoqueRepositoryMock.Verify(r => r.ObterPorIdAsync(itemId), Times.Once);
        }

        [Fact(DisplayName = "Deve retornar true quando quantidade necessária for igual à disponível")]
        [Trait("Método", "VerificarDisponibilidadeAsync")]
        public async Task VerificarDisponibilidadeAsync_DeveRetornarTrue_QuandoQuantidadeNecessariaForIgualADisponivel()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var quantidadeDisponivel = 7;
            var quantidadeNecessaria = 7;
            var item = ItemEstoque.Criar("Bateria", quantidadeDisponivel, TipoItemEstoqueEnum.Peca, 150.00m);

            _itemEstoqueRepositoryMock.Setup(r => r.ObterPorIdAsync(itemId))
                .ReturnsAsync(item);

            // Act
            var resultado = await _service.VerificarDisponibilidadeAsync(itemId, quantidadeNecessaria);

            // Assert
            resultado.Should().BeTrue();

            _itemEstoqueRepositoryMock.Verify(r => r.ObterPorIdAsync(itemId), Times.Once);
        }

        #endregion

        #region AtualizarQuantidadeEstoqueAsync Tests

        [Fact(DisplayName = "Deve atualizar quantidade quando item existir")]
        [Trait("Método", "AtualizarQuantidadeEstoqueAsync")]
        public async Task AtualizarQuantidadeEstoqueAsync_DeveAtualizarQuantidade_QuandoItemExistir()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var novaQuantidade = 15;
            var item = ItemEstoque.Criar("Amortecedor", 8, TipoItemEstoqueEnum.Peca, 200.00m);

            _itemEstoqueRepositoryMock.Setup(r => r.ObterPorIdAsync(itemId))
                .ReturnsAsync(item);

            _itemEstoqueRepositoryMock.Setup(r => r.AtualizarAsync(It.IsAny<ItemEstoque>()))
                .ReturnsAsync(item);

            // Act
            await _service.AtualizarQuantidadeEstoqueAsync(itemId, novaQuantidade);

            // Assert
            item.Quantidade.Valor.Should().Be(novaQuantidade);

            _itemEstoqueRepositoryMock.Verify(r => r.ObterPorIdAsync(itemId), Times.Once);
            _itemEstoqueRepositoryMock.Verify(r => r.AtualizarAsync(item), Times.Once);
        }

        [Fact(DisplayName = "Deve lançar exceção quando item não existir")]
        [Trait("Método", "AtualizarQuantidadeEstoqueAsync")]
        public async Task AtualizarQuantidadeEstoqueAsync_DeveLancarExcecao_QuandoItemNaoExistir()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var novaQuantidade = 10;

            _itemEstoqueRepositoryMock.Setup(r => r.ObterPorIdAsync(itemId))
                .ReturnsAsync((ItemEstoque?)null);

            // Act
            var act = async () => await _service.AtualizarQuantidadeEstoqueAsync(itemId, novaQuantidade);

            // Assert
            await act.Should().ThrowAsync<DomainException>()
                .WithMessage($"Item de estoque com ID {itemId} não encontrado.");

            _itemEstoqueRepositoryMock.Verify(r => r.ObterPorIdAsync(itemId), Times.Once);
            _itemEstoqueRepositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<ItemEstoque>()), Times.Never);
        }

        [Fact(DisplayName = "Deve chamar AtualizarAsync do repositório após atualizar quantidade")]
        [Trait("Método", "AtualizarQuantidadeEstoqueAsync")]
        public async Task AtualizarQuantidadeEstoqueAsync_DeveChamarAtualizarAsync_AposAtualizarQuantidade()
        {
            // Arrange
            var itemId = Guid.NewGuid();
            var novaQuantidade = 25;
            var item = ItemEstoque.Criar("Pneu", 4, TipoItemEstoqueEnum.Peca, 300.00m);

            _itemEstoqueRepositoryMock.Setup(r => r.ObterPorIdAsync(itemId))
                .ReturnsAsync(item);

            _itemEstoqueRepositoryMock.Setup(r => r.AtualizarAsync(It.IsAny<ItemEstoque>()))
                .ReturnsAsync(item);

            // Act
            await _service.AtualizarQuantidadeEstoqueAsync(itemId, novaQuantidade);

            // Assert
            _itemEstoqueRepositoryMock.Verify(r => r.AtualizarAsync(It.Is<ItemEstoque>(i => 
                i.Id == item.Id && i.Quantidade.Valor == novaQuantidade)), Times.Once);
        }

        #endregion
    }
}
