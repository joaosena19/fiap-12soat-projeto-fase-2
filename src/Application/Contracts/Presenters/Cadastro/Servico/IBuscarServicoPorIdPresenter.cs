using Domain.Cadastros.Aggregates;
using Shared.Enums;

namespace Application.Contracts.Presenters
{
    public interface IBuscarServicoPorIdPresenter
    {
        void ApresentarSucesso(Servico servico);
        void ApresentarErro(string mensagem, ErrorType errorType);
    }
}