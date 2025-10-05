using Domain.Cadastros.Aggregates;
using Shared.Enums;

namespace Application.Contracts.Presenters
{
    public interface IBuscarVeiculosPresenter
    {
        void ApresentarSucesso(IEnumerable<Veiculo> veiculos);
        void ApresentarErro(string mensagem, ErrorType errorType);
    }
}