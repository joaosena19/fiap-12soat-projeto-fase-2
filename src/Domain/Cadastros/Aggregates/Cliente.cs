using Domain.Cadastros.ValueObjects.Cliente;

namespace Domain.Cadastros.Aggregates
{
    public class Cliente
    {
        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public Cpf Cpf { get; private set; }

        // Parameterless constructor for EF Core
        private Cliente() { }

        public Cliente(Guid id, string nome, Cpf cpf)
        {
            Id = id;
            Nome = nome;
            Cpf = cpf;
        }

        public static Cliente Criar(string nome, Cpf cpf)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ArgumentException("Nome é obrigatório");

            return new Cliente(Guid.NewGuid(), nome, cpf);
        }
    }
}
