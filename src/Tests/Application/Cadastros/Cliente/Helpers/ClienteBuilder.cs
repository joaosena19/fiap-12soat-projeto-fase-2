using Bogus;
using Tests.Helpers;
using ClienteAggregate = Domain.Cadastros.Aggregates.Cliente;

namespace Tests.Application.Cadastros.Cliente.Helpers
{
    public class ClienteBuilder
    {
        private string _nome;
        private string _documento;
        private readonly Faker _faker = new Faker("pt_BR");

        public ClienteBuilder()
        {
            _nome = _faker.Person.FullName;
            _documento = DocumentoHelper.GerarCpfValido();
        }

        public ClienteBuilder ComNome(string nome)
        {
            _nome = nome;
            return this;
        }

        public ClienteBuilder ComCpfValido()
        {
            _documento = DocumentoHelper.GerarCpfValido();
            return this;
        }

        public ClienteBuilder ComCpfInvalido()
        {
            _documento = DocumentoHelper.GerarCpfInvalido();
            return this;
        }

        public ClienteBuilder ComCnpjValido()
        {
            _documento = DocumentoHelper.GerarCnpjValido();
            return this;
        }

        public ClienteBuilder ComCnpjInvalido()
        {
            _documento = DocumentoHelper.GerarCnpjInvalido();
            return this;
        }

        public ClienteAggregate Build()
        {
            return ClienteAggregate.Criar(_nome, _documento);
        }
    }
}