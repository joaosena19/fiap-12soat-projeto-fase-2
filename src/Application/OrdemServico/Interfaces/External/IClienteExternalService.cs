using Application.OrdemServico.DTO.External;

namespace Application.OrdemServico.Interfaces.External
{
    /// <summary>
    /// Interface anti-corruption para acessar dados do bounded context de Cadastros (Clientes)
    /// </summary>
    public interface IClienteExternalService
    {
        Task<ClienteExternalDTO?> ObterClientePorVeiculoIdAsync(Guid veiculoId);
    }
}
