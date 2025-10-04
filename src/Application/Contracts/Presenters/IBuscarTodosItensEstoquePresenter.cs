using Domain.Estoque.Aggregates;
using Shared.Enums;

namespace Application.Contracts.Presenters;

public interface IBuscarTodosItensEstoquePresenter
{
    void ApresentarSucesso(IEnumerable<ItemEstoque> itens);
    void ApresentarErro(string mensagem, ErrorType errorType);
}