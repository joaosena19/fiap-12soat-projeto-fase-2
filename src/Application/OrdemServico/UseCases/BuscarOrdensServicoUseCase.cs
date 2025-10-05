using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Shared.Enums;

namespace Application.OrdemServico.UseCases;

public class BuscarOrdensServicoUseCase
{
    public async Task ExecutarAsync(IOrdemServicoGateway gateway, IBuscarOrdensServicoPresenter presenter)
    {
        try
        {
            var ordensServico = await gateway.ObterTodosAsync();
            presenter.ApresentarSucesso(ordensServico);
        }
        catch (Exception)
        {
            presenter.ApresentarErro("Erro interno do servidor.", ErrorType.UnexpectedError);
        }
    }
}