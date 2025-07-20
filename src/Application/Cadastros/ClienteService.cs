using System.Net;
using Application.Interfaces;
using Domain.Cadastros.Aggregates;
using Domain.Cadastros.ValueObjects.Cliente;
using Shared.Exceptions;

namespace Application.Cadastros
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepository;

        public ClienteService(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        public async Task CriarCliente(string nome, string cpf)
        {
            var clienteExistente = await _clienteRepository.ObterPorCpfAsync(cpf);
            if (clienteExistente != null)
                throw new DomainException("Já existe um cliente cadastrado com este CPF.", HttpStatusCode.Conflict);

            var novoCliente = Cliente.Criar(new Nome(nome), new Cpf(cpf));
            await _clienteRepository.SalvarAsync(novoCliente);
        }
    }
}
