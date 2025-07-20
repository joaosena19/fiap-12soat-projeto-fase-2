using Application.Cadastros.DTO;

namespace Application.Interfaces
{
    public interface IClienteService
    {
        Task<RetornoClienteDTO> CriarCliente(string nome, string cpf);
    }
}