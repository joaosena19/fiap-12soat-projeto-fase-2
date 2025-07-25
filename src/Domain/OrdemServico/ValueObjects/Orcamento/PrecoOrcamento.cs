using System.Net;
using Shared.Exceptions;

namespace Domain.OrdemServico.ValueObjects.Orcamento
{
    public record PrecoOrcamento
    {
        private readonly decimal _valor;

        // Construtor sem parâmetro para EF Core
        private PrecoOrcamento() { }

        public PrecoOrcamento(decimal preco)
        {
            if (preco < 0)
                throw new DomainException("Preço do orçamento não pode ser negativo", HttpStatusCode.BadRequest);

            _valor = preco;
        }

        public decimal Valor => _valor;
    }
}
