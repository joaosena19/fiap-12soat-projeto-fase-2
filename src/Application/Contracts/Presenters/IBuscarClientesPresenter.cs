using Domain.Cadastros.Aggregates;
using Shared.Enums;

namespace Application.Contracts.Presenters
{
    public interface IBuscarClientesPresenter
    {
        void ApresentarSucesso(IEnumerable<Cliente> clientes);
        void ApresentarErro(string mensagem, ErrorType errorType);
    }
}