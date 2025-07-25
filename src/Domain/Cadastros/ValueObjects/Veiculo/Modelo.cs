using System.Net;
using Shared.Exceptions;

namespace Domain.Cadastros.ValueObjects.Veiculo
{
    public record Modelo
    {
        private readonly string _valor = string.Empty;

        // Construtor sem parâmetro para EF Core
        private Modelo() { }

        public Modelo(string modelo)
        {
            if (string.IsNullOrWhiteSpace(modelo))
                throw new DomainException("Modelo não pode ser vazio", HttpStatusCode.BadRequest);

            if (modelo.Length > 200)
                throw new DomainException("Modelo não pode ter mais de 200 caracteres", HttpStatusCode.BadRequest);

            _valor = modelo.Trim();
        }

        public string Valor => _valor;
    }
}
