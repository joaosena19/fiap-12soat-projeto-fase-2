using Application.Cadastros.Dtos;

namespace Application.Cadastros.Interfaces
{
    public interface IClienteService
    {
        Task<RetornoClienteDto> CriarCliente(string nome, string documento);
        Task<RetornoClienteDto> AtualizarCliente(Guid id, string nome);
        Task<IEnumerable<RetornoClienteDto>> Buscar();
        Task<RetornoClienteDto> BuscarPorId(Guid id);
        Task<RetornoClienteDto> BuscarPorDocumento(string documento);
    }
}