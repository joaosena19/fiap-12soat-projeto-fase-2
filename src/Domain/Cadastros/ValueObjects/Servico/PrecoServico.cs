using System.Net;
using Shared.Exceptions;

namespace Domain.Cadastros.ValueObjects.Servico
{
    public class PrecoServico
    {
        private readonly decimal _valor = 0M;

        // Construtor sem parâmetro para EF Core
        private PrecoServico() { }

        public PrecoServico(decimal preco)
        {
            if (preco < 0)
                throw new DomainException("Preço não pode ser negativo", HttpStatusCode.BadRequest);

            _valor = preco;
        }

        public decimal Valor => _valor;
    }
}
