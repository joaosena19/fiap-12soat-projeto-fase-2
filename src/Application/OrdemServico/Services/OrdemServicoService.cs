using Application.OrdemServico.DTO;
using Application.OrdemServico.Interfaces;
using Application.OrdemServico.Interfaces.External;
using AutoMapper;
using Shared.Enums;
using Shared.Exceptions;

namespace Application.OrdemServico.Services
{
    public class OrdemServicoService : IOrdemServicoService
    {
        private readonly IOrdemServicoRepository _ordemServicoRepository;
        private readonly IServicoExternalService _servicoExternalService;
        private readonly IEstoqueExternalService _estoqueExternalService;
        private readonly IVeiculoExternalService _veiculoExternalService;
        private readonly IClienteExternalService _clienteExternalService;
        private readonly IMapper _mapper;

        public OrdemServicoService(
            IOrdemServicoRepository ordemServicoRepository,
            IServicoExternalService servicoExternalService,
            IEstoqueExternalService estoqueExternalService,
            IVeiculoExternalService veiculoExternalService,
            IClienteExternalService clienteExternalService,
            IMapper mapper)
        {
            _ordemServicoRepository = ordemServicoRepository;
            _servicoExternalService = servicoExternalService;
            _estoqueExternalService = estoqueExternalService;
            _veiculoExternalService = veiculoExternalService;
            _clienteExternalService = clienteExternalService;
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
                throw new DomainException("Ordem de serviço não encontrada.", ErrorType.ResourceNotFound);

            return _mapper.Map<RetornoOrdemServicoCompletaDTO>(ordemServico);
        }

        public async Task<RetornoOrdemServicoCompletaDTO> BuscarPorCodigo(string codigo)
        {
            var ordemServico = await _ordemServicoRepository.ObterPorCodigoAsync(codigo);
            if (ordemServico == null)
                throw new DomainException("Ordem de serviço não encontrada.", ErrorType.ResourceNotFound);

            return _mapper.Map<RetornoOrdemServicoCompletaDTO>(ordemServico);
        }

        public async Task<RetornoOrdemServicoDTO> CriarOrdemServico(CriarOrdemServicoDTO dto)
        {
            var veiculoExiste = await _veiculoExternalService.VerificarExistenciaVeiculo(dto.VeiculoId);
            if (!veiculoExiste)
                throw new DomainException("Veículo não encontrado para criar a ordem de serviço.", ErrorType.ReferenceNotFound);

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
            if (dto.ServicosOriginaisIds is null || dto.ServicosOriginaisIds.Count == 0)
                throw new DomainException($"É necessário informar ao menos um serviço para adiciona na Ordem de Serviço", ErrorType.InvalidInput);

            var ordemServico = await ObterOrdemServicoPorId(ordemServicoId);

            foreach (var servicoId in dto.ServicosOriginaisIds)
            {
                var servico = await _servicoExternalService.ObterServicoPorIdAsync(servicoId);
                if (servico == null)
                    throw new DomainException($"Serviço com ID {servicoId} não encontrado.", ErrorType.ReferenceNotFound);

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
                throw new DomainException($"Item de estoque com ID {dto.ItemEstoqueOriginalId} não encontrado.", ErrorType.ReferenceNotFound);

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

        public async Task AprovarOrcamento(Guid ordemServicoId)
        {
            var ordemServico = await ObterOrdemServicoPorId(ordemServicoId);

            // Verificar disponibilidade dos itens no estoque antes de aprovar o orçamento e iniciar a execução
            foreach (var itemIncluido in ordemServico.ItensIncluidos)
            {
                var disponivel = await _estoqueExternalService.VerificarDisponibilidadeAsync(itemIncluido.ItemEstoqueOriginalId, itemIncluido.Quantidade.Valor);

                if (!disponivel)
                {
                    throw new DomainException($"Item '{itemIncluido.Nome.Valor}' não está disponível no estoque na quantidade necessária ({itemIncluido.Quantidade.Valor}).", ErrorType.DomainRuleBroken);
                }
            }

            // Se todos os itens estão disponíveis - pode aprovar o orçamento e iniciar a execução
            ordemServico.AprovarOrcamento();

            // Atualizar as quantidades no estoque após aprovar o orçamento e iniciar a execução
            foreach (var itemIncluido in ordemServico.ItensIncluidos)
            {
                var itemEstoque = await _estoqueExternalService.ObterItemEstoquePorIdAsync(itemIncluido.ItemEstoqueOriginalId);
                if (itemEstoque != null)
                {
                    var novaQuantidade = itemEstoque.Quantidade - itemIncluido.Quantidade.Valor;
                    await _estoqueExternalService.AtualizarQuantidadeEstoqueAsync(itemIncluido.ItemEstoqueOriginalId, novaQuantidade);
                }
            }

            await _ordemServicoRepository.AtualizarAsync(ordemServico);
        }

        public async Task DesaprovarOrcamento(Guid ordemServicoId)
        {
            var ordemServico = await ObterOrdemServicoPorId(ordemServicoId);
            ordemServico.DesaprovarOrcamento();

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

        public async Task<RetornoTempoMedioDTO> ObterTempoMedio(int quantidadeDias = 365)
        {
            if (quantidadeDias < 1 || quantidadeDias > 365)
                throw new DomainException("A quantidade de dias deve estar entre 1 e 365.", ErrorType.InvalidInput);

            var ordensEntregues = await _ordemServicoRepository.ObterEntreguesUltimosDiasAsync(quantidadeDias);
            if (!ordensEntregues.Any())
                throw new DomainException("Nenhuma ordem de serviço entregue encontrada no período especificado.", ErrorType.DomainRuleBroken);

            // Calcular tempo médio completo (criação até entrega)
            var duracaoCompleta = ordensEntregues
                .Select(ordem => ordem.Historico.DataEntrega!.Value - ordem.Historico.DataCriacao)
                .ToList();

            var mediaCompletaTicks = duracaoCompleta.Average(d => d.Ticks);
            var duracaoMediaCompleta = new TimeSpan((long)mediaCompletaTicks);
            var tempoMedioCompletoHoras = Math.Round(duracaoMediaCompleta.TotalHours, 2);

            // Calcular tempo médio de execução (início execução até finalização)
            var duracaoExecucao = ordensEntregues
                .Select(ordem => ordem.Historico.DataFinalizacao!.Value - ordem.Historico.DataInicioExecucao!.Value)
                .ToList();

            var mediaExecucaoTicks = duracaoExecucao.Average(d => d.Ticks);
            var duracaoMediaExecucao = new TimeSpan((long)mediaExecucaoTicks);
            var tempoMedioExecucaoHoras = Math.Round(duracaoMediaExecucao.TotalHours, 2);

            return new RetornoTempoMedioDTO
            {
                QuantidadeDias = quantidadeDias,
                DataInicio = DateTime.UtcNow.AddDays(-quantidadeDias),
                DataFim = DateTime.UtcNow,
                QuantidadeOrdensAnalisadas = ordensEntregues.Count(),
                TempoMedioCompletoHoras = tempoMedioCompletoHoras,
                TempoMedioExecucaoHoras = tempoMedioExecucaoHoras
            };
        }

        public async Task<RetornoOrdemServicoCompletaDTO?> BuscaPublica(BuscaPublicaOrdemServicoDTO dto)
        {
            try
            {
                var ordemServico = await _ordemServicoRepository.ObterPorCodigoAsync(dto.CodigoOrdemServico);
                if (ordemServico == null)
                    return null; // Sempre retorna null para não revelar se a OS existe

                var cliente = await _clienteExternalService.ObterClientePorVeiculoIdAsync(ordemServico.VeiculoId);
                if (cliente == null)
                    return null; // Sempre retorna null para não revelar informações

                // Verificar se o documento do cliente confere
                if (cliente.DocumentoIdentificador != dto.DocumentoIdentificadorCliente)
                    return null; // Sempre retorna null para não revelar informações

                return _mapper.Map<RetornoOrdemServicoCompletaDTO>(ordemServico);
            }
            catch
            {
                // Para segurança, sempre retorna null em caso de erro
                return null;
            }
        }

        private async Task<Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico> ObterOrdemServicoPorId(Guid id)
        {
            var ordemServico = await _ordemServicoRepository.ObterPorIdAsync(id);
            if (ordemServico == null)
                throw new DomainException("Ordem de serviço não encontrada.", ErrorType.ResourceNotFound);

            return ordemServico;
        }
    }
}
