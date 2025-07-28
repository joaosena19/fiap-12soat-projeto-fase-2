using Domain.Cadastros.Aggregates;

namespace Application.Cadastros.Interfaces
{
    public interface IVeiculoRepository
    {
        Task<Veiculo> SalvarAsync(Veiculo veiculo);
        Task<Veiculo?> ObterPorPlacaAsync(string placa);
        Task<Veiculo?> ObterPorIdAsync(Guid id);
        Task<Veiculo> AtualizarAsync(Veiculo veiculo);
        Task<IEnumerable<Veiculo>> ObterTodosAsync();
        Task<IEnumerable<Veiculo>> ObterPorClienteIdAsync(Guid clienteId);
    }
}
