using Application.Cadastros.Interfaces;
using Application.Cadastros.Services;
using AutoMapper;
using Domain.Cadastros.Aggregates;
using FluentAssertions;
using Moq;
using Shared.Exceptions;
using Application;

namespace Tests.Application.Cadastros
{
    public class ServicoServiceUnitTest
    {
        private readonly Mock<IServicoRepository> _repoMock;
        private readonly IMapper _mapper;
        private readonly ServicoService _service;

        public ServicoServiceUnitTest()
        {
            _repoMock = new Mock<IServicoRepository>();
            _mapper = AutoMapperConfig.CreateMapper();
            _service = new ServicoService(_repoMock.Object, _mapper);
        }

        [Fact(DisplayName = "Não deve criar serviço se Nome já existir")]
        [Trait("Metodo", "CriarServico")]
        public async Task CriarServico_DeveLancarExcecao_SeNomeJaExistir()
        {
            // Arrange
            var nome = "Troca de óleo";
            var preco = 150.00M;
            var servicoExistente = Servico.Criar(nome, preco);

            _repoMock.Setup(r => r.ObterPorNomeAsync(nome))
                .ReturnsAsync(servicoExistente);

            // Act
            var act = async () => await _service.CriarServico(nome, preco);

            // Assert
            await act.Should().ThrowAsync<DomainException>()
                .WithMessage("Já existe um serviço cadastrado com este nome.");

            _repoMock.Verify(r => r.SalvarAsync(It.IsAny<Servico>()), Times.Never);
        }

        [Fact(DisplayName = "Deve criar serviço se for novo")]
        [Trait("Metodo", "CriarServico")]
        public async Task CriarServico_DeveSalvarServico_SeNaoExistir()
        {
            // Arrange
            var nome = "Troca de óleo";
            var preco = 150.00M;

            var servicoNovo = Servico.Criar(nome, preco);

            _repoMock.Setup(r => r.ObterPorNomeAsync(nome))
                .ReturnsAsync((Servico?)null);

            _repoMock.Setup(r => r.SalvarAsync(It.IsAny<Servico>()))
                .ReturnsAsync(servicoNovo);

            // Act
            var result = await _service.CriarServico(nome, preco);

            // Assert
            result.Should().NotBeNull();
            result.Nome.Should().Be(nome);
            result.Preco.Should().Be(preco);
            
            _repoMock.Verify(r => r.SalvarAsync(It.Is<Servico>(s =>
                s.Nome.Valor == nome && s.Preco.Valor == preco
            )), Times.Once);
        }

        [Fact(DisplayName = "Deve atualizar serviço se existir")]
        [Trait("Metodo", "AtualizarServico")]
        public async Task AtualizarServico_DeveAtualizarServico_SeServicoExistir()
        {
            // Arrange
            var id = Guid.NewGuid();
            var nomeOriginal = "Troca de óleo";
            var precoOriginal = 100.00M;
            var novoNome = "Troca de óleo premium";
            var novoPreco = 200.00M;

            var servicoExistente = Servico.Criar(nomeOriginal, precoOriginal);
            var servicoAtualizado = Servico.Criar(novoNome, novoPreco);

            _repoMock.Setup(r => r.ObterPorIdAsync(id))
                .ReturnsAsync(servicoExistente);

            _repoMock.Setup(r => r.AtualizarAsync(It.IsAny<Servico>()))
                .ReturnsAsync(servicoAtualizado);

            // Act
            var result = await _service.AtualizarServico(id, novoNome, novoPreco);

            // Assert
            result.Should().NotBeNull();
            result.Nome.Should().Be(novoNome);
            result.Preco.Should().Be(novoPreco);
            _repoMock.Verify(r => r.ObterPorIdAsync(id), Times.Once);
            _repoMock.Verify(r => r.AtualizarAsync(It.IsAny<Servico>()), Times.Once);
        }

        [Fact(DisplayName = "Não deve atualizar serviço se não existir")]
        [Trait("Metodo", "AtualizarServico")]
        public async Task AtualizarServico_DeveLancarExcecao_SeServicoNaoExistir()
        {
            // Arrange
            var id = Guid.NewGuid();
            var nome = "Troca de óleo premium";
            var preco = 200.00M;

            _repoMock.Setup(r => r.ObterPorIdAsync(id))
                .ReturnsAsync((Servico?)null);

            // Act
            var act = async () => await _service.AtualizarServico(id, nome, preco);

            // Assert
            await act.Should().ThrowAsync<DomainException>()
                .WithMessage("Serviço não encontrado.");

            _repoMock.Verify(r => r.AtualizarAsync(It.IsAny<Servico>()), Times.Never);
        }

        [Fact(DisplayName = "Deve buscar todos os serviços")]
        [Trait("Metodo", "Buscar")]
        public async Task Buscar_DeveRetornarTodosOsServicos()
        {
            // Arrange
            var servico1 = Servico.Criar("Troca de óleo", 150.00M);
            var servico2 = Servico.Criar("Alinhamento", 80.00M);
            var servicos = new List<Servico> { servico1, servico2 };

            _repoMock.Setup(r => r.ObterTodosAsync())
                .ReturnsAsync(servicos);

            // Act
            var result = await _service.Buscar();

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            result.Should().Contain(s => s.Nome == "Troca de óleo" && s.Preco == 150.00M);
            result.Should().Contain(s => s.Nome == "Alinhamento" && s.Preco == 80.00M);
            _repoMock.Verify(r => r.ObterTodosAsync(), Times.Once);
        }

        [Fact(DisplayName = "Deve retornar lista vazia quando não há serviços")]
        [Trait("Metodo", "Buscar")]
        public async Task Buscar_DeveRetornarListaVazia_QuandoNaoHaServicos()
        {
            // Arrange
            _repoMock.Setup(r => r.ObterTodosAsync())
                .ReturnsAsync(new List<Servico>());

            // Act
            var result = await _service.Buscar();

            // Assert
            result.Should().NotBeNull();
            result.Should().BeEmpty();
            _repoMock.Verify(r => r.ObterTodosAsync(), Times.Once);
        }

        [Fact(DisplayName = "Deve buscar serviço por ID quando existir")]
        [Trait("Metodo", "BuscarPorId")]
        public async Task BuscarPorId_DeveRetornarServico_QuandoServicoExistir()
        {
            // Arrange
            var id = Guid.NewGuid();
            var nome = "Troca de óleo";
            var preco = 150.00M;
            var servico = Servico.Criar(nome, preco);

            _repoMock.Setup(r => r.ObterPorIdAsync(id))
                .ReturnsAsync(servico);

            // Act
            var result = await _service.BuscarPorId(id);

            // Assert
            result.Should().NotBeNull();
            result.Nome.Should().Be(nome);
            result.Preco.Should().Be(preco);
            _repoMock.Verify(r => r.ObterPorIdAsync(id), Times.Once);
        }

        [Fact(DisplayName = "Deve lançar exceção ao buscar serviço por ID quando não existir")]
        [Trait("Metodo", "BuscarPorId")]
        public async Task BuscarPorId_DeveLancarExcecao_QuandoServicoNaoExistir()
        {
            // Arrange
            var id = Guid.NewGuid();

            _repoMock.Setup(r => r.ObterPorIdAsync(id))
                .ReturnsAsync((Servico?)null);

            // Act
            var act = async () => await _service.BuscarPorId(id);

            // Assert
            await act.Should().ThrowAsync<DomainException>()
                .WithMessage("Serviço não encontrado.");
            _repoMock.Verify(r => r.ObterPorIdAsync(id), Times.Once);
        }
    }
}
