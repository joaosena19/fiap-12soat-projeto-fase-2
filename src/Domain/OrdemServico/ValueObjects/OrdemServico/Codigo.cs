using Shared.Exceptions;
using Shared.Enums;
using System.Text.RegularExpressions;

namespace Domain.OrdemServico.ValueObjects.OrdemServico
{
    public record Codigo
    {
        private readonly string _valor = string.Empty;

        // Construtor sem parâmetro para o EF Core
        private Codigo() { }

        public Codigo(string codigo)
        {
            var _regex = new Regex(@"^OS-\d{8}-[A-Z0-9]{6}$", RegexOptions.Compiled);

            if (string.IsNullOrWhiteSpace(codigo) || !_regex.IsMatch(codigo))
                throw new DomainException($"Código {codigo} inválido. Formato esperado: OS-YYYYMMDD-ABC123", ErrorType.InvalidInput);

            _valor = codigo;
        }

        public static Codigo GerarNovo()
        {
            var data = DateTime.UtcNow.ToString("yyyyMMdd");
            var sufixo = Guid.NewGuid().ToString("N")[..6].ToUpperInvariant(); // 6 primeiros chars
            var codigo = $"OS-{data}-{sufixo}";
            return new Codigo(codigo);
        }

        public string Valor => _valor;

    }
}
