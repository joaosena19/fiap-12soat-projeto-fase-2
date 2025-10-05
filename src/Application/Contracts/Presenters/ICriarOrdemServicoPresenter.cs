using OrdemServicoAggregate = Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico;
using Shared.Enums;

namespace Application.Contracts.Presenters;

public interface ICriarOrdemServicoPresenter
{
    void ApresentarSucesso(OrdemServicoAggregate ordemServico);
    void ApresentarErro(string mensagem, ErrorType errorType);
}