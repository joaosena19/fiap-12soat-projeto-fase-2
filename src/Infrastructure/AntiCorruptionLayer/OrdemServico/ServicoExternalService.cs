using Application.Cadastros.Interfaces;
using Application.OrdemServico.Dtos.External;
using Application.OrdemServico.Interfaces.External;

namespace Infrastructure.AntiCorruptionLayer.OrdemServico
{
    /// <summary>
    /// Anti-corruption layer para acessar serviços do bounded context de Cadastros
    /// </summary>
    public class ServicoExternalService : IServicoExternalService
    {
        private readonly IServicoRepository _servicoRepository;

        public ServicoExternalService(IServicoRepository servicoRepository)
        {
            _servicoRepository = servicoRepository;
        }

        public async Task<ServicoExternalDto?> ObterServicoPorIdAsync(Guid servicoId)
        {
            var servico = await _servicoRepository.ObterPorIdAsync(servicoId);
            
            if (servico == null)
                return null;

            return new ServicoExternalDto
            {
                Id = servico.Id,
                Nome = servico.Nome.Valor,
                Preco = servico.Preco.Valor
            };
        }
    }
}
