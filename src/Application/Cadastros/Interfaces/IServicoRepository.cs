using Domain.Cadastros.Aggregates;

namespace Application.Cadastros.Interfaces
{
    public interface IServicoRepository
    {
        Task<Servico> SalvarAsync(Servico servico);
        Task<Servico?> ObterPorIdAsync(Guid id);
        Task<Servico?> ObterPorNomeAsync(string nome);
        Task<Servico> AtualizarAsync(Servico servico);
        Task<IEnumerable<Servico>> ObterTodosAsync();
    }
}
