using Domain.Estoque.Enums;

namespace Application.Estoque.DTO
{
    /// <summary>
    /// DTO para criação de item de estoque
    /// </summary>
    public class CriarItemEstoqueDTO
    {
        /// <summary>
        /// Nome do item de estoque
        /// </summary>
        /// <example>Filtro de Óleo</example>
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// Quantidade em estoque
        /// </summary>
        /// <example>50</example>
        public int Quantidade { get; set; }

        /// <summary>
        /// Tipo do item de estoque
        /// </summary>
        /// <example>Peca</example>
        public TipoItemEstoqueEnum TipoItemEstoque { get; set; }
    }
}
