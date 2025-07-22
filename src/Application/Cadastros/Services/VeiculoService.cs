using Application.Cadastros.DTO;
using Application.Cadastros.Interfaces;
using Domain.Cadastros.Aggregates;
using Domain.Cadastros.Enums;
using Shared.Exceptions;
using System.Net;

namespace Application.Cadastros.Services
{
    public class VeiculoService : IVeiculoService
    {
        private readonly IVeiculoRepository _veiculoRepository;

        public VeiculoService(IVeiculoRepository veiculoRepository)
        {
            _veiculoRepository = veiculoRepository;
        }

        public async Task<RetornoVeiculoDTO> CriarVeiculo(string placa, string modelo, string marca, string cor, int ano, TipoVeiculoEnum tipoVeiculo)
        {
            var veiculoExistente = await _veiculoRepository.ObterPorPlacaAsync(placa);
            if (veiculoExistente != null)
                throw new DomainException("Já existe um veículo cadastrado com esta placa.", HttpStatusCode.Conflict);

            var novoVeiculo = Veiculo.Criar(placa, modelo, marca, cor, ano, tipoVeiculo);
            var result = await _veiculoRepository.SalvarAsync(novoVeiculo);

            return new RetornoVeiculoDTO()
            {
                Id = result.Id,
                Placa = result.Placa.Valor,
                Modelo = result.Modelo.Valor,
                Marca = result.Marca.Valor,
                Cor = result.Cor.Valor,
                Ano = result.Ano.Valor,
                TipoVeiculo = result.TipoVeiculo.Valor
            };
        }

        public async Task<RetornoVeiculoDTO> AtualizarVeiculo(Guid id, string modelo, string marca, string cor, int ano, TipoVeiculoEnum tipoVeiculo)
        {
            var veiculo = await _veiculoRepository.ObterPorIdAsync(id);
            if (veiculo == null)
                throw new DomainException("Veículo não encontrado.", HttpStatusCode.NotFound);

            veiculo.Atualizar(modelo, marca, cor, ano, tipoVeiculo);
            var result = await _veiculoRepository.AtualizarAsync(veiculo);

            return new RetornoVeiculoDTO()
            {
                Id = result.Id,
                Placa = result.Placa.Valor,
                Modelo = result.Modelo.Valor,
                Marca = result.Marca.Valor,
                Cor = result.Cor.Valor,
                Ano = result.Ano.Valor,
                TipoVeiculo = result.TipoVeiculo.Valor
            };
        }

        public async Task<IEnumerable<RetornoVeiculoDTO>> Buscar()
        {
            var veiculos = await _veiculoRepository.ObterTodosAsync();
            return veiculos.Select(v => new RetornoVeiculoDTO()
            {
                Id = v.Id,
                Placa = v.Placa.Valor,
                Modelo = v.Modelo.Valor,
                Marca = v.Marca.Valor,
                Cor = v.Cor.Valor,
                Ano = v.Ano.Valor,
                TipoVeiculo = v.TipoVeiculo.Valor
            });
        }

        public async Task<RetornoVeiculoDTO> BuscarPorId(Guid id)
        {
            var veiculo = await _veiculoRepository.ObterPorIdAsync(id);
            if (veiculo == null)
                throw new DomainException("Veículo não encontrado.", HttpStatusCode.NotFound);

            return new RetornoVeiculoDTO()
            {
                Id = veiculo.Id,
                Placa = veiculo.Placa.Valor,
                Modelo = veiculo.Modelo.Valor,
                Marca = veiculo.Marca.Valor,
                Cor = veiculo.Cor.Valor,
                Ano = veiculo.Ano.Valor,
                TipoVeiculo = veiculo.TipoVeiculo.Valor
            };
        }

        public async Task<RetornoVeiculoDTO> BuscarPorPlaca(string placa)
        {
            var veiculo = await _veiculoRepository.ObterPorPlacaAsync(placa);
            if (veiculo == null)
                throw new DomainException("Veículo não encontrado.", HttpStatusCode.NotFound);

            return new RetornoVeiculoDTO()
            {
                Id = veiculo.Id,
                Placa = veiculo.Placa.Valor,
                Modelo = veiculo.Modelo.Valor,
                Marca = veiculo.Marca.Valor,
                Cor = veiculo.Cor.Valor,
                Ano = veiculo.Ano.Valor,
                TipoVeiculo = veiculo.TipoVeiculo.Valor
            };
        }
    }
}
