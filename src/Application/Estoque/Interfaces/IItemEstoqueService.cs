using Application.Estoque.DTO;

namespace Application.Estoque.Interfaces
{
    public interface IItemEstoqueService
    {
        Task<RetornoItemEstoqueDTO> CriarItemEstoque(CriarItemEstoqueDTO dto);
        Task<RetornoItemEstoqueDTO> AtualizarItemEstoque(Guid id, AtualizarItemEstoqueDTO dto);
        Task<RetornoItemEstoqueDTO> AtualizarQuantidade(Guid id, AtualizarQuantidadeDTO dto);
        Task<IEnumerable<RetornoItemEstoqueDTO>> Buscar();
        Task<RetornoItemEstoqueDTO> BuscarPorId(Guid id);
        Task<RetornoDisponibilidadeDTO> VerificarDisponibilidade(Guid id, int quantidadeRequisitada);
    }
}
