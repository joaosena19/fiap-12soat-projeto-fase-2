using Application.Estoque.DTO;
using Application.Estoque.Interfaces;
using Application.Estoque.Services;
using AutoMapper;
using Domain.Estoque.Aggregates;
using Domain.Estoque.Enums;
using FluentAssertions;
using Moq;
using Shared.Exceptions;
using Application;

namespace Tests.Application.Estoque
{
    public class ItemEstoqueServiceUnitTest
    {
        private readonly Mock<IItemEstoqueRepository> _repoMock;
        private readonly IMapper _mapper;
        private readonly ItemEstoqueService _service;

        public ItemEstoqueServiceUnitTest()
        {
            _repoMock = new Mock<IItemEstoqueRepository>();
            _mapper = AutoMapperConfig.CreateMapper();
            _service = new ItemEstoqueService(_repoMock.Object, _mapper);
        }

        #region CriarItemEstoque Tests

        [Fact(DisplayName = "Não deve criar item de estoque se nome já existir")]
        [Trait("Metodo", "CriarItemEstoque")]
        public async Task CriarItemEstoque_DeveLancarExcecao_SeNomeJaExistir()
        {
            // Arrange
            var dto = new CriarItemEstoqueDTO
            {
                Nome = "Filtro de Óleo",
                Quantidade = 50,
                TipoItemEstoque = TipoItemEstoqueEnum.Peca,
                Preco = 25.50m
            };

            var itemExistente = ItemEstoque.Criar(dto.Nome, dto.Quantidade, dto.TipoItemEstoque, dto.Preco);

            _repoMock.Setup(r => r.ObterPorNomeAsync(dto.Nome))
                .ReturnsAsync(itemExistente);

            // Act
            var act = async () => await _service.CriarItemEstoque(dto);

            // Assert
            await act.Should().ThrowAsync<DomainException>()
                .WithMessage("Já existe um item de estoque cadastrado com este nome.");

            _repoMock.Verify(r => r.SalvarAsync(It.IsAny<ItemEstoque>()), Times.Never);
        }

        [Fact(DisplayName = "Deve criar item de estoque se nome for novo")]
        [Trait("Metodo", "CriarItemEstoque")]
        public async Task CriarItemEstoque_DeveSalvarItemEstoque_SeNomeNaoExistir()
        {
            // Arrange
            var dto = new CriarItemEstoqueDTO
            {
                Nome = "Filtro de Óleo",
                Quantidade = 50,
                TipoItemEstoque = TipoItemEstoqueEnum.Peca,
                Preco = 25.50m
            };

            var itemNovo = ItemEstoque.Criar(dto.Nome, dto.Quantidade, dto.TipoItemEstoque, dto.Preco);

            _repoMock.Setup(r => r.ObterPorNomeAsync(dto.Nome))
                .ReturnsAsync((ItemEstoque?)null);

            _repoMock.Setup(r => r.SalvarAsync(It.IsAny<ItemEstoque>()))
                .ReturnsAsync(itemNovo);

            // Act
            var resultado = await _service.CriarItemEstoque(dto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Nome.Should().Be(dto.Nome);
            resultado.Quantidade.Should().Be(dto.Quantidade);
            resultado.TipoItemEstoque.Should().Be(dto.TipoItemEstoque.ToString());
            resultado.Preco.Should().Be(dto.Preco);

            _repoMock.Verify(r => r.SalvarAsync(It.Is<ItemEstoque>(i =>
                i.Nome.Valor == dto.Nome && 
                i.Quantidade.Valor == dto.Quantidade &&
                i.TipoItemEstoque.Valor == dto.TipoItemEstoque &&
                i.Preco.Valor == dto.Preco
            )), Times.Once);
        }

        #endregion

        #region AtualizarItemEstoque Tests

        [Fact(DisplayName = "Deve atualizar item de estoque se existir")]
        [Trait("Metodo", "AtualizarItemEstoque")]
        public async Task AtualizarItemEstoque_DeveAtualizarItemEstoque_SeItemExistir()
        {
            // Arrange
            var id = Guid.NewGuid();
            var dto = new AtualizarItemEstoqueDTO
            {
                Nome = "Filtro de Óleo Premium",
                Quantidade = 75,
                TipoItemEstoque = TipoItemEstoqueEnum.Insumo,
                Preco = 35.75m
            };

            var itemExistente = ItemEstoque.Criar("Filtro de Óleo", 50, TipoItemEstoqueEnum.Peca, 25.50m);
            var itemAtualizado = ItemEstoque.Criar(dto.Nome, dto.Quantidade, dto.TipoItemEstoque, dto.Preco);

            _repoMock.Setup(r => r.ObterPorIdAsync(id))
                .ReturnsAsync(itemExistente);

            _repoMock.Setup(r => r.AtualizarAsync(It.IsAny<ItemEstoque>()))
                .ReturnsAsync(itemAtualizado);

            // Act
            var resultado = await _service.AtualizarItemEstoque(id, dto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Nome.Should().Be(dto.Nome);
            resultado.Quantidade.Should().Be(dto.Quantidade);
            resultado.TipoItemEstoque.Should().Be(dto.TipoItemEstoque.ToString());
            resultado.Preco.Should().Be(dto.Preco);

            _repoMock.Verify(r => r.AtualizarAsync(It.IsAny<ItemEstoque>()), Times.Once);
        }

        [Fact(DisplayName = "Não deve atualizar item de estoque se não existir")]
        [Trait("Metodo", "AtualizarItemEstoque")]
        public async Task AtualizarItemEstoque_DeveLancarExcecao_SeItemNaoExistir()
        {
            // Arrange
            var id = Guid.NewGuid();
            var dto = new AtualizarItemEstoqueDTO
            {
                Nome = "Filtro de Óleo Premium",
                Quantidade = 75,
                TipoItemEstoque = TipoItemEstoqueEnum.Insumo
            };

            _repoMock.Setup(r => r.ObterPorIdAsync(id))
                .ReturnsAsync((ItemEstoque?)null);

            // Act
            var act = async () => await _service.AtualizarItemEstoque(id, dto);

            // Assert
            await act.Should().ThrowAsync<DomainException>()
                .WithMessage("Item de estoque não encontrado.");

            _repoMock.Verify(r => r.AtualizarAsync(It.IsAny<ItemEstoque>()), Times.Never);
        }

        #endregion

        #region AtualizarQuantidade Tests

        [Fact(DisplayName = "Deve atualizar quantidade do item de estoque se existir")]
        [Trait("Metodo", "AtualizarQuantidade")]
        public async Task AtualizarQuantidade_DeveAtualizarQuantidade_SeItemExistir()
        {
            // Arrange
            var id = Guid.NewGuid();
            var dto = new AtualizarQuantidadeDTO { Quantidade = 100 };

            var itemExistente = ItemEstoque.Criar("Filtro de Óleo", 50, TipoItemEstoqueEnum.Peca, 25.50m);
            var itemAtualizado = ItemEstoque.Criar("Filtro de Óleo", dto.Quantidade, TipoItemEstoqueEnum.Peca, 25.50m);

            _repoMock.Setup(r => r.ObterPorIdAsync(id))
                .ReturnsAsync(itemExistente);

            _repoMock.Setup(r => r.AtualizarAsync(It.IsAny<ItemEstoque>()))
                .ReturnsAsync(itemAtualizado);

            // Act
            var resultado = await _service.AtualizarQuantidade(id, dto);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Quantidade.Should().Be(dto.Quantidade);

            _repoMock.Verify(r => r.AtualizarAsync(It.IsAny<ItemEstoque>()), Times.Once);
        }

        [Fact(DisplayName = "Não deve atualizar quantidade se item não existir")]
        [Trait("Metodo", "AtualizarQuantidade")]
        public async Task AtualizarQuantidade_DeveLancarExcecao_SeItemNaoExistir()
        {
            // Arrange
            var id = Guid.NewGuid();
            var dto = new AtualizarQuantidadeDTO { Quantidade = 100 };

            _repoMock.Setup(r => r.ObterPorIdAsync(id))
                .ReturnsAsync((ItemEstoque?)null);

            // Act
            var act = async () => await _service.AtualizarQuantidade(id, dto);

            // Assert
            await act.Should().ThrowAsync<DomainException>()
                .WithMessage("Item de estoque não encontrado.");

            _repoMock.Verify(r => r.AtualizarAsync(It.IsAny<ItemEstoque>()), Times.Never);
        }

        #endregion

        #region Buscar Tests

        [Fact(DisplayName = "Deve buscar todos os itens de estoque")]
        [Trait("Metodo", "Buscar")]
        public async Task Buscar_DeveRetornarTodosOsItensEstoque()
        {
            // Arrange
            var itens = new List<ItemEstoque>
            {
                ItemEstoque.Criar("Filtro de Óleo", 50, TipoItemEstoqueEnum.Peca, 25.50m),
                ItemEstoque.Criar("Óleo Motor", 30, TipoItemEstoqueEnum.Insumo, 35.75m)
            };

            _repoMock.Setup(r => r.ObterTodosAsync())
                .ReturnsAsync(itens);

            // Act
            var resultado = await _service.Buscar();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().HaveCount(2);

            var resultadoLista = resultado.ToList();
            resultadoLista[0].Nome.Should().Be("Filtro de Óleo");
            resultadoLista[1].Nome.Should().Be("Óleo Motor");

            _repoMock.Verify(r => r.ObterTodosAsync(), Times.Once);
        }

        [Fact(DisplayName = "Deve retornar lista vazia quando não há itens de estoque")]
        [Trait("Metodo", "Buscar")]
        public async Task Buscar_DeveRetornarListaVazia_QuandoNaoHaItens()
        {
            // Arrange
            _repoMock.Setup(r => r.ObterTodosAsync())
                .ReturnsAsync(new List<ItemEstoque>());

            // Act
            var resultado = await _service.Buscar();

            // Assert
            resultado.Should().NotBeNull();
            resultado.Should().BeEmpty();

            _repoMock.Verify(r => r.ObterTodosAsync(), Times.Once);
        }

        #endregion

        #region BuscarPorId Tests

        [Fact(DisplayName = "Deve buscar item de estoque por ID quando existir")]
        [Trait("Metodo", "BuscarPorId")]
        public async Task BuscarPorId_DeveRetornarItemEstoque_QuandoItemExistir()
        {
            // Arrange
            var id = Guid.NewGuid();
            var item = ItemEstoque.Criar("Filtro de Óleo", 50, TipoItemEstoqueEnum.Peca, 25.50m);

            _repoMock.Setup(r => r.ObterPorIdAsync(id))
                .ReturnsAsync(item);

            // Act
            var resultado = await _service.BuscarPorId(id);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Nome.Should().Be("Filtro de Óleo");
            resultado.Quantidade.Should().Be(50);

            _repoMock.Verify(r => r.ObterPorIdAsync(id), Times.Once);
        }

        [Fact(DisplayName = "Deve lançar exceção ao buscar item de estoque por ID quando não existir")]
        [Trait("Metodo", "BuscarPorId")]
        public async Task BuscarPorId_DeveLancarExcecao_QuandoItemNaoExistir()
        {
            // Arrange
            var id = Guid.NewGuid();

            _repoMock.Setup(r => r.ObterPorIdAsync(id))
                .ReturnsAsync((ItemEstoque?)null);

            // Act
            var act = async () => await _service.BuscarPorId(id);

            // Assert
            await act.Should().ThrowAsync<DomainException>()
                .WithMessage("Item de estoque não encontrado.");

            _repoMock.Verify(r => r.ObterPorIdAsync(id), Times.Once);
        }

        #endregion

        #region VerificarDisponibilidade Tests

        [Fact(DisplayName = "Deve verificar disponibilidade quando item existir e estiver disponível")]
        [Trait("Metodo", "VerificarDisponibilidade")]
        public async Task VerificarDisponibilidade_DeveRetornarDisponivel_QuandoItemExistirEEstiverDisponivel()
        {
            // Arrange
            var id = Guid.NewGuid();
            var quantidadeRequisitada = 30;
            var item = ItemEstoque.Criar("Filtro de Óleo", 50, TipoItemEstoqueEnum.Peca, 25.50m);

            _repoMock.Setup(r => r.ObterPorIdAsync(id))
                .ReturnsAsync(item);

            // Act
            var resultado = await _service.VerificarDisponibilidade(id, quantidadeRequisitada);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Disponivel.Should().BeTrue();
            resultado.QuantidadeEmEstoque.Should().Be(50);
            resultado.QuantidadeSolicitada.Should().Be(quantidadeRequisitada);

            _repoMock.Verify(r => r.ObterPorIdAsync(id), Times.Once);
        }

        [Fact(DisplayName = "Deve verificar disponibilidade quando item existir mas não estiver disponível")]
        [Trait("Metodo", "VerificarDisponibilidade")]
        public async Task VerificarDisponibilidade_DeveRetornarIndisponivel_QuandoItemExistirMasNaoEstiverDisponivel()
        {
            // Arrange
            var id = Guid.NewGuid();
            var quantidadeRequisitada = 51;
            var item = ItemEstoque.Criar("Filtro de Óleo", 50, TipoItemEstoqueEnum.Peca, 25.50m);

            _repoMock.Setup(r => r.ObterPorIdAsync(id))
                .ReturnsAsync(item);

            // Act
            var resultado = await _service.VerificarDisponibilidade(id, quantidadeRequisitada);

            // Assert
            resultado.Should().NotBeNull();
            resultado.Disponivel.Should().BeFalse();
            resultado.QuantidadeEmEstoque.Should().Be(50);
            resultado.QuantidadeSolicitada.Should().Be(quantidadeRequisitada);

            _repoMock.Verify(r => r.ObterPorIdAsync(id), Times.Once);
        }

        [Fact(DisplayName = "Deve lançar exceção ao verificar disponibilidade quando item não existir")]
        [Trait("Metodo", "VerificarDisponibilidade")]
        public async Task VerificarDisponibilidade_DeveLancarExcecao_QuandoItemNaoExistir()
        {
            // Arrange
            var id = Guid.NewGuid();
            var quantidadeRequisitada = 30;

            _repoMock.Setup(r => r.ObterPorIdAsync(id))
                .ReturnsAsync((ItemEstoque?)null);

            // Act
            var act = async () => await _service.VerificarDisponibilidade(id, quantidadeRequisitada);

            // Assert
            await act.Should().ThrowAsync<DomainException>()
                .WithMessage("Item de estoque não encontrado.");

            _repoMock.Verify(r => r.ObterPorIdAsync(id), Times.Once);
        }

        #endregion
    }
}
