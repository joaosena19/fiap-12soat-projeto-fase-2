using Domain.Cadastros.Enums;
using Shared.Exceptions;
using Shared.Enums;

namespace Domain.Cadastros.ValueObjects.Veiculo
{
    public record TipoVeiculo
    {
        private readonly TipoVeiculoEnum _valor;

        // Construtor sem parâmetro para o EF Core
        private TipoVeiculo() { }

        public TipoVeiculo(TipoVeiculoEnum tipoVeiculoEnum)
        {
            if (!Enum.IsDefined(typeof(TipoVeiculoEnum), tipoVeiculoEnum))
            {
                var valores = string.Join(", ", Enum.GetNames(typeof(TipoVeiculoEnum)));
                throw new DomainException($"Tipo de veículo '{tipoVeiculoEnum}' não é válido. Valores aceitos: {valores}.", ErrorType.InvalidInput);
            }

            _valor = tipoVeiculoEnum;
        }

        public TipoVeiculoEnum Valor => _valor;
    }
}
