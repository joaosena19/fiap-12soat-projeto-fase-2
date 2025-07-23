using Domain.Estoque.Enums;
using Domain.Estoque.ValueObjects.ItemEstoque;
using Shared.Exceptions;
using UUIDNext;

namespace Domain.Estoque.Aggregates
{
    public class ItemEstoque
    {
        public Guid Id { get; private set; }
        public Nome Nome { get; private set; } = null!;
        public Quantidade Quantidade { get; private set; } = null!;
        public TipoItemEstoque TipoItemEstoque { get; private set; } = null!;

        // Construtor sem parâmetro para EF Core
        private ItemEstoque() { }

        private ItemEstoque(Guid id, Nome nome, Quantidade quantidade, TipoItemEstoque tipoItemEstoque)
        {
            Id = id;
            Nome = nome;
            Quantidade = quantidade;
            TipoItemEstoque = tipoItemEstoque;
        }

        public static ItemEstoque Criar(string nome, int quantidade, TipoItemEstoqueEnum tipoItemEstoque)
        {
            return new ItemEstoque(
                Uuid.NewSequential(), 
                new Nome(nome), 
                new Quantidade(quantidade), 
                new TipoItemEstoque(tipoItemEstoque)
            );
        }

        public void Atualizar(string nome, int quantidade, TipoItemEstoqueEnum tipoItemEstoque)
        {
            Nome = new Nome(nome);
            Quantidade = new Quantidade(quantidade);
            TipoItemEstoque = new TipoItemEstoque(tipoItemEstoque);
        }

        public void AtualizarQuantidade(int quantidade)
        {
            Quantidade = new Quantidade(quantidade);
        }

        public bool VerificarDisponibilidade(int quantidadeNecessaria)
        {
            if (quantidadeNecessaria <= 0)
                throw new DomainException("Quantidade requisitada deve ser maior que 0");

            return Quantidade.Valor >= quantidadeNecessaria;
        }
    }
}
