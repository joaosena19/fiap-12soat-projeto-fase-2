using Application.Contracts.Gateways;
using Application.Contracts.Presenters;
using Application.OrdemServico.Interfaces.External;

namespace Application.OrdemServico.UseCases;

public class BuscaPublicaOrdemServicoUseCase
{
    public async Task ExecutarAsync(string codigoOrdemServico, string documentoIdentificadorCliente, IOrdemServicoGateway gateway, IClienteExternalService clienteExternalService, IBuscaPublicaOrdemServicoPresenter presenter)
    {
        try
        {
            var ordemServico = await gateway.ObterPorCodigoAsync(codigoOrdemServico);
            if (ordemServico == null)
            {
                presenter.ApresentarNaoEncontrado(); // Sempre retorna null para não revelar se a OS existe
                return;
            }

            var cliente = await clienteExternalService.ObterClientePorVeiculoIdAsync(ordemServico.VeiculoId);
            if (cliente == null)
            {
                presenter.ApresentarNaoEncontrado(); // Sempre retorna null para não revelar informações
                return;
            }

            // Verificar se o documento do cliente confere
            if (cliente.DocumentoIdentificador != documentoIdentificadorCliente)
            {
                presenter.ApresentarNaoEncontrado(); // Sempre retorna null para não revelar informações
                return;
            }

            presenter.ApresentarSucesso(ordemServico);
        }
        catch
        {
            // Para segurança, sempre retorna não encontrado em caso de erro
            presenter.ApresentarNaoEncontrado();
        }
    }
}