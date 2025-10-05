using OrdemServicoAggregate = Domain.OrdemServico.Aggregates.OrdemServico.OrdemServico;
using Shared.Enums;

namespace Application.Contracts.Presenters;

public interface IAdicionarItemPresenter : IBasePresenter<OrdemServicoAggregate>
{
}