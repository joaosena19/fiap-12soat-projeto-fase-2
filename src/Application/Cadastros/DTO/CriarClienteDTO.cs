namespace Application.Cadastros.DTO
{
    /// <summary>
    /// DTO para criação de cliente
    /// </summary>
    public class CriarClienteDTO
    {
        /// <summary>
        /// Nome completo do cliente
        /// </summary>
        /// <example>João da Silva</example>
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// CPF do cliente (apenas números)
        /// </summary>
        /// <example>12345678901</example>
        public string Cpf { get; set; } = string.Empty;
    }
}
