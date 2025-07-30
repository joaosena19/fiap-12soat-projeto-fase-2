using Application.Cadastros.Interfaces;
using Application.OrdemServico.Interfaces.External;

namespace Infrastructure.AntiCorruptionLayer.OrdemServico
{
    /// <summary>
    /// Anti-corruption layer para acessar veículos do bounded context de Cadastros
    /// </summary>
    public class VeiculoExternalService : IVeiculoExternalService
    {
        private readonly IVeiculoRepository _veiculoRepository;

        public VeiculoExternalService(IVeiculoRepository veiculoRepository)
        {
            _veiculoRepository = veiculoRepository;
        }

        public async Task<bool> VerificarExistenciaVeiculo(Guid veiculoId)
        {
            var veiculo = await _veiculoRepository.ObterPorIdAsync(veiculoId);
            return veiculo != null;
        }
    }
}
