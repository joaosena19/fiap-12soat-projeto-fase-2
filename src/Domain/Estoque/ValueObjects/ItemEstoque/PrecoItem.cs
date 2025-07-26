using System.Net;
using Shared.Exceptions;

namespace Domain.Estoque.ValueObjects.ItemEstoque
{
    public record PrecoItem
    {
        private readonly decimal _valor = 0M;

        // Construtor sem parâmetro para EF Core
        private PrecoItem() { }

        public PrecoItem(decimal preco)
        {
            if (preco < 0)
                throw new DomainException("Preço não pode ser negativo", HttpStatusCode.BadRequest);

            _valor = preco;
        }

        public decimal Valor => _valor;
    }
}
