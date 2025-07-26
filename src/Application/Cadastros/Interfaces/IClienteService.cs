using Application.Cadastros.DTO;

namespace Application.Cadastros.Interfaces
{
    public interface IClienteService
    {
        Task<RetornoClienteDTO> CriarCliente(string nome, string documento);
        Task<RetornoClienteDTO> AtualizarCliente(Guid id, string nome);
        Task<IEnumerable<RetornoClienteDTO>> Buscar();
        Task<RetornoClienteDTO> BuscarPorId(Guid id);
        Task<RetornoClienteDTO> BuscarPorDocumento(string documento);
    }
}