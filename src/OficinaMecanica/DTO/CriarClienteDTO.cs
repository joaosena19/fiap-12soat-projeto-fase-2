using System.ComponentModel.DataAnnotations;

namespace OfficinaMecanica.DTO
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
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(200, MinimumLength = 2, ErrorMessage = "Nome deve ter entre 2 e 200 caracteres")]
        public string Nome { get; set; } = string.Empty;

        /// <summary>
        /// CPF do cliente (apenas números)
        /// </summary>
        /// <example>12345678901</example>
        [Required(ErrorMessage = "CPF é obrigatório")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "CPF deve ter exatamente 11 dígitos")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "CPF deve conter apenas números")]
        public string Cpf { get; set; } = string.Empty;
    }
}
