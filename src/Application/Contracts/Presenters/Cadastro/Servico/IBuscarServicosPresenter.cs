using Domain.Cadastros.Aggregates;
using Shared.Enums;

namespace Application.Contracts.Presenters
{
    public interface IBuscarServicosPresenter
    {
        void ApresentarSucesso(IEnumerable<Servico> servicos);
        void ApresentarErro(string mensagem, ErrorType errorType);
    }
}