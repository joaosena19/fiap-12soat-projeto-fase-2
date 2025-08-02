using Application.Cadastros.Dtos;
using Domain.Cadastros.Enums;

namespace Application.Cadastros.Interfaces
{
    public interface IVeiculoService
    {
        Task<RetornoVeiculoDto> CriarVeiculo(Guid clienteId, string placa, string modelo, string marca, string cor, int ano, TipoVeiculoEnum tipoVeiculo);
        Task<RetornoVeiculoDto> AtualizarVeiculo(Guid id, string modelo, string marca, string cor, int ano, TipoVeiculoEnum tipoVeiculo);
        Task<IEnumerable<RetornoVeiculoDto>> Buscar();
        Task<RetornoVeiculoDto> BuscarPorId(Guid id);
        Task<RetornoVeiculoDto> BuscarPorPlaca(string placa);
        Task<IEnumerable<RetornoVeiculoDto>> BuscarPorClienteId(Guid clienteId);
    }
}
