using Domain.Estoque.Aggregates;
using Shared.Enums;

namespace Application.Contracts.Presenters;

public interface IAtualizarQuantidadePresenter
{
    void ApresentarSucesso(ItemEstoque itemEstoque);
    void ApresentarErro(string mensagem, ErrorType errorType);
}