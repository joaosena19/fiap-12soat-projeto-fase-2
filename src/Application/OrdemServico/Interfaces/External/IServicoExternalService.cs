using Application.OrdemServico.DTO.External;

namespace Application.OrdemServico.Interfaces.External
{
    /// <summary>
    /// Interface anti-corruption para acessar dados do bounded context de Cadastros (Serviços)
    /// </summary>
    public interface IServicoExternalService
    {
        Task<ServicoExternalDTO?> ObterServicoPorIdAsync(Guid servicoId);
    }
}
