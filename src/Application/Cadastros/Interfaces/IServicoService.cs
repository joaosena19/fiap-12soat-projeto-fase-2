using Application.Cadastros.Dtos;

namespace Application.Cadastros.Interfaces
{
    public interface IServicoService
    {
        Task<RetornoServicoDto> CriarServico(string nome, decimal preco);
        Task<RetornoServicoDto> AtualizarServico(Guid id, string nome, decimal preco);
        Task<IEnumerable<RetornoServicoDto>> Buscar();
        Task<RetornoServicoDto> BuscarPorId(Guid id);
    }
}