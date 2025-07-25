using System.Net;
using Shared.Exceptions;

namespace Domain.Cadastros.ValueObjects.Veiculo
{
    public record Cor
    {
        private readonly string _valor = string.Empty;

        // Construtor sem parâmetro para EF Core
        private Cor() { }

        public Cor(string cor)
        {
            if (string.IsNullOrWhiteSpace(cor))
                throw new DomainException("Cor não pode ser vazia", HttpStatusCode.BadRequest);

            if (cor.Length > 100)
                throw new DomainException("Cor não pode ter mais de 100 caracteres", HttpStatusCode.BadRequest);

            _valor = cor.Trim();
        }

        public string Valor => _valor;
    }
}
