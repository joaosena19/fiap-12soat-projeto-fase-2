using Application.Estoque.DTO;
using Application.Estoque.Interfaces;
using AutoMapper;
using Domain.Estoque.Aggregates;
using Shared.Enums;
using Shared.Exceptions;

namespace Application.Estoque.Services
{
    public class ItemEstoqueService : IItemEstoqueService
    {
        private readonly IItemEstoqueRepository _itemEstoqueRepository;
        private readonly IMapper _mapper;

        public ItemEstoqueService(IItemEstoqueRepository itemEstoqueRepository, IMapper mapper)
        {
            _itemEstoqueRepository = itemEstoqueRepository;
            _mapper = mapper;
        }

        public async Task<RetornoItemEstoqueDTO> CriarItemEstoque(CriarItemEstoqueDTO dto)
        {
            var itemExistente = await _itemEstoqueRepository.ObterPorNomeAsync(dto.Nome);
            if (itemExistente != null)
                throw new DomainException("Já existe um item de estoque cadastrado com este nome.", ErrorType.Conflict);

            var novoItemEstoque = ItemEstoque.Criar(dto.Nome, dto.Quantidade, dto.TipoItemEstoque, dto.Preco);
            var result = await _itemEstoqueRepository.SalvarAsync(novoItemEstoque);

            return _mapper.Map<RetornoItemEstoqueDTO>(result);
        }

        public async Task<RetornoItemEstoqueDTO> AtualizarItemEstoque(Guid id, AtualizarItemEstoqueDTO dto)
        {
            var itemEstoque = await _itemEstoqueRepository.ObterPorIdAsync(id);
            if (itemEstoque == null)
                throw new DomainException("Item de estoque não encontrado.", ErrorType.ResourceNotFound);

            itemEstoque.Atualizar(dto.Nome, dto.Quantidade, dto.TipoItemEstoque, dto.Preco);
            var result = await _itemEstoqueRepository.AtualizarAsync(itemEstoque);

            return _mapper.Map<RetornoItemEstoqueDTO>(result);
        }

        public async Task<RetornoItemEstoqueDTO> AtualizarQuantidade(Guid id, AtualizarQuantidadeDTO dto)
        {
            var itemEstoque = await _itemEstoqueRepository.ObterPorIdAsync(id);
            if (itemEstoque == null)
                throw new DomainException("Item de estoque não encontrado.", ErrorType.ResourceNotFound);

            itemEstoque.AtualizarQuantidade(dto.Quantidade);
            var result = await _itemEstoqueRepository.AtualizarAsync(itemEstoque);

            return _mapper.Map<RetornoItemEstoqueDTO>(result);
        }

        public async Task<IEnumerable<RetornoItemEstoqueDTO>> Buscar()
        {
            var itensEstoque = await _itemEstoqueRepository.ObterTodosAsync();
            return _mapper.Map<IEnumerable<RetornoItemEstoqueDTO>>(itensEstoque);
        }

        public async Task<RetornoItemEstoqueDTO> BuscarPorId(Guid id)
        {
            var itemEstoque = await _itemEstoqueRepository.ObterPorIdAsync(id);
            if (itemEstoque == null)
                throw new DomainException("Item de estoque não encontrado.", ErrorType.ResourceNotFound);

            return _mapper.Map<RetornoItemEstoqueDTO>(itemEstoque);
        }

        public async Task<RetornoDisponibilidadeDTO> VerificarDisponibilidade(Guid id, int quantidadeRequisitada)
        {
            var itemEstoque = await _itemEstoqueRepository.ObterPorIdAsync(id);
            if (itemEstoque == null)
                throw new DomainException("Item de estoque não encontrado.", ErrorType.ResourceNotFound);

            var disponivel = itemEstoque.VerificarDisponibilidade(quantidadeRequisitada);

            return new RetornoDisponibilidadeDTO
            {
                Disponivel = disponivel,
                QuantidadeEmEstoque = itemEstoque.Quantidade.Valor,
                QuantidadeSolicitada = quantidadeRequisitada
            };
        }
    }
}
