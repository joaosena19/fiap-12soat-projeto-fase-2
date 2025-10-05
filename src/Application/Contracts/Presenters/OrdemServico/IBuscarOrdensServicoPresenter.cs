using OrdemServicoAggregate = Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico;
using Shared.Enums;

namespace Application.Contracts.Presenters;

public interface IBuscarOrdensServicoPresenter
{
    void ApresentarSucesso(IEnumerable<OrdemServicoAggregate> ordensServico);
    void ApresentarErro(string mensagem, ErrorType errorType);
}