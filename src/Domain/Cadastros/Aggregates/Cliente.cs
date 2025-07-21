using Domain.Cadastros.ValueObjects.Cliente;
using UUIDNext;

namespace Domain.Cadastros.Aggregates
{
    public class Cliente
    {
        public Guid Id { get; private set; }
        public NomeCliente Nome { get; private set; } = null!;
        public Cpf Cpf { get; private set; } = null!;

        // Parameterless constructor for EF Core
        private Cliente() { }

        private Cliente(Guid id, NomeCliente nome, Cpf cpf)
        {
            Id = id;
            Nome = nome;
            Cpf = cpf;
        }

        public static Cliente Criar(string nome, string cpf)
        {
            return new Cliente(Uuid.NewSequential(), new NomeCliente(nome), new Cpf(cpf));
        }

        public void Atualizar(string nome)
        {
            Nome = new NomeCliente(nome);
        }
    }
}
