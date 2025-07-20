namespace Application.Cadastros.DTO
{
    /// <summary>
    /// DTO para atualização de cliente
    /// </summary>
    public class AtualizarClienteDTO
    {
        /// <summary>
        /// Nome completo do cliente
        /// </summary>
        /// <example>João da Silva</example>
        public string Nome { get; set; } = string.Empty;
    }
}
