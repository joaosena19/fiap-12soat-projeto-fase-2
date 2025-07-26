using Application.OrdemServico.DTO;
using Application.OrdemServico.Interfaces;
using Application.OrdemServico.Interfaces.External;
using AutoMapper;
using Domain.OrdemServico.Enums;
using Shared.Exceptions;
using System.Net;

namespace Application.OrdemServico.Services
{
    public class OrdemServicoService : IOrdemServicoService
    {
        private readonly IOrdemServicoRepository _ordemServicoRepository;
        private readonly IServicoExternalService _servicoExternalService;
        private readonly IEstoqueExternalService _estoqueExternalService;
        private readonly IVeiculoExternalService _veiculoExternalService;
        private readonly IMapper _mapper;

        public OrdemServicoService(
            IOrdemServicoRepository ordemServicoRepository,
            IServicoExternalService servicoExternalService,
            IEstoqueExternalService estoqueExternalService,
            IVeiculoExternalService veiculoExternalService,
            IMapper mapper)
        {
            _ordemServicoRepository = ordemServicoRepository;
            _servicoExternalService = servicoExternalService;
            _estoqueExternalService = estoqueExternalService;
            _veiculoExternalService = veiculoExternalService;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RetornoOrdemServicoCompletaDTO>> Buscar()
        {
            var ordensServico = await _ordemServicoRepository.ObterTodosAsync();
            return _mapper.Map<IEnumerable<RetornoOrdemServicoCompletaDTO>>(ordensServico);
        }

        public async Task<RetornoOrdemServicoCompletaDTO> BuscarPorId(Guid id)
        {
            var ordemServico = await _ordemServicoRepository.ObterPorIdAsync(id);
            if (ordemServico == null)
                throw new DomainException("Ordem de serviço não encontrada.", HttpStatusCode.NotFound);

            return _mapper.Map<RetornoOrdemServicoCompletaDTO>(ordemServico);
        }

        public async Task<RetornoOrdemServicoCompletaDTO> BuscarPorCodigo(string codigo)
        {
            var ordemServico = await _ordemServicoRepository.ObterPorCodigoAsync(codigo);
            if (ordemServico == null)
                throw new DomainException("Ordem de serviço não encontrada.", HttpStatusCode.NotFound);

            return _mapper.Map<RetornoOrdemServicoCompletaDTO>(ordemServico);
        }

        public async Task<RetornoOrdemServicoDTO> CriarOrdemServico(CriarOrdemServicoDTO dto)
        {
            var veiculoExiste = await _veiculoExternalService.VerificarExistenciaVeiculo(dto.VeiculoId);
            if (!veiculoExiste)
                throw new DomainException("Veículo não encontrado para criar a ordem de serviço.", HttpStatusCode.UnprocessableEntity);

            Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico novaOrdemServico;
            Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico? ordemServicoExistente;

            // Apesar de improvável, é possível que o código da Ordem de Serviço se repita, por isso precisa tentar recriar caso existir
            do
            {
                novaOrdemServico = Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico.Criar(dto.VeiculoId);
                ordemServicoExistente = await _ordemServicoRepository.ObterPorCodigoAsync(novaOrdemServico.Codigo.Valor);
            } while (ordemServicoExistente != null);

            var result = await _ordemServicoRepository.SalvarAsync(novaOrdemServico);
            return _mapper.Map<RetornoOrdemServicoDTO>(result);
        }

        public async Task<RetornoOrdemServicoComServicosItensDTO> AdicionarServicos(Guid ordemServicoId, AdicionarServicosDTO dto)
        {
            var ordemServico = await ObterOrdemServicoPorId(ordemServicoId);

            foreach (var servicoId in dto.ServicosOriginaisIds)
            {
                var servico = await _servicoExternalService.ObterServicoPorIdAsync(servicoId);
                if (servico == null)
                    throw new DomainException($"Serviço com ID {servicoId} não encontrado.", HttpStatusCode.UnprocessableEntity);

                ordemServico.AdicionarServico(servico.Id, servico.Nome, servico.Preco);
            }

            var result = await _ordemServicoRepository.AtualizarAsync(ordemServico);
            return _mapper.Map<RetornoOrdemServicoComServicosItensDTO>(result);
        }

        public async Task<RetornoOrdemServicoComServicosItensDTO> AdicionarItem(Guid ordemServicoId, AdicionarItemDTO dto)
        {
            var ordemServico = await ObterOrdemServicoPorId(ordemServicoId);

            var itemEstoque = await _estoqueExternalService.ObterItemEstoquePorIdAsync(dto.ItemEstoqueOriginalId);
            if (itemEstoque == null)
                throw new DomainException($"Item de estoque com ID {dto.ItemEstoqueOriginalId} não encontrado.", HttpStatusCode.UnprocessableEntity);

            ordemServico.AdicionarItem(
                itemEstoque.Id,
                itemEstoque.Nome,
                itemEstoque.Preco,
                dto.Quantidade,
                itemEstoque.TipoItemIncluido);

            var result = await _ordemServicoRepository.AtualizarAsync(ordemServico);
            return _mapper.Map<RetornoOrdemServicoComServicosItensDTO>(result);
        }

        public async Task RemoverServico(Guid ordemServicoId, Guid servicoIncluidoId)
        {
            var ordemServico = await ObterOrdemServicoPorId(ordemServicoId);
            ordemServico.RemoverServico(servicoIncluidoId);

            await _ordemServicoRepository.AtualizarAsync(ordemServico);
        }

        public async Task RemoverItem(Guid ordemServicoId, Guid itemIncluidoId)
        {
            var ordemServico = await ObterOrdemServicoPorId(ordemServicoId);
            ordemServico.RemoverItem(itemIncluidoId);

            await _ordemServicoRepository.AtualizarAsync(ordemServico);
        }

        public async Task Cancelar(Guid ordemServicoId)
        {
            var ordemServico = await ObterOrdemServicoPorId(ordemServicoId);
            ordemServico.Cancelar();

            await _ordemServicoRepository.AtualizarAsync(ordemServico);
        }

        public async Task IniciarDiagnostico(Guid ordemServicoId)
        {
            var ordemServico = await ObterOrdemServicoPorId(ordemServicoId);
            ordemServico.IniciarDiagnostico();

            await _ordemServicoRepository.AtualizarAsync(ordemServico);
        }

        public async Task<RetornoOrcamentoDTO> GerarOrcamento(Guid ordemServicoId)
        {
            var ordemServico = await ObterOrdemServicoPorId(ordemServicoId);
            ordemServico.GerarOrcamento();

            var result = await _ordemServicoRepository.AtualizarAsync(ordemServico);
            return _mapper.Map<RetornoOrcamentoDTO>(result.Orcamento);
        }

        public async Task IniciarExecucao(Guid ordemServicoId)
        {
            var ordemServico = await ObterOrdemServicoPorId(ordemServicoId);
            ordemServico.IniciarExecucao();

            await _ordemServicoRepository.AtualizarAsync(ordemServico);
        }

        public async Task FinalizarExecucao(Guid ordemServicoId)
        {
            var ordemServico = await ObterOrdemServicoPorId(ordemServicoId);
            ordemServico.FinalizarExecucao();

            await _ordemServicoRepository.AtualizarAsync(ordemServico);
        }

        public async Task Entregar(Guid ordemServicoId)
        {
            var ordemServico = await ObterOrdemServicoPorId(ordemServicoId);
            ordemServico.Entregar();

            await _ordemServicoRepository.AtualizarAsync(ordemServico);
        }

        private async Task<Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico> ObterOrdemServicoPorId(Guid id)
        {
            var ordemServico = await _ordemServicoRepository.ObterPorIdAsync(id);
            if (ordemServico == null)
                throw new DomainException("Ordem de serviço não encontrada.", HttpStatusCode.NotFound);

            return ordemServico;
        }
    }
}
