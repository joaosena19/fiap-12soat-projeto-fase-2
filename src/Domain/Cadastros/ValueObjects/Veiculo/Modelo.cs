using System.Net;
using Shared.Exceptions;

namespace Domain.Cadastros.ValueObjects.Veiculo
{
    public class Modelo
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

        public override string ToString()
        {
            return _valor;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Modelo other)
                return false;

            return _valor == other._valor;
        }

        public override int GetHashCode()
        {
            return _valor.GetHashCode();
        }
    }
}
