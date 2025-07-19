using Domain.Cadastros.Aggregates;

namespace Application.Interfaces
{
    public interface IClienteRepository
    {
        void Salvar(Cliente cliente);
        Cliente? ObterPorCpf(string cpf);
    }
}
