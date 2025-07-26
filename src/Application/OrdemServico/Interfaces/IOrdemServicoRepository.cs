namespace Application.OrdemServico.Interfaces
{
    public interface IOrdemServicoRepository
    {
        Task<Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico> SalvarAsync(Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico ordemServico);
        Task<Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico?> ObterPorIdAsync(Guid id);
        Task<Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico?> ObterPorCodigoAsync(string codigo);
        Task<Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico> AtualizarAsync(Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico ordemServico);
        Task<IEnumerable<Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico>> ObterTodosAsync();
        Task RemoverAsync(Guid id);
    }
}
