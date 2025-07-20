using System.Net;
using Shared.Exceptions;

namespace Domain.Cadastros.ValueObjects.Cliente
{
    public class Nome
    {
        private readonly string _valor = string.Empty;

        // Parameterless constructor for EF Core
        private Nome() { }

        public Nome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new DomainException("Nome não pode ser vazio", HttpStatusCode.BadRequest);

            if (nome.Length > 200)
                throw new DomainException("Nome não pode ter mais de 200 caracteres", HttpStatusCode.BadRequest);

            _valor = nome;
        }

        public string Valor => _valor;


    }
}
