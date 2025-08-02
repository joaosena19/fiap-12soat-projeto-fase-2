using Application.OrdemServico.Interfaces;
using Application.OrdemServico.Interfaces.External;
using Application.OrdemServico.Services;
using Application.OrdemServico.Dtos;
using Application.OrdemServico.Dtos.External;
using AutoMapper;
using Domain.OrdemServico.Enums;
using FluentAssertions;
using Moq;
using Shared.Exceptions;
using Application;
using OrdemServicoAggregate = Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico;

namespace Tests.Application.OrdemServico
{
    public class OrdemServicoServiceUnitTest
    {
        private readonly Mock<IOrdemServicoRepository> _ordemServicoRepositoryMock;
        private readonly Mock<IServicoExternalService> _servicoExternalServiceMock;
        private readonly Mock<IEstoqueExternalService> _estoqueExternalServiceMock;
        private readonly Mock<IVeiculoExternalService> _veiculoExternalServiceMock;
        private readonly Mock<IClienteExternalService> _clienteExternalServiceMock;
        private readonly IMapper _mapper;
        private readonly OrdemServicoService _service;

        public OrdemServicoServiceUnitTest()
        {
            _ordemServicoRepositoryMock = new Mock<IOrdemServicoRepository>();
            _servicoExternalServiceMock = new Mock<IServicoExternalService>();
            _estoqueExternalServiceMock = new Mock<IEstoqueExternalService>();
            _veiculoExternalServiceMock = new Mock<IVeiculoExternalService>();
            _clienteExternalServiceMock = new Mock<IClienteExternalService>();
            _mapper = AutoMapperConfig.CreateMapper();
            _service = new OrdemServicoService(
                _ordemServicoRepositoryMock.Object,
                _servicoExternalServiceMock.Object,
                _estoqueExternalServiceMock.Object,
                _veiculoExternalServiceMock.Object,
                _clienteExternalServiceMock.Object,
                _mapper);
        }

        #region Buscar Tests

        [Fact(DisplayName = "Deve buscar todas as ordens de serviço")]
        [Trait("Metodo", "Buscar")]
        public async Task Buscar_DeveRetornarTodasAsOrdensServico()
        {
            // Arrange
            var veiculoId1 = Guid.NewGuid();
            var veiculoId2 = Guid.NewGuid();
            var ordemServico1 = OrdemServicoAggregate.Criar(veiculoId1);
            var ordemServico2 = OrdemServicoAggregate.Criar(veiculoId2);
            var ordensServico = new List<OrdemServicoAggregate> { ordemServico1, ordemServico2 };

            _ordemServicoRepositoryMock.Setup(r => r.ObterTodosAsync())
                .ReturnsAsync(ordensServico);

            // Act
            var result = await _service.Buscar();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().Contain(os => os.VeiculoId == veiculoId1 && os.Status == StatusOrdemServicoEnum.Recebida.ToString());
            result.Should().Contain(os => os.VeiculoId == veiculoId2 && os.Status == StatusOrdemServicoEnum.Recebida.ToString());
            _ordemServicoRepositoryMock.Verify(r => r.ObterTodosAsync(), Times.Once);
        }

        [Fact(DisplayName = "Deve retornar lista vazia quando não há ordens de serviço")]
        [Trait("Metodo", "Buscar")]
        public async Task Buscar_DeveRetornarListaVazia_QuandoNaoHaOrdensServico()
        {
            // Arrange
            _ordemServicoRepositoryMock.Setup(r => r.ObterTodosAsync())
                .ReturnsAsync(new List<OrdemServicoAggregate>());

            // Act
            var result = await _service.Buscar();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            _ordemServicoRepositoryMock.Verify(r => r.ObterTodosAsync(), Times.Once);
        }

        #endregion

        #region BuscarPorId Tests

        [Fact(DisplayName = "Deve buscar ordem de serviço por ID quando existir")]
        [Trait("Metodo", "BuscarPorId")]
        public async Task BuscarPorId_DeveRetornarOrdemServico_QuandoOrdemServicoExistir()
        {
            // Arrange
            var id = Guid.NewGuid();
            var veiculoId = Guid.NewGuid();
            var ordemServico = OrdemServicoAggregate.Criar(veiculoId);

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorIdAsync(id))
                .ReturnsAsync(ordemServico);

            // Act
            var result = await _service.BuscarPorId(id);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(ordemServico.Id);
            result.VeiculoId.Should().Be(veiculoId);
            result.Codigo.Should().Be(ordemServico.Codigo.Valor);
            result.Status.Should().Be(StatusOrdemServicoEnum.Recebida.ToString());
            _ordemServicoRepositoryMock.Verify(r => r.ObterPorIdAsync(id), Times.Once);
        }

        [Fact(DisplayName = "Deve lançar exceção ao buscar ordem de serviço por ID quando não existir")]
        [Trait("Metodo", "BuscarPorId")]
        public async Task BuscarPorId_DeveLancarExcecao_QuandoOrdemServicoNaoExistir()
        {
            // Arrange
            var id = Guid.NewGuid();

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorIdAsync(id))
                .ReturnsAsync((OrdemServicoAggregate?)null);

            // Act
            var act = async () => await _service.BuscarPorId(id);

            // Assert
            await act.Should().ThrowAsync<DomainException>()
                .WithMessage("Ordem de serviço não encontrada.");
            _ordemServicoRepositoryMock.Verify(r => r.ObterPorIdAsync(id), Times.Once);
        }

        #endregion

        #region BuscarPorCodigo Tests

        [Fact(DisplayName = "Deve buscar ordem de serviço por código quando existir")]
        [Trait("Metodo", "BuscarPorCodigo")]
        public async Task BuscarPorCodigo_DeveRetornarOrdemServico_QuandoOrdemServicoExistir()
        {
            // Arrange
            var veiculoId = Guid.NewGuid();
            var ordemServico = OrdemServicoAggregate.Criar(veiculoId);
            var codigo = ordemServico.Codigo.Valor;

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorCodigoAsync(codigo))
                .ReturnsAsync(ordemServico);

            // Act
            var result = await _service.BuscarPorCodigo(codigo);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(ordemServico.Id);
            result.VeiculoId.Should().Be(veiculoId);
            result.Codigo.Should().Be(codigo);
            result.Status.Should().Be(StatusOrdemServicoEnum.Recebida.ToString());
            _ordemServicoRepositoryMock.Verify(r => r.ObterPorCodigoAsync(codigo), Times.Once);
        }

        [Fact(DisplayName = "Deve lançar exceção ao buscar ordem de serviço por código quando não existir")]
        [Trait("Metodo", "BuscarPorCodigo")]
        public async Task BuscarPorCodigo_DeveLancarExcecao_QuandoOrdemServicoNaoExistir()
        {
            // Arrange
            var codigo = "OS-20250125-TESTE";

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorCodigoAsync(codigo))
                .ReturnsAsync((OrdemServicoAggregate?)null);

            // Act
            var act = async () => await _service.BuscarPorCodigo(codigo);

            // Assert
            await act.Should().ThrowAsync<DomainException>()
                .WithMessage("Ordem de serviço não encontrada.");
            _ordemServicoRepositoryMock.Verify(r => r.ObterPorCodigoAsync(codigo), Times.Once);
        }

        [Fact(DisplayName = "Deve buscar ordem de serviço por código independente do case")]
        [Trait("Metodo", "BuscarPorCodigo")]
        public async Task BuscarPorCodigo_DeveRetornarOrdemServico_IndependenteDoCaseDoCodigo()
        {
            // Arrange
            var veiculoId = Guid.NewGuid();
            var ordemServico = OrdemServicoAggregate.Criar(veiculoId);
            var codigoOriginal = ordemServico.Codigo.Valor;
            var codigoLowerCase = codigoOriginal.ToLowerInvariant();

            // Configura o repository para retornar a OS mesmo passando codigo lowercase
            _ordemServicoRepositoryMock.Setup(r => r.ObterPorCodigoAsync(codigoLowerCase))
                .ReturnsAsync(ordemServico);

            // Act
            var result = await _service.BuscarPorCodigo(codigoLowerCase);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(ordemServico.Id);
            result.VeiculoId.Should().Be(veiculoId);
            result.Codigo.Should().Be(codigoOriginal); // Deve retornar com codigo original
            result.Status.Should().Be(StatusOrdemServicoEnum.Recebida.ToString());
            _ordemServicoRepositoryMock.Verify(r => r.ObterPorCodigoAsync(codigoLowerCase), Times.Once);
        }

        #endregion

        #region CriarOrdemServico Tests

        [Fact(DisplayName = "Deve criar ordem de serviço com veículo válido")]
        [Trait("Metodo", "CriarOrdemServico")]
        public async Task CriarOrdemServico_ComVeiculoValido_DeveRetornarOrdemServicoCriada()
        {
            // Arrange
            var veiculoId = Guid.NewGuid();
            var dto = new CriarOrdemServicoDto { VeiculoId = veiculoId };
            var novaOrdemServico = OrdemServicoAggregate.Criar(veiculoId);

            _veiculoExternalServiceMock.Setup(v => v.VerificarExistenciaVeiculo(veiculoId))
                .ReturnsAsync(true);

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorCodigoAsync(It.IsAny<string>()))
                .ReturnsAsync((OrdemServicoAggregate?)null);

            _ordemServicoRepositoryMock.Setup(r => r.SalvarAsync(It.IsAny<OrdemServicoAggregate>()))
                .ReturnsAsync(novaOrdemServico);

            // Act
            var result = await _service.CriarOrdemServico(dto);

            // Assert
            result.Should().NotBeNull();
            result.VeiculoId.Should().Be(veiculoId);
            result.Status.Should().Be(StatusOrdemServicoEnum.Recebida.ToString());
            result.Codigo.Should().NotBeNullOrEmpty();

            _veiculoExternalServiceMock.Verify(v => v.VerificarExistenciaVeiculo(veiculoId), Times.Once);
            _ordemServicoRepositoryMock.Verify(r => r.SalvarAsync(It.IsAny<OrdemServicoAggregate>()), Times.Once);
        }

        [Fact(DisplayName = "Não deve criar ordem de serviço se veículo não existir")]
        [Trait("Metodo", "CriarOrdemServico")]
        public async Task CriarOrdemServico_ComVeiculoInexistente_DeveLancarExcecao()
        {
            // Arrange
            var veiculoId = Guid.NewGuid();
            var dto = new CriarOrdemServicoDto { VeiculoId = veiculoId };

            _veiculoExternalServiceMock.Setup(v => v.VerificarExistenciaVeiculo(veiculoId))
                .ReturnsAsync(false);

            // Act
            var act = async () => await _service.CriarOrdemServico(dto);

            // Assert
            await act.Should().ThrowAsync<DomainException>()
                .WithMessage("Veículo não encontrado para criar a ordem de serviço.");

            _veiculoExternalServiceMock.Verify(v => v.VerificarExistenciaVeiculo(veiculoId), Times.Once);
            _ordemServicoRepositoryMock.Verify(r => r.SalvarAsync(It.IsAny<OrdemServicoAggregate>()), Times.Never);
        }

        [Fact(DisplayName = "Deve gerar novo código se código já existir")]
        [Trait("Metodo", "CriarOrdemServico")]
        public async Task CriarOrdemServico_ComCodigoExistente_DeveGerarNovoCodigo()
        {
            // Arrange
            var veiculoId = Guid.NewGuid();
            var dto = new CriarOrdemServicoDto { VeiculoId = veiculoId };
            var ordemServicoExistente = OrdemServicoAggregate.Criar(veiculoId);
            var novaOrdemServico = OrdemServicoAggregate.Criar(veiculoId);

            _veiculoExternalServiceMock.Setup(v => v.VerificarExistenciaVeiculo(veiculoId))
                .ReturnsAsync(true);

            // Primeira chamada retorna a OS existente, segunda chamada retorna null, assim vai forçar fazer um loop no do while. Os códigos vão ser diferentes, mas o ponto é testar que se o repository retorna OS existente, o código deve criar OS de novo
            _ordemServicoRepositoryMock.SetupSequence(r => r.ObterPorCodigoAsync(It.IsAny<string>()))
                .ReturnsAsync(ordemServicoExistente)
                .ReturnsAsync((OrdemServicoAggregate?)null);

            _ordemServicoRepositoryMock.Setup(r => r.SalvarAsync(It.IsAny<OrdemServicoAggregate>()))
                .ReturnsAsync(novaOrdemServico);

            // Act
            var result = await _service.CriarOrdemServico(dto);

            // Assert
            result.Should().NotBeNull();
            result.VeiculoId.Should().Be(veiculoId);

            _veiculoExternalServiceMock.Verify(v => v.VerificarExistenciaVeiculo(veiculoId), Times.Once);
            _ordemServicoRepositoryMock.Verify(r => r.ObterPorCodigoAsync(It.IsAny<string>()), Times.AtLeast(2));
            _ordemServicoRepositoryMock.Verify(r => r.SalvarAsync(It.IsAny<OrdemServicoAggregate>()), Times.Once);
        }

        #endregion

        #region AdicionarServicos Tests

        [Fact(DisplayName = "Deve adicionar serviços válidos à ordem de serviço")]
        [Trait("Metodo", "AdicionarServicos")]
        public async Task AdicionarServicos_ComServicosValidos_DeveAdicionarServicos()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var veiculoId = Guid.NewGuid();
            var servicoId1 = Guid.NewGuid();
            var servicoId2 = Guid.NewGuid();
            
            var dto = new AdicionarServicosDto
            {
                ServicosOriginaisIds = new List<Guid> { servicoId1, servicoId2 }
            };

            var ordemServico = OrdemServicoAggregate.Criar(veiculoId);
            var servico1 = new ServicoExternalDto
            {
                Id = servicoId1,
                Nome = "Troca de Óleo",
                Preco = 150.00m
            };
            var servico2 = new ServicoExternalDto
            {
                Id = servicoId2,
                Nome = "Alinhamento",
                Preco = 80.00m
            };

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorIdAsync(ordemServicoId))
                .ReturnsAsync(ordemServico);

            _servicoExternalServiceMock.Setup(s => s.ObterServicoPorIdAsync(servicoId1))
                .ReturnsAsync(servico1);

            _servicoExternalServiceMock.Setup(s => s.ObterServicoPorIdAsync(servicoId2))
                .ReturnsAsync(servico2);

            _ordemServicoRepositoryMock.Setup(r => r.AtualizarAsync(It.IsAny<OrdemServicoAggregate>()))
                .ReturnsAsync(ordemServico);

            // Act
            var result = await _service.AdicionarServicos(ordemServicoId, dto);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(ordemServico.Id);

            _ordemServicoRepositoryMock.Verify(r => r.ObterPorIdAsync(ordemServicoId), Times.Once);
            _servicoExternalServiceMock.Verify(s => s.ObterServicoPorIdAsync(servicoId1), Times.Once);
            _servicoExternalServiceMock.Verify(s => s.ObterServicoPorIdAsync(servicoId2), Times.Once);
            _ordemServicoRepositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<OrdemServicoAggregate>()), Times.Once);
        }

        [Fact(DisplayName = "Não deve adicionar serviços se ordem de serviço não existir")]
        [Trait("Metodo", "AdicionarServicos")]
        public async Task AdicionarServicos_ComOrdemServicoInexistente_DeveLancarExcecao()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var servicoId = Guid.NewGuid();
            var dto = new AdicionarServicosDto
            {
                ServicosOriginaisIds = new List<Guid> { servicoId }
            };

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorIdAsync(ordemServicoId))
                .ReturnsAsync((OrdemServicoAggregate?)null);

            // Act
            var act = async () => await _service.AdicionarServicos(ordemServicoId, dto);

            // Assert
            await act.Should().ThrowAsync<DomainException>()
                .WithMessage("Ordem de serviço não encontrada.");

            _ordemServicoRepositoryMock.Verify(r => r.ObterPorIdAsync(ordemServicoId), Times.Once);
            _servicoExternalServiceMock.Verify(s => s.ObterServicoPorIdAsync(It.IsAny<Guid>()), Times.Never);
            _ordemServicoRepositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<OrdemServicoAggregate>()), Times.Never);
        }

        [Fact(DisplayName = "Não deve adicionar serviços se serviço não existir")]
        [Trait("Metodo", "AdicionarServicos")]
        public async Task AdicionarServicos_ComServicoInexistente_DeveLancarExcecao()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var veiculoId = Guid.NewGuid();
            var servicoId = Guid.NewGuid();
            var dto = new AdicionarServicosDto
            {
                ServicosOriginaisIds = new List<Guid> { servicoId }
            };

            var ordemServico = OrdemServicoAggregate.Criar(veiculoId);

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorIdAsync(ordemServicoId))
                .ReturnsAsync(ordemServico);

            _servicoExternalServiceMock.Setup(s => s.ObterServicoPorIdAsync(servicoId))
                .ReturnsAsync((ServicoExternalDto?)null);

            // Act
            var act = async () => await _service.AdicionarServicos(ordemServicoId, dto);

            // Assert
            await act.Should().ThrowAsync<DomainException>()
                .WithMessage($"Serviço com ID {servicoId} não encontrado.");

            _ordemServicoRepositoryMock.Verify(r => r.ObterPorIdAsync(ordemServicoId), Times.Once);
            _servicoExternalServiceMock.Verify(s => s.ObterServicoPorIdAsync(servicoId), Times.Once);
            _ordemServicoRepositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<OrdemServicoAggregate>()), Times.Never);
        }

        [Fact(DisplayName = "Não deve adicionar serviços se lista estiver nula")]
        [Trait("Metodo", "AdicionarServicos")]
        public async Task AdicionarServicos_ComListaNula_DeveLancarExcecao()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var veiculoId = Guid.NewGuid();
            var dto = new AdicionarServicosDto
            {
                ServicosOriginaisIds = null!
            };

            var ordemServico = OrdemServicoAggregate.Criar(veiculoId);

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorIdAsync(ordemServicoId))
                .ReturnsAsync(ordemServico);

            // Act
            var act = async () => await _service.AdicionarServicos(ordemServicoId, dto);

            // Assert
            await act.Should().ThrowAsync<DomainException>()
                .WithMessage("É necessário informar ao menos um serviço para adiciona na Ordem de Serviço");

            _ordemServicoRepositoryMock.Verify(r => r.ObterPorIdAsync(ordemServicoId), Times.Never);
            _servicoExternalServiceMock.Verify(s => s.ObterServicoPorIdAsync(It.IsAny<Guid>()), Times.Never);
            _ordemServicoRepositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<OrdemServicoAggregate>()), Times.Never);
        }

        #endregion

        #region AdicionarItem Tests

        [Fact(DisplayName = "Deve adicionar item válido à ordem de serviço")]
        [Trait("Metodo", "AdicionarItem")]
        public async Task AdicionarItem_ComItemValido_DeveAdicionarItem()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var veiculoId = Guid.NewGuid();
            var itemId = Guid.NewGuid();
            var dto = new AdicionarItemDto
            {
                ItemEstoqueOriginalId = itemId,
                Quantidade = 2
            };

            var ordemServico = OrdemServicoAggregate.Criar(veiculoId);
            var itemEstoque = new ItemEstoqueExternalDto
            {
                Id = itemId,
                Nome = "Filtro de Óleo",
                Preco = 25.50m,
                Quantidade = 10,
                TipoItemIncluido = TipoItemIncluidoEnum.Peca
            };

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorIdAsync(ordemServicoId))
                .ReturnsAsync(ordemServico);

            _estoqueExternalServiceMock.Setup(e => e.ObterItemEstoquePorIdAsync(itemId))
                .ReturnsAsync(itemEstoque);

            _ordemServicoRepositoryMock.Setup(r => r.AtualizarAsync(It.IsAny<OrdemServicoAggregate>()))
                .ReturnsAsync(ordemServico);

            // Act
            var result = await _service.AdicionarItem(ordemServicoId, dto);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(ordemServico.Id);

            _ordemServicoRepositoryMock.Verify(r => r.ObterPorIdAsync(ordemServicoId), Times.Once);
            _estoqueExternalServiceMock.Verify(e => e.ObterItemEstoquePorIdAsync(itemId), Times.Once);
            _ordemServicoRepositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<OrdemServicoAggregate>()), Times.Once);
        }

        [Fact(DisplayName = "Não deve adicionar item se ordem de serviço não existir")]
        [Trait("Metodo", "AdicionarItem")]
        public async Task AdicionarItem_ComOrdemServicoInexistente_DeveLancarExcecao()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var itemId = Guid.NewGuid();
            var dto = new AdicionarItemDto
            {
                ItemEstoqueOriginalId = itemId,
                Quantidade = 2
            };

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorIdAsync(ordemServicoId))
                .ReturnsAsync((OrdemServicoAggregate?)null);

            // Act
            var act = async () => await _service.AdicionarItem(ordemServicoId, dto);

            // Assert
            await act.Should().ThrowAsync<DomainException>()
                .WithMessage("Ordem de serviço não encontrada.");

            _ordemServicoRepositoryMock.Verify(r => r.ObterPorIdAsync(ordemServicoId), Times.Once);
            _estoqueExternalServiceMock.Verify(e => e.ObterItemEstoquePorIdAsync(It.IsAny<Guid>()), Times.Never);
            _ordemServicoRepositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<OrdemServicoAggregate>()), Times.Never);
        }

        [Fact(DisplayName = "Não deve adicionar item se item de estoque não existir")]
        [Trait("Metodo", "AdicionarItem")]
        public async Task AdicionarItem_ComItemEstoqueInexistente_DeveLancarExcecao()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var veiculoId = Guid.NewGuid();
            var itemId = Guid.NewGuid();
            var dto = new AdicionarItemDto
            {
                ItemEstoqueOriginalId = itemId,
                Quantidade = 2
            };

            var ordemServico = OrdemServicoAggregate.Criar(veiculoId);

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorIdAsync(ordemServicoId))
                .ReturnsAsync(ordemServico);

            _estoqueExternalServiceMock.Setup(e => e.ObterItemEstoquePorIdAsync(itemId))
                .ReturnsAsync((ItemEstoqueExternalDto?)null);

            // Act
            var act = async () => await _service.AdicionarItem(ordemServicoId, dto);

            // Assert
            await act.Should().ThrowAsync<DomainException>()
                .WithMessage($"Item de estoque com ID {itemId} não encontrado.");

            _ordemServicoRepositoryMock.Verify(r => r.ObterPorIdAsync(ordemServicoId), Times.Once);
            _estoqueExternalServiceMock.Verify(e => e.ObterItemEstoquePorIdAsync(itemId), Times.Once);
            _ordemServicoRepositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<OrdemServicoAggregate>()), Times.Never);
        }

        [Fact(DisplayName = "Deve adicionar item com quantidade correta")]
        [Trait("Metodo", "AdicionarItem")]
        public async Task AdicionarItem_ComQuantidadeEspecifica_DeveAdicionarComQuantidadeCorreta()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var veiculoId = Guid.NewGuid();
            var itemId = Guid.NewGuid();
            var quantidadeEsperada = 5;
            var dto = new AdicionarItemDto
            {
                ItemEstoqueOriginalId = itemId,
                Quantidade = quantidadeEsperada
            };

            var ordemServico = OrdemServicoAggregate.Criar(veiculoId);
            var itemEstoque = new ItemEstoqueExternalDto
            {
                Id = itemId,
                Nome = "Parafuso",
                Preco = 1.50m,
                Quantidade = 100,
                TipoItemIncluido = TipoItemIncluidoEnum.Peca
            };

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorIdAsync(ordemServicoId))
                .ReturnsAsync(ordemServico);

            _estoqueExternalServiceMock.Setup(e => e.ObterItemEstoquePorIdAsync(itemId))
                .ReturnsAsync(itemEstoque);

            _ordemServicoRepositoryMock.Setup(r => r.AtualizarAsync(It.IsAny<OrdemServicoAggregate>()))
                .ReturnsAsync(ordemServico);

            // Act
            var result = await _service.AdicionarItem(ordemServicoId, dto);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(ordemServico.Id);
            result.ItensIncluidos.First().Quantidade.Should().Be(quantidadeEsperada);

            _ordemServicoRepositoryMock.Verify(r => r.ObterPorIdAsync(ordemServicoId), Times.Once);
            _estoqueExternalServiceMock.Verify(e => e.ObterItemEstoquePorIdAsync(itemId), Times.Once);
            _ordemServicoRepositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<OrdemServicoAggregate>()), Times.Once);
        }

        #endregion

        #region RemoverServico Tests

        [Fact(DisplayName = "Deve remover serviço válido da ordem de serviço")]
        [Trait("Metodo", "RemoverServico")]
        public async Task RemoverServico_ComServicoValido_DeveRemoverServico()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var veiculoId = Guid.NewGuid();
            var servicoIncluidoId = Guid.NewGuid();

            var ordemServico = OrdemServicoAggregate.Criar(veiculoId);
            // First add a service to remove it later
            ordemServico.AdicionarServico(Guid.NewGuid(), "Troca de Óleo", 150.00m);
            var servicoIncluido = ordemServico.ServicosIncluidos.First();

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorIdAsync(ordemServicoId))
                .ReturnsAsync(ordemServico);

            _ordemServicoRepositoryMock.Setup(r => r.AtualizarAsync(It.IsAny<OrdemServicoAggregate>()))
                .ReturnsAsync(ordemServico);

            // Act
            await _service.RemoverServico(ordemServicoId, servicoIncluido.Id);

            // Assert
            ordemServico.ServicosIncluidos.Should().BeEmpty();

            _ordemServicoRepositoryMock.Verify(r => r.ObterPorIdAsync(ordemServicoId), Times.Once);
            _ordemServicoRepositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<OrdemServicoAggregate>()), Times.Once);
        }

        [Fact(DisplayName = "Não deve remover serviço se ordem de serviço não existir")]
        [Trait("Metodo", "RemoverServico")]
        public async Task RemoverServico_ComOrdemServicoInexistente_DeveLancarExcecao()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var servicoIncluidoId = Guid.NewGuid();

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorIdAsync(ordemServicoId))
                .ReturnsAsync((OrdemServicoAggregate?)null);

            // Act
            var act = async () => await _service.RemoverServico(ordemServicoId, servicoIncluidoId);

            // Assert
            await act.Should().ThrowAsync<DomainException>()
                .WithMessage("Ordem de serviço não encontrada.");

            _ordemServicoRepositoryMock.Verify(r => r.ObterPorIdAsync(ordemServicoId), Times.Once);
            _ordemServicoRepositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<OrdemServicoAggregate>()), Times.Never);
        }

        #endregion

        #region RemoverItem Tests

        [Fact(DisplayName = "Deve remover item válido da ordem de serviço")]
        [Trait("Metodo", "RemoverItem")]
        public async Task RemoverItem_ComItemValido_DeveRemoverItem()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var veiculoId = Guid.NewGuid();

            var ordemServico = OrdemServicoAggregate.Criar(veiculoId);
            // First add an item to remove it later
            ordemServico.AdicionarItem(Guid.NewGuid(), "Filtro de Óleo", 25.50m, 2, TipoItemIncluidoEnum.Peca);
            var itemIncluido = ordemServico.ItensIncluidos.First();

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorIdAsync(ordemServicoId))
                .ReturnsAsync(ordemServico);

            _ordemServicoRepositoryMock.Setup(r => r.AtualizarAsync(It.IsAny<OrdemServicoAggregate>()))
                .ReturnsAsync(ordemServico);

            // Act
            await _service.RemoverItem(ordemServicoId, itemIncluido.Id);

            // Assert
            ordemServico.ItensIncluidos.Should().BeEmpty();

            _ordemServicoRepositoryMock.Verify(r => r.ObterPorIdAsync(ordemServicoId), Times.Once);
            _ordemServicoRepositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<OrdemServicoAggregate>()), Times.Once);
        }

        [Fact(DisplayName = "Não deve remover item se ordem de serviço não existir")]
        [Trait("Metodo", "RemoverItem")]
        public async Task RemoverItem_ComOrdemServicoInexistente_DeveLancarExcecao()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var itemIncluidoId = Guid.NewGuid();

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorIdAsync(ordemServicoId))
                .ReturnsAsync((OrdemServicoAggregate?)null);

            // Act
            var act = async () => await _service.RemoverItem(ordemServicoId, itemIncluidoId);

            // Assert
            await act.Should().ThrowAsync<DomainException>()
                .WithMessage("Ordem de serviço não encontrada.");

            _ordemServicoRepositoryMock.Verify(r => r.ObterPorIdAsync(ordemServicoId), Times.Once);
            _ordemServicoRepositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<OrdemServicoAggregate>()), Times.Never);
        }

        #endregion

        #region Cancelar Tests

        [Fact(DisplayName = "Deve cancelar ordem de serviço válida")]
        [Trait("Metodo", "Cancelar")]
        public async Task Cancelar_ComOrdemServicoValida_DeveCancelarOrdemServico()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var veiculoId = Guid.NewGuid();

            var ordemServico = OrdemServicoAggregate.Criar(veiculoId);

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorIdAsync(ordemServicoId))
                .ReturnsAsync(ordemServico);

            _ordemServicoRepositoryMock.Setup(r => r.AtualizarAsync(It.IsAny<OrdemServicoAggregate>()))
                .ReturnsAsync(ordemServico);

            // Act
            await _service.Cancelar(ordemServicoId);

            // Assert
            ordemServico.Status.Valor.Should().Be(StatusOrdemServicoEnum.Cancelada);

            _ordemServicoRepositoryMock.Verify(r => r.ObterPorIdAsync(ordemServicoId), Times.Once);
            _ordemServicoRepositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<OrdemServicoAggregate>()), Times.Once);
        }

        [Fact(DisplayName = "Não deve cancelar se ordem de serviço não existir")]
        [Trait("Metodo", "Cancelar")]
        public async Task Cancelar_ComOrdemServicoInexistente_DeveLancarExcecao()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorIdAsync(ordemServicoId))
                .ReturnsAsync((OrdemServicoAggregate?)null);

            // Act
            var act = async () => await _service.Cancelar(ordemServicoId);

            // Assert
            await act.Should().ThrowAsync<DomainException>()
                .WithMessage("Ordem de serviço não encontrada.");

            _ordemServicoRepositoryMock.Verify(r => r.ObterPorIdAsync(ordemServicoId), Times.Once);
            _ordemServicoRepositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<OrdemServicoAggregate>()), Times.Never);
        }

        #endregion

        #region IniciarDiagnostico Tests

        [Fact(DisplayName = "Deve iniciar diagnóstico para ordem com status Recebida")]
        [Trait("Metodo", "IniciarDiagnostico")]
        public async Task IniciarDiagnostico_ComStatusRecebida_DeveIniciarDiagnostico()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var veiculoId = Guid.NewGuid();

            var ordemServico = OrdemServicoAggregate.Criar(veiculoId);

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorIdAsync(ordemServicoId))
                .ReturnsAsync(ordemServico);

            _ordemServicoRepositoryMock.Setup(r => r.AtualizarAsync(It.IsAny<OrdemServicoAggregate>()))
                .ReturnsAsync(ordemServico);

            // Act
            await _service.IniciarDiagnostico(ordemServicoId);

            // Assert
            ordemServico.Status.Valor.Should().Be(StatusOrdemServicoEnum.EmDiagnostico);

            _ordemServicoRepositoryMock.Verify(r => r.ObterPorIdAsync(ordemServicoId), Times.Once);
            _ordemServicoRepositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<OrdemServicoAggregate>()), Times.Once);
        }

        [Fact(DisplayName = "Não deve iniciar diagnóstico se ordem de serviço não existir")]
        [Trait("Metodo", "IniciarDiagnostico")]
        public async Task IniciarDiagnostico_ComOrdemServicoInexistente_DeveLancarExcecao()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorIdAsync(ordemServicoId))
                .ReturnsAsync((OrdemServicoAggregate?)null);

            // Act
            var act = async () => await _service.IniciarDiagnostico(ordemServicoId);

            // Assert
            await act.Should().ThrowAsync<DomainException>()
                .WithMessage("Ordem de serviço não encontrada.");

            _ordemServicoRepositoryMock.Verify(r => r.ObterPorIdAsync(ordemServicoId), Times.Once);
            _ordemServicoRepositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<OrdemServicoAggregate>()), Times.Never);
        }

        #endregion

        #region GerarOrcamento Tests

        [Fact(DisplayName = "Deve gerar orçamento para ordem com serviços e status EmDiagnostico")]
        [Trait("Metodo", "GerarOrcamento")]
        public async Task GerarOrcamento_ComServicosEStatusValido_DeveGerarOrcamento()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var veiculoId = Guid.NewGuid();

            var ordemServico = OrdemServicoAggregate.Criar(veiculoId);
            ordemServico.IniciarDiagnostico(); // Alterar status para EmDiagnostico
            ordemServico.AdicionarServico(Guid.NewGuid(), "Troca de Óleo", 150.00m);

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorIdAsync(ordemServicoId))
                .ReturnsAsync(ordemServico);

            _ordemServicoRepositoryMock.Setup(r => r.AtualizarAsync(It.IsAny<OrdemServicoAggregate>()))
                .ReturnsAsync(ordemServico);

            // Act
            var result = await _service.GerarOrcamento(ordemServicoId);

            // Assert
            result.Should().NotBeNull();
            result.Preco.Should().BeGreaterThan(0);
            ordemServico.Status.Valor.Should().Be(StatusOrdemServicoEnum.AguardandoAprovacao);
            ordemServico.Orcamento.Should().NotBeNull();

            _ordemServicoRepositoryMock.Verify(r => r.ObterPorIdAsync(ordemServicoId), Times.Once);
            _ordemServicoRepositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<OrdemServicoAggregate>()), Times.Once);
        }

        [Fact(DisplayName = "Deve gerar orçamento para ordem com itens e status EmDiagnostico")]
        [Trait("Metodo", "GerarOrcamento")]
        public async Task GerarOrcamento_ComItensEStatusValido_DeveGerarOrcamento()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var veiculoId = Guid.NewGuid();

            var ordemServico = OrdemServicoAggregate.Criar(veiculoId);
            ordemServico.IniciarDiagnostico(); // Change status to EmDiagnostico
            ordemServico.AdicionarItem(Guid.NewGuid(), "Filtro de Óleo", 25.50m, 2, TipoItemIncluidoEnum.Peca);

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorIdAsync(ordemServicoId))
                .ReturnsAsync(ordemServico);

            _ordemServicoRepositoryMock.Setup(r => r.AtualizarAsync(It.IsAny<OrdemServicoAggregate>()))
                .ReturnsAsync(ordemServico);

            // Act
            var result = await _service.GerarOrcamento(ordemServicoId);

            // Assert
            result.Should().NotBeNull();
            result.Preco.Should().BeGreaterThan(0);
            ordemServico.Status.Valor.Should().Be(StatusOrdemServicoEnum.AguardandoAprovacao);
            ordemServico.Orcamento.Should().NotBeNull();

            _ordemServicoRepositoryMock.Verify(r => r.ObterPorIdAsync(ordemServicoId), Times.Once);
            _ordemServicoRepositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<OrdemServicoAggregate>()), Times.Once);
        }

        [Fact(DisplayName = "Não deve gerar orçamento se ordem de serviço não existir")]
        [Trait("Metodo", "GerarOrcamento")]
        public async Task GerarOrcamento_ComOrdemServicoInexistente_DeveLancarExcecao()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorIdAsync(ordemServicoId))
                .ReturnsAsync((OrdemServicoAggregate?)null);

            // Act
            var act = async () => await _service.GerarOrcamento(ordemServicoId);

            // Assert
            await act.Should().ThrowAsync<DomainException>()
                .WithMessage("Ordem de serviço não encontrada.");

            _ordemServicoRepositoryMock.Verify(r => r.ObterPorIdAsync(ordemServicoId), Times.Once);
            _ordemServicoRepositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<OrdemServicoAggregate>()), Times.Never);
        }

        #endregion

        #region AprovarOrcamento Tests

        [Fact(DisplayName = "Deve aprovar orçamento com itens disponíveis em estoque, e reduzir os itens do estoque")]
        [Trait("Metodo", "AprovarOrcamento")]
        public async Task AprovarOrcamento_ComItensDisponiveis_DeveAprovarEIniciarExecucaoEReduzirItensEstoque()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var veiculoId = Guid.NewGuid();
            var itemId = Guid.NewGuid();

            var ordemServico = OrdemServicoAggregate.Criar(veiculoId);
            ordemServico.IniciarDiagnostico();
            ordemServico.AdicionarItem(itemId, "Filtro de Óleo", 25.50m, 2, TipoItemIncluidoEnum.Peca);
            ordemServico.GerarOrcamento();

            var itemEstoque = new ItemEstoqueExternalDto
            {
                Id = itemId,
                Nome = "Filtro de Óleo",
                Preco = 25.50m,
                Quantidade = 10,
                TipoItemIncluido = TipoItemIncluidoEnum.Peca
            };

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorIdAsync(ordemServicoId))
                .ReturnsAsync(ordemServico);

            _estoqueExternalServiceMock.Setup(e => e.VerificarDisponibilidadeAsync(itemId, 2))
                .ReturnsAsync(true);

            _estoqueExternalServiceMock.Setup(e => e.ObterItemEstoquePorIdAsync(itemId))
                .ReturnsAsync(itemEstoque);

            _ordemServicoRepositoryMock.Setup(r => r.AtualizarAsync(It.IsAny<OrdemServicoAggregate>()))
                .ReturnsAsync(ordemServico);

            // Act
            await _service.AprovarOrcamento(ordemServicoId);

            // Assert
            ordemServico.Status.Valor.Should().Be(StatusOrdemServicoEnum.EmExecucao);

            _ordemServicoRepositoryMock.Verify(r => r.ObterPorIdAsync(ordemServicoId), Times.Once);
            _estoqueExternalServiceMock.Verify(e => e.VerificarDisponibilidadeAsync(itemId, 2), Times.Once);
            _estoqueExternalServiceMock.Verify(e => e.ObterItemEstoquePorIdAsync(itemId), Times.Once);
            _estoqueExternalServiceMock.Verify(e => e.AtualizarQuantidadeEstoqueAsync(itemId, 8), Times.Once);
            _ordemServicoRepositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<OrdemServicoAggregate>()), Times.Once);
        }

        [Fact(DisplayName = "Deve aprovar orçamento sem itens, só com serviços")]
        [Trait("Metodo", "AprovarOrcamento")]
        public async Task AprovarOrcamento_SemItens_DeveAprovarEIniciarExecucao()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var veiculoId = Guid.NewGuid();

            var ordemServico = OrdemServicoAggregate.Criar(veiculoId);
            ordemServico.IniciarDiagnostico();
            ordemServico.AdicionarServico(Guid.NewGuid(), "Troca de Óleo", 150.00m);
            ordemServico.GerarOrcamento();

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorIdAsync(ordemServicoId))
                .ReturnsAsync(ordemServico);

            _ordemServicoRepositoryMock.Setup(r => r.AtualizarAsync(It.IsAny<OrdemServicoAggregate>()))
                .ReturnsAsync(ordemServico);

            // Act
            await _service.AprovarOrcamento(ordemServicoId);

            // Assert
            ordemServico.Status.Valor.Should().Be(StatusOrdemServicoEnum.EmExecucao);

            _ordemServicoRepositoryMock.Verify(r => r.ObterPorIdAsync(ordemServicoId), Times.Once);
            _estoqueExternalServiceMock.Verify(e => e.VerificarDisponibilidadeAsync(It.IsAny<Guid>(), It.IsAny<int>()), Times.Never);
            _estoqueExternalServiceMock.Verify(e => e.AtualizarQuantidadeEstoqueAsync(It.IsAny<Guid>(), It.IsAny<int>()), Times.Never);
            _ordemServicoRepositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<OrdemServicoAggregate>()), Times.Once);
        }

        [Fact(DisplayName = "Não deve aprovar orçamento se ordem de serviço não existir")]
        [Trait("Metodo", "AprovarOrcamento")]
        public async Task AprovarOrcamento_ComOrdemServicoInexistente_DeveLancarExcecao()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorIdAsync(ordemServicoId))
                .ReturnsAsync((OrdemServicoAggregate?)null);

            // Act
            var act = async () => await _service.AprovarOrcamento(ordemServicoId);

            // Assert
            await act.Should().ThrowAsync<DomainException>()
                .WithMessage("Ordem de serviço não encontrada.");

            _ordemServicoRepositoryMock.Verify(r => r.ObterPorIdAsync(ordemServicoId), Times.Once);
            _estoqueExternalServiceMock.Verify(e => e.VerificarDisponibilidadeAsync(It.IsAny<Guid>(), It.IsAny<int>()), Times.Never);
            _ordemServicoRepositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<OrdemServicoAggregate>()), Times.Never);
        }

        [Fact(DisplayName = "Não deve aprovar orçamento se item não estiver disponível em estoque")]
        [Trait("Metodo", "AprovarOrcamento")]
        public async Task AprovarOrcamento_ComItemIndisponivel_DeveLancarExcecao()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var veiculoId = Guid.NewGuid();
            var itemId = Guid.NewGuid();

            var ordemServico = OrdemServicoAggregate.Criar(veiculoId);
            ordemServico.IniciarDiagnostico();
            ordemServico.AdicionarItem(itemId, "Filtro de Óleo", 25.50m, 5, TipoItemIncluidoEnum.Peca);
            ordemServico.GerarOrcamento();

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorIdAsync(ordemServicoId))
                .ReturnsAsync(ordemServico);

            _estoqueExternalServiceMock.Setup(e => e.VerificarDisponibilidadeAsync(itemId, 5))
                .ReturnsAsync(false);

            // Act
            var act = async () => await _service.AprovarOrcamento(ordemServicoId);

            // Assert
            await act.Should().ThrowAsync<DomainException>()
                .WithMessage("Item 'Filtro de Óleo' não está disponível no estoque na quantidade necessária (5).");

            _ordemServicoRepositoryMock.Verify(r => r.ObterPorIdAsync(ordemServicoId), Times.Once);
            _estoqueExternalServiceMock.Verify(e => e.VerificarDisponibilidadeAsync(itemId, 5), Times.Once);
            _estoqueExternalServiceMock.Verify(e => e.ObterItemEstoquePorIdAsync(It.IsAny<Guid>()), Times.Never);
            _ordemServicoRepositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<OrdemServicoAggregate>()), Times.Never);
        }

        #endregion

        #region DesaprovarOrcamento Tests

        [Fact(DisplayName = "Deve desaprovar orçamento válido")]
        [Trait("Metodo", "DesaprovarOrcamento")]
        public async Task DesaprovarOrcamento_ComOrdemServicoValida_DeveDesaprovarOrcamento()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var veiculoId = Guid.NewGuid();

            var ordemServico = OrdemServicoAggregate.Criar(veiculoId);
            ordemServico.IniciarDiagnostico();
            ordemServico.AdicionarServico(Guid.NewGuid(), "Troca de Óleo", 150.00m);
            ordemServico.GerarOrcamento();

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorIdAsync(ordemServicoId))
                .ReturnsAsync(ordemServico);

            _ordemServicoRepositoryMock.Setup(r => r.AtualizarAsync(It.IsAny<OrdemServicoAggregate>()))
                .ReturnsAsync(ordemServico);

            // Act
            await _service.DesaprovarOrcamento(ordemServicoId);

            // Assert
            ordemServico.Status.Valor.Should().Be(StatusOrdemServicoEnum.Cancelada);

            _ordemServicoRepositoryMock.Verify(r => r.ObterPorIdAsync(ordemServicoId), Times.Once);
            _ordemServicoRepositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<OrdemServicoAggregate>()), Times.Once);
        }

        [Fact(DisplayName = "Não deve desaprovar orçamento se ordem de serviço não existir")]
        [Trait("Metodo", "DesaprovarOrcamento")]
        public async Task DesaprovarOrcamento_ComOrdemServicoInexistente_DeveLancarExcecao()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorIdAsync(ordemServicoId))
                .ReturnsAsync((OrdemServicoAggregate?)null);

            // Act
            var act = async () => await _service.DesaprovarOrcamento(ordemServicoId);

            // Assert
            await act.Should().ThrowAsync<DomainException>()
                .WithMessage("Ordem de serviço não encontrada.");

            _ordemServicoRepositoryMock.Verify(r => r.ObterPorIdAsync(ordemServicoId), Times.Once);
            _ordemServicoRepositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<OrdemServicoAggregate>()), Times.Never);
        }

        #endregion

        #region FinalizarExecucao Tests

        [Fact(DisplayName = "Deve finalizar execução de ordem de serviço válida")]
        [Trait("Metodo", "FinalizarExecucao")]
        public async Task FinalizarExecucao_ComOrdemServicoValida_DeveFinalizarExecucao()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var veiculoId = Guid.NewGuid();

            var ordemServico = OrdemServicoAggregate.Criar(veiculoId);
            ordemServico.IniciarDiagnostico();
            ordemServico.AdicionarServico(Guid.NewGuid(), "Troca de Óleo", 150.00m);
            ordemServico.GerarOrcamento();
            ordemServico.AprovarOrcamento();

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorIdAsync(ordemServicoId))
                .ReturnsAsync(ordemServico);

            _ordemServicoRepositoryMock.Setup(r => r.AtualizarAsync(It.IsAny<OrdemServicoAggregate>()))
                .ReturnsAsync(ordemServico);

            // Act
            await _service.FinalizarExecucao(ordemServicoId);

            // Assert
            ordemServico.Status.Valor.Should().Be(StatusOrdemServicoEnum.Finalizada);

            _ordemServicoRepositoryMock.Verify(r => r.ObterPorIdAsync(ordemServicoId), Times.Once);
            _ordemServicoRepositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<OrdemServicoAggregate>()), Times.Once);
        }

        [Fact(DisplayName = "Não deve finalizar execução se ordem de serviço não existir")]
        [Trait("Metodo", "FinalizarExecucao")]
        public async Task FinalizarExecucao_ComOrdemServicoInexistente_DeveLancarExcecao()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorIdAsync(ordemServicoId))
                .ReturnsAsync((OrdemServicoAggregate?)null);

            // Act
            var act = async () => await _service.FinalizarExecucao(ordemServicoId);

            // Assert
            await act.Should().ThrowAsync<DomainException>()
                .WithMessage("Ordem de serviço não encontrada.");

            _ordemServicoRepositoryMock.Verify(r => r.ObterPorIdAsync(ordemServicoId), Times.Once);
            _ordemServicoRepositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<OrdemServicoAggregate>()), Times.Never);
        }

        #endregion

        #region Entregar Tests

        [Fact(DisplayName = "Deve entregar ordem de serviço finalizada")]
        [Trait("Metodo", "Entregar")]
        public async Task Entregar_ComOrdemServicoFinalizada_DeveEntregarOrdemServico()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();
            var veiculoId = Guid.NewGuid();

            var ordemServico = OrdemServicoAggregate.Criar(veiculoId);
            ordemServico.IniciarDiagnostico();
            ordemServico.AdicionarServico(Guid.NewGuid(), "Troca de Óleo", 150.00m);
            ordemServico.GerarOrcamento();
            ordemServico.AprovarOrcamento();
            ordemServico.FinalizarExecucao();

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorIdAsync(ordemServicoId))
                .ReturnsAsync(ordemServico);

            _ordemServicoRepositoryMock.Setup(r => r.AtualizarAsync(It.IsAny<OrdemServicoAggregate>()))
                .ReturnsAsync(ordemServico);

            // Act
            await _service.Entregar(ordemServicoId);

            // Assert
            ordemServico.Status.Valor.Should().Be(StatusOrdemServicoEnum.Entregue);

            _ordemServicoRepositoryMock.Verify(r => r.ObterPorIdAsync(ordemServicoId), Times.Once);
            _ordemServicoRepositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<OrdemServicoAggregate>()), Times.Once);
        }

        [Fact(DisplayName = "Não deve entregar se ordem de serviço não existir")]
        [Trait("Metodo", "Entregar")]
        public async Task Entregar_ComOrdemServicoInexistente_DeveLancarExcecao()
        {
            // Arrange
            var ordemServicoId = Guid.NewGuid();

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorIdAsync(ordemServicoId))
                .ReturnsAsync((OrdemServicoAggregate?)null);

            // Act
            var act = async () => await _service.Entregar(ordemServicoId);

            // Assert
            await act.Should().ThrowAsync<DomainException>()
                .WithMessage("Ordem de serviço não encontrada.");

            _ordemServicoRepositoryMock.Verify(r => r.ObterPorIdAsync(ordemServicoId), Times.Once);
            _ordemServicoRepositoryMock.Verify(r => r.AtualizarAsync(It.IsAny<OrdemServicoAggregate>()), Times.Never);
        }

        #endregion

        #region ObterTempoMedio Tests

        [Fact(DisplayName = "Deve obter tempo médio com ordens entregues válidas")]
        [Trait("Metodo", "ObterTempoMedio")]
        public async Task ObterTempoMedio_ComOrdensEntregues_DeveRetornarTempoMedio()
        {
            // Arrange
            var quantidadeDias = 30;
            var veiculoId = Guid.NewGuid();
            var ordemServico = OrdemServicoAggregate.Criar(veiculoId);
            
            // Simula o fluxo para definir as datas corretamente
            ordemServico.IniciarDiagnostico();
            ordemServico.AdicionarServico(Guid.NewGuid(), "Troca de Óleo", 150.00m);
            ordemServico.GerarOrcamento();
            ordemServico.AprovarOrcamento();
            ordemServico.FinalizarExecucao();
            ordemServico.Entregar();

            var ordensEntregues = new List<OrdemServicoAggregate> { ordemServico };

            _ordemServicoRepositoryMock.Setup(r => r.ObterEntreguesUltimosDiasAsync(quantidadeDias))
                .ReturnsAsync(ordensEntregues);

            // Act
            var result = await _service.ObterTempoMedio(quantidadeDias);

            // Assert
            result.Should().NotBeNull();
            result.QuantidadeDias.Should().Be(quantidadeDias);
            result.QuantidadeOrdensAnalisadas.Should().Be(1);
            // Como as datas são definidas automaticamente durante o fluxo, 
            // apenas verificamos que o cálculo foi feito (valores >= 0)
            result.TempoMedioCompletoHoras.Should().BeGreaterThanOrEqualTo(0);
            result.TempoMedioExecucaoHoras.Should().BeGreaterThanOrEqualTo(0);
            result.DataInicio.Should().BeCloseTo(DateTime.UtcNow.AddDays(-quantidadeDias), TimeSpan.FromMinutes(1));
            result.DataFim.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromMinutes(1));

            _ordemServicoRepositoryMock.Verify(r => r.ObterEntreguesUltimosDiasAsync(quantidadeDias), Times.Once);
        }

        [Fact(DisplayName = "Deve calcular tempo médio corretamente com múltiplas ordens (alterando manualmente os horários)")]
        [Trait("Metodo", "ObterTempoMedio")]
        public async Task ObterTempoMedio_ComMultiplasOrdens_DeveCalcularCorretamente()
        {
            // Arrange
            var quantidadeDias = 30;
            var veiculoId1 = Guid.NewGuid();
            var veiculoId2 = Guid.NewGuid();

            var ordemServico1 = OrdemServicoAggregate.Criar(veiculoId1);
            var ordemServico2 = OrdemServicoAggregate.Criar(veiculoId2);

            // Fluxo para as ordens
            foreach (var ordem in new[] { ordemServico1, ordemServico2 })
            {
                ordem.IniciarDiagnostico();
                ordem.AdicionarServico(Guid.NewGuid(), "Troca de Óleo", 150.00m);
                ordem.GerarOrcamento();
                ordem.AprovarOrcamento();
                ordem.FinalizarExecucao();
                ordem.Entregar();
            }

            // Simular datas realistas com reflection para ambas as ordens
            var dataCriacao1 = DateTime.UtcNow.AddDays(-5);
            var dataInicioExecucao1 = dataCriacao1.AddHours(24);
            var dataFinalizacao1 = dataInicioExecucao1.AddHours(6);
            var dataEntrega1 = dataFinalizacao1.AddHours(1);

            var dataCriacao2 = DateTime.UtcNow.AddDays(-3);
            var dataInicioExecucao2 = dataCriacao2.AddHours(12);
            var dataFinalizacao2 = dataInicioExecucao2.AddHours(10);
            var dataEntrega2 = dataFinalizacao2.AddHours(2);

            // Configurar ordem 1
            var historico1 = ordemServico1.Historico;
            var historicoType1 = historico1.GetType();

            var dataCriacaoField1 = historicoType1.GetField("_dataCriacao", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var dataInicioExecucaoField1 = historicoType1.GetField("_dataInicioExecucao", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var dataFinalizacaoField1 = historicoType1.GetField("_dataFinalizacao", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var dataEntregaField1 = historicoType1.GetField("_dataEntrega", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (dataCriacaoField1 != null) dataCriacaoField1.SetValue(historico1, dataCriacao1);
            if (dataInicioExecucaoField1 != null) dataInicioExecucaoField1.SetValue(historico1, dataInicioExecucao1);
            if (dataFinalizacaoField1 != null) dataFinalizacaoField1.SetValue(historico1, dataFinalizacao1);
            if (dataEntregaField1 != null) dataEntregaField1.SetValue(historico1, dataEntrega1);

            // Configurar ordem 2
            var historico2 = ordemServico2.Historico;
            var historicoType2 = historico2.GetType();

            var dataCriacaoField2 = historicoType2.GetField("_dataCriacao", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var dataInicioExecucaoField2 = historicoType2.GetField("_dataInicioExecucao", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var dataFinalizacaoField2 = historicoType2.GetField("_dataFinalizacao", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var dataEntregaField2 = historicoType2.GetField("_dataEntrega", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (dataCriacaoField2 != null) dataCriacaoField2.SetValue(historico2, dataCriacao2);
            if (dataInicioExecucaoField2 != null) dataInicioExecucaoField2.SetValue(historico2, dataInicioExecucao2);
            if (dataFinalizacaoField2 != null) dataFinalizacaoField2.SetValue(historico2, dataFinalizacao2);
            if (dataEntregaField2 != null) dataEntregaField2.SetValue(historico2, dataEntrega2);

            var ordensEntregues = new List<OrdemServicoAggregate> { ordemServico1, ordemServico2 };

            _ordemServicoRepositoryMock.Setup(r => r.ObterEntreguesUltimosDiasAsync(quantidadeDias))
                .ReturnsAsync(ordensEntregues);

            // Act
            var result = await _service.ObterTempoMedio(quantidadeDias);

            // Assert
            result.Should().NotBeNull();
            result.QuantidadeDias.Should().Be(quantidadeDias);
            result.QuantidadeOrdensAnalisadas.Should().Be(2);

            // Ordem 1: 31h completo (24+6+1), 6h execução
            // Ordem 2: 24h completo (12+10+2), 10h execução
            // Média completa: (31+24)/2 = 27.5h
            // Média execução: (6+10)/2 = 8h
            result.TempoMedioCompletoHoras.Should().Be(27.5);
            result.TempoMedioExecucaoHoras.Should().Be(8.0);

            _ordemServicoRepositoryMock.Verify(r => r.ObterEntreguesUltimosDiasAsync(quantidadeDias), Times.Once);
        }


        [Fact(DisplayName = "Deve usar valor padrão de 365 dias quando não especificado")]
        [Trait("Metodo", "ObterTempoMedio")]
        public async Task ObterTempoMedio_SemParametro_DeveUsarValorPadrao365Dias()
        {
            // Arrange
            var veiculoId = Guid.NewGuid();
            var ordemServico = OrdemServicoAggregate.Criar(veiculoId);
            
            ordemServico.IniciarDiagnostico();
            ordemServico.AdicionarServico(Guid.NewGuid(), "Troca de Óleo", 150.00m);
            ordemServico.GerarOrcamento();
            ordemServico.AprovarOrcamento();
            ordemServico.FinalizarExecucao();
            ordemServico.Entregar();

            var ordensEntregues = new List<OrdemServicoAggregate> { ordemServico };

            _ordemServicoRepositoryMock.Setup(r => r.ObterEntreguesUltimosDiasAsync(365))
                .ReturnsAsync(ordensEntregues);

            // Act
            var result = await _service.ObterTempoMedio();

            // Assert
            result.Should().NotBeNull();
            result.QuantidadeDias.Should().Be(365);

            _ordemServicoRepositoryMock.Verify(r => r.ObterEntreguesUltimosDiasAsync(365), Times.Once);
        }

        [Fact(DisplayName = "Não deve obter tempo médio com quantidade de dias menor que 1")]
        [Trait("Metodo", "ObterTempoMedio")]
        public async Task ObterTempoMedio_ComQuantidadeDiasMenorQue1_DeveLancarExcecao()
        {
            // Arrange
            var quantidadeDias = 0;

            // Act
            var act = async () => await _service.ObterTempoMedio(quantidadeDias);

            // Assert
            await act.Should().ThrowAsync<DomainException>()
                .WithMessage("A quantidade de dias deve estar entre 1 e 365.");

            _ordemServicoRepositoryMock.Verify(r => r.ObterEntreguesUltimosDiasAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact(DisplayName = "Não deve obter tempo médio com quantidade de dias maior que 365")]
        [Trait("Metodo", "ObterTempoMedio")]
        public async Task ObterTempoMedio_ComQuantidadeDiasMaiorQue365_DeveLancarExcecao()
        {
            // Arrange
            var quantidadeDias = 366;

            // Act
            var act = async () => await _service.ObterTempoMedio(quantidadeDias);

            // Assert
            await act.Should().ThrowAsync<DomainException>()
                .WithMessage("A quantidade de dias deve estar entre 1 e 365.");

            _ordemServicoRepositoryMock.Verify(r => r.ObterEntreguesUltimosDiasAsync(It.IsAny<int>()), Times.Never);
        }

        [Fact(DisplayName = "Não deve obter tempo médio quando não há ordens entregues")]
        [Trait("Metodo", "ObterTempoMedio")]
        public async Task ObterTempoMedio_SemOrdensEntregues_DeveLancarExcecao()
        {
            // Arrange
            var quantidadeDias = 30;
            var ordensEntregues = new List<OrdemServicoAggregate>();

            _ordemServicoRepositoryMock.Setup(r => r.ObterEntreguesUltimosDiasAsync(quantidadeDias))
                .ReturnsAsync(ordensEntregues);

            // Act
            var act = async () => await _service.ObterTempoMedio(quantidadeDias);

            // Assert
            await act.Should().ThrowAsync<DomainException>()
                .WithMessage("Nenhuma ordem de serviço entregue encontrada no período especificado.");

            _ordemServicoRepositoryMock.Verify(r => r.ObterEntreguesUltimosDiasAsync(quantidadeDias), Times.Once);
        }

        #endregion

        #region BuscaPublica Tests

        [Fact(DisplayName = "Deve retornar ordem de serviço quando código e documento conferem")]
        [Trait("Metodo", "BuscaPublica")]
        public async Task BuscaPublica_ComCodigoEDocumentoValidos_DeveRetornarOrdemServico()
        {
            // Arrange
            var veiculoId = Guid.NewGuid();
            var ordemServico = OrdemServicoAggregate.Criar(veiculoId);
            var codigo = ordemServico.Codigo.Valor;
            var documentoCliente = "12345678901";

            var dto = new BuscaPublicaOrdemServicoDto
            {
                CodigoOrdemServico = codigo,
                DocumentoIdentificadorCliente = documentoCliente
            };

            var cliente = new ClienteExternalDto
            {
                Id = Guid.NewGuid(),
                Nome = "João Silva",
                DocumentoIdentificador = documentoCliente
            };

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorCodigoAsync(codigo))
                .ReturnsAsync(ordemServico);

            _clienteExternalServiceMock.Setup(c => c.ObterClientePorVeiculoIdAsync(veiculoId))
                .ReturnsAsync(cliente);

            // Act
            var result = await _service.BuscaPublica(dto);

            // Assert
            result.Should().NotBeNull();
            result.Id.Should().Be(ordemServico.Id);
            result.VeiculoId.Should().Be(veiculoId);

            _ordemServicoRepositoryMock.Verify(r => r.ObterPorCodigoAsync(codigo), Times.Once);
            _clienteExternalServiceMock.Verify(c => c.ObterClientePorVeiculoIdAsync(veiculoId), Times.Once);
        }

        [Fact(DisplayName = "Deve retornar null quando ordem de serviço não existir")]
        [Trait("Metodo", "BuscaPublica")]
        public async Task BuscaPublica_ComOrdemServicoInexistente_DeveRetornarNull()
        {
            // Arrange
            var codigo = "OS-20250125-INEXISTENTE";
            var dto = new BuscaPublicaOrdemServicoDto
            {
                CodigoOrdemServico = codigo,
                DocumentoIdentificadorCliente = "12345678901"
            };

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorCodigoAsync(codigo))
                .ReturnsAsync((OrdemServicoAggregate?)null);

            // Act
            var result = await _service.BuscaPublica(dto);

            // Assert
            result.Should().BeNull();

            _ordemServicoRepositoryMock.Verify(r => r.ObterPorCodigoAsync(codigo), Times.Once);
            _clienteExternalServiceMock.Verify(c => c.ObterClientePorVeiculoIdAsync(It.IsAny<Guid>()), Times.Never);
        }

        [Fact(DisplayName = "Deve retornar null quando cliente não existir")]
        [Trait("Metodo", "BuscaPublica")]
        public async Task BuscaPublica_ComClienteInexistente_DeveRetornarNull()
        {
            // Arrange
            var veiculoId = Guid.NewGuid();
            var ordemServico = OrdemServicoAggregate.Criar(veiculoId);
            var codigo = ordemServico.Codigo.Valor;

            var dto = new BuscaPublicaOrdemServicoDto
            {
                CodigoOrdemServico = codigo,
                DocumentoIdentificadorCliente = "12345678901"
            };

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorCodigoAsync(codigo))
                .ReturnsAsync(ordemServico);

            _clienteExternalServiceMock.Setup(c => c.ObterClientePorVeiculoIdAsync(veiculoId))
                .ReturnsAsync((ClienteExternalDto?)null);

            // Act
            var result = await _service.BuscaPublica(dto);

            // Assert
            result.Should().BeNull();

            _ordemServicoRepositoryMock.Verify(r => r.ObterPorCodigoAsync(codigo), Times.Once);
            _clienteExternalServiceMock.Verify(c => c.ObterClientePorVeiculoIdAsync(veiculoId), Times.Once);
        }

        [Fact(DisplayName = "Deve retornar null quando documento do cliente não conferir")]
        [Trait("Metodo", "BuscaPublica")]
        public async Task BuscaPublica_ComDocumentoIncorreto_DeveRetornarNull()
        {
            // Arrange
            var veiculoId = Guid.NewGuid();
            var ordemServico = OrdemServicoAggregate.Criar(veiculoId);
            var codigo = ordemServico.Codigo.Valor;

            var dto = new BuscaPublicaOrdemServicoDto
            {
                CodigoOrdemServico = codigo,
                DocumentoIdentificadorCliente = "12345678901"
            };

            var cliente = new ClienteExternalDto
            {
                Id = Guid.NewGuid(),
                Nome = "João Silva",
                DocumentoIdentificador = "98765432100" // Documento diferente
            };

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorCodigoAsync(codigo))
                .ReturnsAsync(ordemServico);

            _clienteExternalServiceMock.Setup(c => c.ObterClientePorVeiculoIdAsync(veiculoId))
                .ReturnsAsync(cliente);

            // Act
            var result = await _service.BuscaPublica(dto);

            // Assert
            result.Should().BeNull();

            _ordemServicoRepositoryMock.Verify(r => r.ObterPorCodigoAsync(codigo), Times.Once);
            _clienteExternalServiceMock.Verify(c => c.ObterClientePorVeiculoIdAsync(veiculoId), Times.Once);
        }

        [Fact(DisplayName = "Deve retornar null quando ocorrer exceção no repository")]
        [Trait("Metodo", "BuscaPublica")]
        public async Task BuscaPublica_ComExcecaoNoRepository_DeveRetornarNull()
        {
            // Arrange
            var dto = new BuscaPublicaOrdemServicoDto
            {
                CodigoOrdemServico = "OS-20250125-TESTE",
                DocumentoIdentificadorCliente = "12345678901"
            };

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorCodigoAsync(It.IsAny<string>()))
                .ThrowsAsync(new InvalidOperationException("Database error"));

            // Act
            var result = await _service.BuscaPublica(dto);

            // Assert
            result.Should().BeNull();

            _ordemServicoRepositoryMock.Verify(r => r.ObterPorCodigoAsync(dto.CodigoOrdemServico), Times.Once);
        }

        [Fact(DisplayName = "Deve retornar null quando ocorrer exceção no serviço externo de cliente")]
        [Trait("Metodo", "BuscaPublica")]
        public async Task BuscaPublica_ComExcecaoNoServicoExternoCliente_DeveRetornarNull()
        {
            // Arrange
            var veiculoId = Guid.NewGuid();
            var ordemServico = OrdemServicoAggregate.Criar(veiculoId);
            var codigo = ordemServico.Codigo.Valor;

            var dto = new BuscaPublicaOrdemServicoDto
            {
                CodigoOrdemServico = codigo,
                DocumentoIdentificadorCliente = "12345678901"
            };

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorCodigoAsync(codigo))
                .ReturnsAsync(ordemServico);

            _clienteExternalServiceMock.Setup(c => c.ObterClientePorVeiculoIdAsync(veiculoId))
                .ThrowsAsync(new HttpRequestException("External service error"));

            // Act
            var result = await _service.BuscaPublica(dto);

            // Assert
            result.Should().BeNull();

            _ordemServicoRepositoryMock.Verify(r => r.ObterPorCodigoAsync(codigo), Times.Once);
            _clienteExternalServiceMock.Verify(c => c.ObterClientePorVeiculoIdAsync(veiculoId), Times.Once);
        }

        [Fact(DisplayName = "Deve retornar null quando ocorrer exceção inesperada")]
        [Trait("Metodo", "BuscaPublica")]
        public async Task BuscaPublica_ComExcecaoInesperada_DeveRetornarNull()
        {
            // Arrange
            var dto = new BuscaPublicaOrdemServicoDto
            {
                CodigoOrdemServico = "OS-20250125-TESTE",
                DocumentoIdentificadorCliente = "12345678901"
            };

            _ordemServicoRepositoryMock.Setup(r => r.ObterPorCodigoAsync(It.IsAny<string>()))
                .ThrowsAsync(new OutOfMemoryException("Unexpected error"));

            // Act
            var result = await _service.BuscaPublica(dto);

            // Assert
            result.Should().BeNull();

            _ordemServicoRepositoryMock.Verify(r => r.ObterPorCodigoAsync(dto.CodigoOrdemServico), Times.Once);
        }

        #endregion
    }
}
