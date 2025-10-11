namespace Application.OrdemServico.Dtos
{
    /// <summary>
    /// DTO para webhook de aprovação/desaprovação de orçamento
    /// </summary>
    public class WebhookOrcamentoDto
    {
        /// <summary>
        /// ID da ordem de serviço
        /// </summary>
        /// <example>00000000-0000-0000-0000-000000000000</example>
        public Guid Id { get; set; }
    }
}