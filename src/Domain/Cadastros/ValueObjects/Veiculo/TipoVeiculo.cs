using Domain.Cadastros.Enums;
using Shared.Exceptions;
using System.Net;

namespace Domain.Cadastros.ValueObjects.Veiculo
{
    public record TipoVeiculo
    {
        private readonly string _valor = string.Empty;

        // Construtor sem parâmetro para o EF Core
        private TipoVeiculo() { }

        public TipoVeiculo(TipoVeiculoEnum tipoVeiculoEnum)
        {
            if (!Enum.IsDefined(typeof(TipoVeiculoEnum), tipoVeiculoEnum))
            {
                var valores = string.Join(", ", Enum.GetNames(typeof(TipoVeiculoEnum)));
                throw new DomainException($"Tipo de veículo '{tipoVeiculoEnum}' não é válido. Valores aceitos: {valores}.", HttpStatusCode.BadRequest);
            }

            _valor = tipoVeiculoEnum.ToString().ToLower();
        }

        public string Valor => _valor;
    }
}
