using Application.OrdemServico.Dtos;

namespace Application.OrdemServico.Interfaces
{
    public interface IOrdemServicoService
    {
        Task<IEnumerable<RetornoOrdemServicoCompletaDto>> Buscar();
        Task<RetornoOrdemServicoCompletaDto> BuscarPorId(Guid id);
        Task<RetornoOrdemServicoCompletaDto> BuscarPorCodigo(string codigo);
        Task<RetornoOrdemServicoDto> CriarOrdemServico(CriarOrdemServicoDto dto);
        Task<RetornoOrdemServicoComServicosItensDto> AdicionarServicos(Guid ordemServicoId, AdicionarServicosDto dto);
        Task<RetornoOrdemServicoComServicosItensDto> AdicionarItem(Guid ordemServicoId, AdicionarItemDto dto);
        Task RemoverServico(Guid ordemServicoId, Guid servicoIncluidoId);
        Task RemoverItem(Guid ordemServicoId, Guid itemIncluidoId);
        Task Cancelar(Guid ordemServicoId);
        Task IniciarDiagnostico(Guid ordemServicoId);
        Task<RetornoOrcamentoDto> GerarOrcamento(Guid ordemServicoId);
        Task AprovarOrcamento(Guid ordemServicoId);
        Task DesaprovarOrcamento(Guid ordemServicoId);
        Task FinalizarExecucao(Guid ordemServicoId);
        Task Entregar(Guid ordemServicoId);
        Task<RetornoTempoMedioDto> ObterTempoMedio(int quantidadeDias = 365);
        Task<RetornoOrdemServicoCompletaDto?> BuscaPublica(BuscaPublicaOrdemServicoDto dto);
    }
}
