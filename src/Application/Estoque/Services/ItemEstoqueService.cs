using Application.Estoque.Dtos;
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

        public async Task<RetornoItemEstoqueDto> CriarItemEstoque(CriarItemEstoqueDto dto)
        {
            var itemExistente = await _itemEstoqueRepository.ObterPorNomeAsync(dto.Nome);
            if (itemExistente != null)
                throw new DomainException("Já existe um item de estoque cadastrado com este nome.", ErrorType.Conflict);

            var novoItemEstoque = ItemEstoque.Criar(dto.Nome, dto.Quantidade, dto.TipoItemEstoque, dto.Preco);
            var result = await _itemEstoqueRepository.SalvarAsync(novoItemEstoque);

            return _mapper.Map<RetornoItemEstoqueDto>(result);
        }

        public async Task<RetornoItemEstoqueDto> AtualizarItemEstoque(Guid id, AtualizarItemEstoqueDto dto)
        {
            var itemEstoque = await _itemEstoqueRepository.ObterPorIdAsync(id);
            if (itemEstoque == null)
                throw new DomainException("Item de estoque não encontrado.", ErrorType.ResourceNotFound);

            itemEstoque.Atualizar(dto.Nome, dto.Quantidade, dto.TipoItemEstoque, dto.Preco);
            var result = await _itemEstoqueRepository.AtualizarAsync(itemEstoque);

            return _mapper.Map<RetornoItemEstoqueDto>(result);
        }

        public async Task<RetornoItemEstoqueDto> AtualizarQuantidade(Guid id, AtualizarQuantidadeDto dto)
        {
            var itemEstoque = await _itemEstoqueRepository.ObterPorIdAsync(id);
            if (itemEstoque == null)
                throw new DomainException("Item de estoque não encontrado.", ErrorType.ResourceNotFound);

            itemEstoque.AtualizarQuantidade(dto.Quantidade);
            var result = await _itemEstoqueRepository.AtualizarAsync(itemEstoque);

            return _mapper.Map<RetornoItemEstoqueDto>(result);
        }

        public async Task<IEnumerable<RetornoItemEstoqueDto>> Buscar()
        {
            var itensEstoque = await _itemEstoqueRepository.ObterTodosAsync();
            return _mapper.Map<IEnumerable<RetornoItemEstoqueDto>>(itensEstoque);
        }

        public async Task<RetornoItemEstoqueDto> BuscarPorId(Guid id)
        {
            var itemEstoque = await _itemEstoqueRepository.ObterPorIdAsync(id);
            if (itemEstoque == null)
                throw new DomainException("Item de estoque não encontrado.", ErrorType.ResourceNotFound);

            return _mapper.Map<RetornoItemEstoqueDto>(itemEstoque);
        }

        public async Task<RetornoDisponibilidadeDto> VerificarDisponibilidade(Guid id, int quantidadeRequisitada)
        {
            var itemEstoque = await _itemEstoqueRepository.ObterPorIdAsync(id);
            if (itemEstoque == null)
                throw new DomainException("Item de estoque não encontrado.", ErrorType.ResourceNotFound);

            var disponivel = itemEstoque.VerificarDisponibilidade(quantidadeRequisitada);

            return new RetornoDisponibilidadeDto
            {
                Disponivel = disponivel,
                QuantidadeEmEstoque = itemEstoque.Quantidade.Valor,
                QuantidadeSolicitada = quantidadeRequisitada
            };
        }
    }
}
