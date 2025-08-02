using Application.Cadastros.Dtos;
using Application.Cadastros.Interfaces;
using AutoMapper;
using Domain.Cadastros.Aggregates;
using Domain.Cadastros.Enums;
using Shared.Enums;
using Shared.Exceptions;

namespace Application.Cadastros.Services
{
    public class VeiculoService : IVeiculoService
    {
        private readonly IVeiculoRepository _veiculoRepository;
        private readonly IClienteRepository _clienteRepository;
        private readonly IMapper _mapper;

        public VeiculoService(IVeiculoRepository veiculoRepository, IClienteRepository clienteRepository, IMapper mapper)
        {
            _veiculoRepository = veiculoRepository;
            _clienteRepository = clienteRepository;
            _mapper = mapper;
        }

        public async Task<RetornoVeiculoDto> CriarVeiculo(Guid clienteId, string placa, string modelo, string marca, string cor, int ano, TipoVeiculoEnum tipoVeiculo)
        {
            var veiculoExistente = await _veiculoRepository.ObterPorPlacaAsync(placa.ToUpper());
            if (veiculoExistente != null)
                throw new DomainException("Já existe um veículo cadastrado com esta placa.", ErrorType.Conflict);

            var cliente = await _clienteRepository.ObterPorIdAsync(clienteId);
            if (cliente == null)
                throw new DomainException("Cliente não encontrado para realizar associação com o veículo.", ErrorType.ReferenceNotFound);

            var novoVeiculo = Veiculo.Criar(clienteId, placa, modelo, marca, cor, ano, tipoVeiculo);
            var result = await _veiculoRepository.SalvarAsync(novoVeiculo);

            return _mapper.Map<RetornoVeiculoDto>(result);
        }

        public async Task<RetornoVeiculoDto> AtualizarVeiculo(Guid id, string modelo, string marca, string cor, int ano, TipoVeiculoEnum tipoVeiculo)
        {
            var veiculo = await _veiculoRepository.ObterPorIdAsync(id);
            if (veiculo == null)
                throw new DomainException("Veículo não encontrado.", ErrorType.ResourceNotFound);

            veiculo.Atualizar(modelo, marca, cor, ano, tipoVeiculo);
            var result = await _veiculoRepository.AtualizarAsync(veiculo);

            return _mapper.Map<RetornoVeiculoDto>(result);
        }

        public async Task<IEnumerable<RetornoVeiculoDto>> Buscar()
        {
            var veiculos = await _veiculoRepository.ObterTodosAsync();
            return _mapper.Map<IEnumerable<RetornoVeiculoDto>>(veiculos);
        }

        public async Task<RetornoVeiculoDto> BuscarPorId(Guid id)
        {
            var veiculo = await _veiculoRepository.ObterPorIdAsync(id);
            if (veiculo == null)
                throw new DomainException("Veículo não encontrado.", ErrorType.ResourceNotFound);

            return _mapper.Map<RetornoVeiculoDto>(veiculo);
        }

        public async Task<RetornoVeiculoDto> BuscarPorPlaca(string placa)
        {
            var veiculo = await _veiculoRepository.ObterPorPlacaAsync(placa);
            if (veiculo == null)
                throw new DomainException("Veículo não encontrado.", ErrorType.ResourceNotFound);

            return _mapper.Map<RetornoVeiculoDto>(veiculo);
        }

        public async Task<IEnumerable<RetornoVeiculoDto>> BuscarPorClienteId(Guid clienteId)
        {
            var cliente = await _clienteRepository.ObterPorIdAsync(clienteId);
            if (cliente == null)
                throw new DomainException("Cliente não encontrado.", ErrorType.ReferenceNotFound);

            var veiculos = await _veiculoRepository.ObterPorClienteIdAsync(clienteId);
            return _mapper.Map<IEnumerable<RetornoVeiculoDto>>(veiculos);
        }
    }
}
