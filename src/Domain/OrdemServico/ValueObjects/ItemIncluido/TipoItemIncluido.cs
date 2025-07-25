using Domain.OrdemServico.Enums;
using Shared.Exceptions;
using System.Net;

namespace Domain.OrdemServico.ValueObjects.ItemIncluido
{
    public record TipoItemIncluido
    {
        private readonly string _valor = string.Empty;

        // Construtor sem parâmetro para o EF Core
        private TipoItemIncluido() { }

        public TipoItemIncluido(TipoItemIncluidoEnum tipoItemIncluidoEnum)
        {
            if (!Enum.IsDefined(typeof(TipoItemIncluidoEnum), tipoItemIncluidoEnum))
            {
                var valores = string.Join(", ", Enum.GetNames(typeof(TipoItemIncluidoEnum)));
                throw new DomainException($"Tipo de item incluído na Ordem de Serviço '{tipoItemIncluidoEnum}' não é válido. Valores aceitos: {valores}.", HttpStatusCode.BadRequest);
            }

            _valor = tipoItemIncluidoEnum.ToString().ToLower();
        }

        public string Valor => _valor;
    }
}
