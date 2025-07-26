namespace Application.OrdemServico.DTO.External
{
    /// <summary>
    /// DTO para dados de Serviço vindos do bounded context de Cadastros
    /// </summary>
    public class ServicoExternalDTO
    {
        public Guid Id { get; set; }
        public string Nome { get; set; } = string.Empty;
        public decimal Preco { get; set; }
    }
}
