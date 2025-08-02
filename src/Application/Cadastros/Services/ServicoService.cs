using Application.Cadastros.Dtos;
using Application.Cadastros.Interfaces;
using AutoMapper;
using Domain.Cadastros.Aggregates;
using Shared.Enums;
using Shared.Exceptions;

namespace Application.Cadastros.Services
{
    public class ServicoService : IServicoService
    {
        private readonly IServicoRepository _servicoRepository;
        private readonly IMapper _mapper;

        public ServicoService(IServicoRepository servicoRepository, IMapper mapper)
        {
            _servicoRepository = servicoRepository;
            _mapper = mapper;
        }

        public async Task<RetornoServicoDto> CriarServico(string nome, decimal preco)
        {
            var servicoExistente = await _servicoRepository.ObterPorNomeAsync(nome);
            if (servicoExistente != null)
                throw new DomainException("Já existe um serviço cadastrado com este nome.", ErrorType.Conflict);

            var novoServico = Servico.Criar(nome, preco);
            var result = await _servicoRepository.SalvarAsync(novoServico);

            return _mapper.Map<RetornoServicoDto>(result);
        }

        public async Task<RetornoServicoDto> AtualizarServico(Guid id, string nome, decimal preco)
        {
            var servico = await _servicoRepository.ObterPorIdAsync(id);
            if (servico == null)
                throw new DomainException("Serviço não encontrado.", ErrorType.ResourceNotFound);

            servico.Atualizar(nome, preco);
            var result = await _servicoRepository.AtualizarAsync(servico);

            return _mapper.Map<RetornoServicoDto>(result);
        }

        public async Task<IEnumerable<RetornoServicoDto>> Buscar()
        {
            var servicos = await _servicoRepository.ObterTodosAsync();
            return _mapper.Map<IEnumerable<RetornoServicoDto>>(servicos);
        }

        public async Task<RetornoServicoDto> BuscarPorId(Guid id)
        {
            var servico = await _servicoRepository.ObterPorIdAsync(id);
            if (servico == null)
                throw new DomainException("Serviço não encontrado.", ErrorType.ResourceNotFound);

            return _mapper.Map<RetornoServicoDto>(servico);
        }
    }
}
