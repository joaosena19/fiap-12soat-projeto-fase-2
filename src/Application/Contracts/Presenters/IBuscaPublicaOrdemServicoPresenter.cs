using OrdemServicoAggregate = Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico;
using Shared.Enums;

namespace Application.Contracts.Presenters;

public interface IBuscaPublicaOrdemServicoPresenter
{
    void ApresentarSucesso(OrdemServicoAggregate ordemServico);
    void ApresentarNaoEncontrado();
    void ApresentarErro(string mensagem, ErrorType errorType);
}