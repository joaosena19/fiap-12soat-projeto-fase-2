using Application.Cadastros.DTO;

namespace Application.Cadastros.Interfaces
{
    public interface IServicoService
    {
        Task<RetornoServicoDTO> CriarServico(string nome, decimal preco);
        Task<RetornoServicoDTO> AtualizarServico(Guid id, string nome, decimal preco);
        Task<IEnumerable<RetornoServicoDTO>> Buscar();
        Task<RetornoServicoDTO> BuscarPorId(Guid id);
    }
}