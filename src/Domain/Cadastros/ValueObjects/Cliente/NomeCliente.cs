using System.Net;
using Shared.Exceptions;

namespace Domain.Cadastros.ValueObjects.Cliente
{
    public class NomeCliente
    {
        private readonly string _valor = string.Empty;

        // Construtor sem parâmetro para EF Core
        private NomeCliente() { }

        public NomeCliente(string nome)
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
