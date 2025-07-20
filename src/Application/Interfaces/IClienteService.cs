namespace Application.Interfaces
{
    public interface IClienteService
    {
        Task CriarCliente(string nome, string cpf);
    }
}