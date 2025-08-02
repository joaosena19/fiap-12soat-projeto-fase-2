using Application.Cadastros.Interfaces;
using Application.OrdemServico.Dtos.External;
using Application.OrdemServico.Interfaces.External;

namespace Infrastructure.AntiCorruptionLayer.OrdemServico
{
    /// <summary>
    /// Anti-corruption layer para acessar clientes do bounded context de Cadastros
    /// </summary>
    public class ClienteExternalService : IClienteExternalService
    {
        private readonly IVeiculoRepository _veiculoRepository;
        private readonly IClienteRepository _clienteRepository;

        public ClienteExternalService(IVeiculoRepository veiculoRepository, IClienteRepository clienteRepository)
        {
            _veiculoRepository = veiculoRepository;
            _clienteRepository = clienteRepository;
        }

        public async Task<ClienteExternalDto?> ObterClientePorVeiculoIdAsync(Guid veiculoId)
        {
            var veiculo = await _veiculoRepository.ObterPorIdAsync(veiculoId);
            if (veiculo == null)
                return null;

            var cliente = await _clienteRepository.ObterPorIdAsync(veiculo.ClienteId);
            if (cliente == null)
                return null;

            return new ClienteExternalDto
            {
                Id = cliente.Id,
                Nome = cliente.Nome.Valor,
                DocumentoIdentificador = cliente.DocumentoIdentificador.Valor
            };
        }
    }
}
