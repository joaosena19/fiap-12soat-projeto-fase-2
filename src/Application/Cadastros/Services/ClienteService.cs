using Application.Cadastros.DTO;
using Application.Cadastros.Interfaces;
using AutoMapper;
using Domain.Cadastros.Aggregates;
using Shared.Exceptions;
using System.Net;

namespace Application.Cadastros.Services
{
    public class ClienteService : IClienteService
    {
        private readonly IClienteRepository _clienteRepository;
        private readonly IMapper _mapper;

        public ClienteService(IClienteRepository clienteRepository, IMapper mapper)
        {
            _clienteRepository = clienteRepository;
            _mapper = mapper;
        }

        public async Task<RetornoClienteDTO> CriarCliente(string nome, string cpf)
        {
            var clienteExistente = await _clienteRepository.ObterPorCpfAsync(cpf);
            if (clienteExistente != null)
                throw new DomainException("Já existe um cliente cadastrado com este CPF.", HttpStatusCode.Conflict);

            var novoCliente = Cliente.Criar(nome, cpf);
            var result = await _clienteRepository.SalvarAsync(novoCliente);

            return _mapper.Map<RetornoClienteDTO>(result);
        }

        public async Task<RetornoClienteDTO> AtualizarCliente(Guid id, string nome)
        {
            var cliente = await _clienteRepository.ObterPorIdAsync(id);
            if (cliente == null)
                throw new DomainException("Cliente não encontrado.", HttpStatusCode.NotFound);

            cliente.Atualizar(nome);
            var result = await _clienteRepository.AtualizarAsync(cliente);

            return _mapper.Map<RetornoClienteDTO>(result);
        }

        public async Task<IEnumerable<RetornoClienteDTO>> Buscar()
        {
            var clientes = await _clienteRepository.ObterTodosAsync();
            return _mapper.Map<IEnumerable<RetornoClienteDTO>>(clientes);
        }

        public async Task<RetornoClienteDTO> BuscarPorId(Guid id)
        {
            var cliente = await _clienteRepository.ObterPorIdAsync(id);
            if (cliente == null)
                throw new DomainException("Cliente não encontrado.", HttpStatusCode.NotFound);

            return _mapper.Map<RetornoClienteDTO>(cliente);
        }

        public async Task<RetornoClienteDTO> BuscarPorCpf(string cpf)
        {
            var cliente = await _clienteRepository.ObterPorCpfAsync(cpf);
            if (cliente == null)
                throw new DomainException("Cliente não encontrado.", HttpStatusCode.NotFound);

            return _mapper.Map<RetornoClienteDTO>(cliente);
        }
    }
}
