using Application.Cadastros.DTO;

namespace Application.Interfaces
{
    public interface IClienteService
    {
        Task<RetornoClienteDTO> CriarCliente(string nome, string cpf);
        Task<RetornoClienteDTO> AtualizarCliente(Guid id, string nome);
        Task<IEnumerable<RetornoClienteDTO>> Buscar();
        Task<RetornoClienteDTO> BuscarPorId(Guid id);
        Task<RetornoClienteDTO> BuscarPorCpf(string cpf);
    }
}