using Domain.Cadastros.Aggregates;
using Shared.Enums;

namespace Application.Contracts.Presenters
{
    public interface ICriarVeiculoPresenter
    {
        void ApresentarSucesso(Veiculo veiculo);
        void ApresentarErro(string mensagem, ErrorType errorType);
    }
}