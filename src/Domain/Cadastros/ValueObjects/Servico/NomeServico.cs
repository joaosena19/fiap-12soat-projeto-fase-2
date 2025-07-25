using System.Net;
using Shared.Exceptions;

namespace Domain.Cadastros.ValueObjects.Servico
{
    public record NomeServico
    {
        private readonly string _valor = string.Empty;

        // Construtor sem parâmetro para EF Core
        private NomeServico() { }

        public NomeServico(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new DomainException("Nome não pode ser vazio", HttpStatusCode.BadRequest);

            if (nome.Length > 500)
                throw new DomainException("Nome não pode ter mais de 500 caracteres", HttpStatusCode.BadRequest);

            _valor = nome;
        }

        public string Valor => _valor;


    }
}
