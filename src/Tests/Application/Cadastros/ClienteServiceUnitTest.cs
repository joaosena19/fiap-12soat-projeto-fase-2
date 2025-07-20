using Application.Cadastros.Services;
using Application.Interfaces;
using Domain.Cadastros.Aggregates;
using Domain.Cadastros.ValueObjects.Cliente;
using FluentAssertions;
using Moq;
using Shared.Exceptions;

namespace Tests.Application.Cadastros
{
    public class ClienteServiceUnitTest
    {
        private readonly Mock<IClienteRepository> _repoMock;
        private readonly ClienteService _service;

        public ClienteServiceUnitTest()
        {
            _repoMock = new Mock<IClienteRepository>();
            _service = new ClienteService(_repoMock.Object);
        }

        [Fact(DisplayName = "Não deve criar cliente se CPF já existir")]
        public async Task CriarCliente_DeveLancarExcecao_SeCpfJaExistir()
        {
            // Arrange
            var cpf = "12345678909";
            var nome = "João";
            var clienteExistente = Cliente.Criar(nome, cpf);

            _repoMock.Setup(r => r.ObterPorCpfAsync(cpf))
                .ReturnsAsync(clienteExistente);

            // Act
            var act = async () => await _service.CriarCliente(nome, cpf);

            // Assert
            await act.Should().ThrowAsync<DomainException>()
                .WithMessage("Já existe um cliente cadastrado com este CPF.");

            _repoMock.Verify(r => r.SalvarAsync(It.IsAny<Cliente>()), Times.Never);
        }

        [Fact(DisplayName = "Deve criar cliente se CPF for novo")]
        public async Task CriarCliente_DeveSalvarCliente_SeCpfNaoExistir()
        {
            // Arrange
            var cpf = "12345678909";
            var nome = "João";

            var clienteNovo = Cliente.Criar(nome, cpf);

            _repoMock.Setup(r => r.ObterPorCpfAsync(cpf))
                .ReturnsAsync((Cliente?)null);

            _repoMock.Setup(r => r.SalvarAsync(It.IsAny<Cliente>()))
                .ReturnsAsync(clienteNovo);

            // Act
            await _service.CriarCliente(nome, cpf);

            // Assert
            _repoMock.Verify(r => r.SalvarAsync(It.Is<Cliente>(c =>
                c.Cpf.Valor == cpf && c.Nome.Valor == nome
            )), Times.Once);
        }

        [Fact(DisplayName = "Deve atualizar cliente se existir")]
        public async Task AtualizarCliente_DeveAtualizarCliente_SeClienteExistir()
        {
            // Arrange
            var id = Guid.NewGuid();
            var nomeOriginal = "João";
            var nomeNovo = "João Silva";
            var cpf = "12345678909";

            var clienteExistente = Cliente.Criar(nomeOriginal, cpf);
            var clienteAtualizado = Cliente.Criar(nomeNovo, cpf);

            _repoMock.Setup(r => r.ObterPorIdAsync(id))
                .ReturnsAsync(clienteExistente);

            _repoMock.Setup(r => r.AtualizarAsync(It.IsAny<Cliente>()))
                .ReturnsAsync(clienteAtualizado);

            // Act
            var result = await _service.AtualizarCliente(id, nomeNovo);

            // Assert
            result.Should().NotBeNull();
            result.Nome.Should().Be(nomeNovo);
            _repoMock.Verify(r => r.ObterPorIdAsync(id), Times.Once);
            _repoMock.Verify(r => r.AtualizarAsync(It.IsAny<Cliente>()), Times.Once);
        }

        [Fact(DisplayName = "Não deve atualizar cliente se não existir")]
        public async Task AtualizarCliente_DeveLancarExcecao_SeClienteNaoExistir()
        {
            // Arrange
            var id = Guid.NewGuid();
            var nome = "João Silva";

            _repoMock.Setup(r => r.ObterPorIdAsync(id))
                .ReturnsAsync((Cliente?)null);

            // Act
            var act = async () => await _service.AtualizarCliente(id, nome);

            // Assert
            await act.Should().ThrowAsync<DomainException>()
                .WithMessage("Cliente não encontrado.");

            _repoMock.Verify(r => r.AtualizarAsync(It.IsAny<Cliente>()), Times.Never);
        }
    }
}
