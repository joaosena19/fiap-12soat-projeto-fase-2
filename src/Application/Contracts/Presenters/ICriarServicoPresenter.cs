using Domain.Cadastros.Aggregates;
using Shared.Enums;

namespace Application.Contracts.Presenters
{
    public interface ICriarServicoPresenter
    {
        void ApresentarSucesso(Servico servico);
        void ApresentarErro(string mensagem, ErrorType errorType);
    }
}