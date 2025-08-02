using Application.Estoque.Dtos;

namespace Application.Estoque.Interfaces
{
    public interface IItemEstoqueService
    {
        Task<RetornoItemEstoqueDto> CriarItemEstoque(CriarItemEstoqueDto dto);
        Task<RetornoItemEstoqueDto> AtualizarItemEstoque(Guid id, AtualizarItemEstoqueDto dto);
        Task<RetornoItemEstoqueDto> AtualizarQuantidade(Guid id, AtualizarQuantidadeDto dto);
        Task<IEnumerable<RetornoItemEstoqueDto>> Buscar();
        Task<RetornoItemEstoqueDto> BuscarPorId(Guid id);
        Task<RetornoDisponibilidadeDto> VerificarDisponibilidade(Guid id, int quantidadeRequisitada);
    }
}
