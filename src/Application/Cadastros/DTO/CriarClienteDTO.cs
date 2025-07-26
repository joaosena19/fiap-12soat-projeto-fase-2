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
        /// Documento de identificação do cliente (CPF ou CNPJ, com ou sem formatação)
        /// </summary>
        /// <example>12345678901</example>
        public string DocumentoIdentificador { get; set; } = string.Empty;
    }
}
