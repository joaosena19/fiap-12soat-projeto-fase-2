using Domain.Cadastros.Aggregates;

namespace Application.Cadastros.Interfaces
{
    public interface IClienteRepository
    {
        Task<Cliente> SalvarAsync(Cliente cliente);
        Task<Cliente?> ObterPorDocumentoAsync(string documento);
        Task<Cliente?> ObterPorIdAsync(Guid id);
        Task<Cliente> AtualizarAsync(Cliente cliente);
        Task<IEnumerable<Cliente>> ObterTodosAsync();
    }
}
