using Domain.Cadastros.Aggregates;

namespace Application.Interfaces
{
    public interface IClienteRepository
    {
        Task<Cliente> SalvarAsync(Cliente cliente);
        Task<Cliente?> ObterPorCpfAsync(string cpf);
    }
}
