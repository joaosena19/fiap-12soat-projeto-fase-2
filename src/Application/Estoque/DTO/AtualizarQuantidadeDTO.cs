namespace Application.Estoque.DTO
{
    /// <summary>
    /// DTO para atualização de quantidade do item de estoque
    /// </summary>
    public class AtualizarQuantidadeDTO
    {

        /// <summary>
        /// Quantidade em estoque
        /// </summary>
        /// <example>50</example>
        public int Quantidade { get; set; }
    }
}
