using System.Net;
using System.Text.RegularExpressions;
using Shared.Exceptions;

namespace Domain.Cadastros.ValueObjects.Veiculo
{
    public record Placa
    {
        private readonly string _valor = string.Empty;

        // Construtor sem parâmetro para EF Core
        private Placa() { }

        public Placa(string placa)
        {
            if (string.IsNullOrWhiteSpace(placa))
                throw new DomainException("Placa não pode ser vazia", HttpStatusCode.BadRequest);

            placa = placa.Replace("-", "").ToUpper().Trim();

            if (placa.Length != 7)
                throw new DomainException("Placa deve ter exatamente 7 caracteres", HttpStatusCode.BadRequest);

            if (!Regex.IsMatch(placa, @"^[A-Z0-9]{7}$"))
                throw new DomainException("Placa deve conter apenas letras e números", HttpStatusCode.BadRequest);

            _valor = placa;
        }

        public string Valor => _valor;
    }
}
