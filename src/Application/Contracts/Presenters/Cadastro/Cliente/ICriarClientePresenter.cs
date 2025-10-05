using Domain.Cadastros.Aggregates;
using Shared.Enums;

namespace Application.Contracts.Presenters
{
    public interface ICriarClientePresenter
    {
        void ApresentarSucesso(Cliente cliente);
        void ApresentarErro(string mensagem, ErrorType errorType);
    }
}