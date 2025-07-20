using Domain.Cadastros.ValueObjects.Cliente;

namespace Domain.Cadastros.Aggregates
{
    public class Cliente
    {
        public Guid Id { get; private set; }
        public Nome Nome { get; private set; } = null!;
        public Cpf Cpf { get; private set; } = null!;

        // Parameterless constructor for EF Core
        private Cliente() { }

        private Cliente(Guid id, Nome nome, Cpf cpf)
        {
            Id = id;
            Nome = nome;
            Cpf = cpf;
        }

        public static Cliente Criar(string nome, string cpf)
        {
            return new Cliente(Guid.NewGuid(), new Nome(nome), new Cpf(cpf));
        }

        public void Atualizar(string nome)
        {
            Nome = new Nome(nome);
        }
    }
}
