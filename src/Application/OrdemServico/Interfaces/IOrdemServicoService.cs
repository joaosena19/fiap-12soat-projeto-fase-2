using Application.OrdemServico.DTO;

namespace Application.OrdemServico.Interfaces
{
    public interface IOrdemServicoService
    {
        Task<IEnumerable<RetornoOrdemServicoCompletaDTO>> Buscar();
        Task<RetornoOrdemServicoCompletaDTO> BuscarPorId(Guid id);
        Task<RetornoOrdemServicoCompletaDTO> BuscarPorCodigo(string codigo);
        Task<RetornoOrdemServicoDTO> CriarOrdemServico(CriarOrdemServicoDTO dto);
        Task<RetornoOrdemServicoComServicosItensDTO> AdicionarServicos(Guid ordemServicoId, AdicionarServicosDTO dto);
        Task<RetornoOrdemServicoComServicosItensDTO> AdicionarItem(Guid ordemServicoId, AdicionarItemDTO dto);
        Task RemoverServico(Guid ordemServicoId, Guid servicoIncluidoId);
        Task RemoverItem(Guid ordemServicoId, Guid itemIncluidoId);
        Task Cancelar(Guid ordemServicoId);
        Task IniciarDiagnostico(Guid ordemServicoId);
        Task<RetornoOrcamentoDTO> GerarOrcamento(Guid ordemServicoId);
        Task IniciarExecucao(Guid ordemServicoId);
        Task FinalizarExecucao(Guid ordemServicoId);
        Task Entregar(Guid ordemServicoId);
        Task<RetornoTempoMedioDTO> ObterTempoMedio(int quantidadeDias = 365);
    }
}
