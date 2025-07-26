using Domain.OrdemServico.Enums;
using Shared.Exceptions;
using System.Net;

namespace Domain.OrdemServico.ValueObjects.OrdemServico
{
    public record Status
    {
        private readonly string _valor = string.Empty;

        // Construtor sem parâmetro para o EF Core
        private Status() { }

        public Status(StatusOrdemServicoEnum statusOrdemServicoEnum)
        {
            if (!Enum.IsDefined(typeof(StatusOrdemServicoEnum), statusOrdemServicoEnum))
            {
                var valores = string.Join(", ", Enum.GetNames(typeof(StatusOrdemServicoEnum)));
                throw new DomainException($"Status da Ordem de Serviço '{statusOrdemServicoEnum}' não é válido. Valores aceitos: {valores}.", HttpStatusCode.UnprocessableContent);
            }

            _valor = statusOrdemServicoEnum.ToString().ToLower();
        }

        public string Valor => _valor;
    }
}
