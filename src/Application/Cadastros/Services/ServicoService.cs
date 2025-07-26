using Application.Cadastros.DTO;
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

        public async Task<RetornoServicoDTO> CriarServico(string nome, decimal preco)
        {
            var servicoExistente = await _servicoRepository.ObterPorNomeAsync(nome);
            if (servicoExistente != null)
                throw new DomainException("Já existe um serviço cadastrado com este nome.", ErrorType.Conflict);

            var novoServico = Servico.Criar(nome, preco);
            var result = await _servicoRepository.SalvarAsync(novoServico);

            return _mapper.Map<RetornoServicoDTO>(result);
        }

        public async Task<RetornoServicoDTO> AtualizarServico(Guid id, string nome, decimal preco)
        {
            var servico = await _servicoRepository.ObterPorIdAsync(id);
            if (servico == null)
                throw new DomainException("Serviço não encontrado.", ErrorType.ResourceNotFound);

            servico.Atualizar(nome, preco);
            var result = await _servicoRepository.AtualizarAsync(servico);

            return _mapper.Map<RetornoServicoDTO>(result);
        }

        public async Task<IEnumerable<RetornoServicoDTO>> Buscar()
        {
            var servicos = await _servicoRepository.ObterTodosAsync();
            return _mapper.Map<IEnumerable<RetornoServicoDTO>>(servicos);
        }

        public async Task<RetornoServicoDTO> BuscarPorId(Guid id)
        {
            var servico = await _servicoRepository.ObterPorIdAsync(id);
            if (servico == null)
                throw new DomainException("Serviço não encontrado.", ErrorType.ResourceNotFound);

            return _mapper.Map<RetornoServicoDTO>(servico);
        }
    }
}
