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
            var clienteExistente = Cliente.Criar(new Nome(nome), new Cpf(cpf));

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

            var clienteNovo = Cliente.Criar(new Nome(nome), new Cpf(cpf));

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
    }
}
