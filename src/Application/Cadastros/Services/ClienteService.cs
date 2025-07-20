using Application.Cadastros.DTO;
using Application.Interfaces;
using Domain.Cadastros.Aggregates;
using Domain.Cadastros.ValueObjects.Cliente;
using Shared.Exceptions;
using System.Net;

namespace Application.Cadastros.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepository;

        public ClienteService(IClienteRepository clienteRepository)
        {
            _clienteRepository = clienteRepository;
        }

        public async Task<RetornoClienteDTO> CriarCliente(string nome, string cpf)
        {
            var clienteExistente = await _clienteRepository.ObterPorCpfAsync(cpf);
            if (clienteExistente != null)
                throw new DomainException("Já existe um cliente cadastrado com este CPF.", HttpStatusCode.Conflict);

            var novoCliente = Cliente.Criar(nome, cpf);
            var result = await _clienteRepository.SalvarAsync(novoCliente);

            return new RetornoClienteDTO() { Id = result.Id, Nome = result.Nome.Valor, Cpf = result.Cpf.Valor };
        }

        public async Task<RetornoClienteDTO> AtualizarCliente(Guid id, string nome)
        {
            var cliente = await _clienteRepository.ObterPorIdAsync(id);
            if (cliente == null)
                throw new DomainException("Cliente não encontrado.", HttpStatusCode.NotFound);

            cliente.Atualizar(nome);
            var result = await _clienteRepository.AtualizarAsync(cliente);

            return new RetornoClienteDTO() { Id = result.Id, Nome = result.Nome.Valor, Cpf = result.Cpf.Valor };
        }

        public async Task<IEnumerable<RetornoClienteDTO>> Buscar()
        {
            var clientes = await _clienteRepository.ObterTodosAsync();
            return clientes.Select(c => new RetornoClienteDTO 
            { 
                Id = c.Id, 
                Nome = c.Nome.Valor, 
                Cpf = c.Cpf.Valor 
            });
        }

        public async Task<RetornoClienteDTO> BuscarPorId(Guid id)
        {
            var cliente = await _clienteRepository.ObterPorIdAsync(id);
            if (cliente == null)
                throw new DomainException("Cliente não encontrado.", HttpStatusCode.NotFound);

            return new RetornoClienteDTO() { Id = cliente.Id, Nome = cliente.Nome.Valor, Cpf = cliente.Cpf.Valor };
        }

        public async Task<RetornoClienteDTO> BuscarPorCpf(string cpf)
        {
            var cliente = await _clienteRepository.ObterPorCpfAsync(cpf);
            if (cliente == null)
                throw new DomainException("Cliente não encontrado.", HttpStatusCode.NotFound);

            return new RetornoClienteDTO() { Id = cliente.Id, Nome = cliente.Nome.Valor, Cpf = cliente.Cpf.Valor };
        }
    }
}
