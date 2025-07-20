using Domain.Cadastros.ValueObjects.Cliente;

namespace Domain.Cadastros.Aggregates
{
    public class Cliente
    {
        public Guid Id { get; private set; }
        public Nome Nome { get; private set; }
        public Cpf Cpf { get; private set; }

        // Parameterless constructor for EF Core
        private Cliente() { }

        private Cliente(Guid id, Nome nome, Cpf cpf)
        {
            Id = id;
            Nome = nome;
            Cpf = cpf;
        }

        public static Cliente Criar(Nome nome, Cpf cpf)
        {
            return new Cliente(Guid.NewGuid(), nome, cpf);
        }
    }
}
