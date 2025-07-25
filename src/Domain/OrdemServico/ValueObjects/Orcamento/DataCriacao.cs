using System.Net;
using Shared.Exceptions;

namespace Domain.OrdemServico.ValueObjects.Orcamento
{
    public record DataCriacao
    {
        private readonly DateTime _valor;

        // Construtor sem parâmetro para EF Core
        private DataCriacao() { }

        public DataCriacao(DateTime dataCriacao)
        {
            if (dataCriacao == default)
                throw new DomainException("Data de criação não pode ser vazia", HttpStatusCode.BadRequest);

            _valor = dataCriacao;
        }

        public DateTime Valor => _valor;
    }
}
