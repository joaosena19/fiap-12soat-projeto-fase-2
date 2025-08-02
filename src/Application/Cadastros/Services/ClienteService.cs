using Application.Cadastros.Dtos;
using Application.Cadastros.Interfaces;
using AutoMapper;
using Domain.Cadastros.Aggregates;
using Shared.Exceptions;
using Shared.Enums;

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

        public async Task<RetornoClienteDto> CriarCliente(string nome, string documento)
        {
            var clienteExistente = await _clienteRepository.ObterPorDocumentoAsync(documento);
            if (clienteExistente != null)
                throw new DomainException("Já existe um cliente cadastrado com este documento.", ErrorType.Conflict);

            var novoCliente = Cliente.Criar(nome, documento);
            var result = await _clienteRepository.SalvarAsync(novoCliente);

            return _mapper.Map<RetornoClienteDto>(result);
        }

        public async Task<RetornoClienteDto> AtualizarCliente(Guid id, string nome)
        {
            var cliente = await _clienteRepository.ObterPorIdAsync(id);
            if (cliente == null)
                throw new DomainException("Cliente não encontrado.", ErrorType.ResourceNotFound);

            cliente.Atualizar(nome);
            var result = await _clienteRepository.AtualizarAsync(cliente);

            return _mapper.Map<RetornoClienteDto>(result);
        }

        public async Task<IEnumerable<RetornoClienteDto>> Buscar()
        {
            var clientes = await _clienteRepository.ObterTodosAsync();
            return _mapper.Map<IEnumerable<RetornoClienteDto>>(clientes);
        }

        public async Task<RetornoClienteDto> BuscarPorId(Guid id)
        {
            var cliente = await _clienteRepository.ObterPorIdAsync(id);
            if (cliente == null)
                throw new DomainException("Cliente não encontrado.", ErrorType.ResourceNotFound);

            return _mapper.Map<RetornoClienteDto>(cliente);
        }

        public async Task<RetornoClienteDto> BuscarPorDocumento(string documento)
        {
            var cliente = await _clienteRepository.ObterPorDocumentoAsync(documento);
            if (cliente == null)
                throw new DomainException("Cliente não encontrado.", ErrorType.ResourceNotFound);

            return _mapper.Map<RetornoClienteDto>(cliente);
        }
    }
}
