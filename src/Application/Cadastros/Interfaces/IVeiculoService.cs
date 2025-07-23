using Application.Cadastros.DTO;
using Domain.Cadastros.Enums;

namespace Application.Cadastros.Interfaces
{
    public interface IVeiculoService
    {
        Task<RetornoVeiculoDTO> CriarVeiculo(Guid clientId, string placa, string modelo, string marca, string cor, int ano, TipoVeiculoEnum tipoVeiculo);
        Task<RetornoVeiculoDTO> AtualizarVeiculo(Guid id, string modelo, string marca, string cor, int ano, TipoVeiculoEnum tipoVeiculo);
        Task<IEnumerable<RetornoVeiculoDTO>> Buscar();
        Task<RetornoVeiculoDTO> BuscarPorId(Guid id);
        Task<RetornoVeiculoDTO> BuscarPorPlaca(string placa);
    }
}
