using System.Net;
using Shared.Exceptions;

namespace Domain.Cadastros.ValueObjects.Veiculo
{
    public class Marca
    {
        private readonly string _valor = string.Empty;

        // Construtor sem parâmetro para EF Core
        private Marca() { }

        public Marca(string marca)
        {
            if (string.IsNullOrWhiteSpace(marca))
                throw new DomainException("Marca não pode ser vazia", HttpStatusCode.BadRequest);

            if (marca.Length > 200)
                throw new DomainException("Marca não pode ter mais de 200 caracteres", HttpStatusCode.BadRequest);

            _valor = marca.Trim();
        }

        public string Valor => _valor;

        public override string ToString()
        {
            return _valor;
        }

        public override bool Equals(object? obj)
        {
            if (obj is not Marca other)
                return false;

            return _valor == other._valor;
        }

        public override int GetHashCode()
        {
            return _valor.GetHashCode();
        }
    }
}
