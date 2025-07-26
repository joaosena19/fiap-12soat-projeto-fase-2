namespace Application.OrdemServico.DTO
{
    /// <summary>
    /// DTO para retorno de orçamento
    /// </summary>
    public class RetornoOrcamentoDTO
    {
        public Guid Id { get; set; } = Guid.Empty;

        /// <summary>
        /// Data de criação do orçamento
        /// </summary>
        /// <example>2025-01-25T14:30:00Z</example>
        public DateTime DataCriacao { get; set; }

        /// <summary>
        /// Preço total do orçamento
        /// </summary>
        /// <example>275.50</example>
        public decimal Preco { get; set; } = 0M;
    }
}
